using AutoMapper;
using Microsoft.AspNetCore.Identity;
using register_app.Data;
using Microsoft.Extensions.Options;
using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Drive.v3;
using Google.Apis.Forms.v1;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Forms.v1.Data;
using Google.Apis.Services;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;

namespace register_app.Services
{
    public interface IFormService
    {
        Task<List<File>> GetAllFormFilesAsync(ClaimsPrincipal user); //drive api called once
        Task<List<Form>> GetAllFormsAsync(ClaimsPrincipal user); //drive api called once + forms api called for every form 
        Task<Form> GetFormAsync(ClaimsPrincipal user, string id); //forms api called once
        Task<IdentityResult> SetRefreshTokenAsync(ClaimsPrincipal user, string new_token);

    }
    public class FormService : IFormService
    {
        private ApplicationDbContext Context { get; }
        private IMapper Mapper { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private ClientSecrets Secrets { get; }

        public FormService(ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            IOptions<ClientSecrets> secret)
        {
            Context = context;
            Mapper = mapper;
            UserManager = userManager;
            Secrets = secret.Value;
        }

        public async Task<UserCredential> GetCredentialAsync(string refresh_token)
        {
            TokenResponse tokenReponse = new TokenResponse()
            {
                RefreshToken = refresh_token
            };

            var initializer = new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = Secrets
            };

            initializer.IncludeGrantedScopes = true;

            var userCredential = new UserCredential(
               new GoogleAuthorizationCodeFlow(initializer),
               "user",
               tokenReponse);

            await userCredential.RefreshTokenAsync(CancellationToken.None);
            return userCredential;
        }

        public async Task<string> GetRefreshTokenAsync(ClaimsPrincipal user)
        {
            IdentityUser context_user = await UserManager.GetUserAsync(user);
            var refresh_token = await UserManager.GetAuthenticationTokenAsync(
                context_user,
                GoogleOpenIdConnectDefaults.AuthenticationScheme,
                "refresh_token"
            );

            if (refresh_token == null)
            {
                throw new ArgumentNullException("refresh token not found in database");
            }
            return refresh_token;
        }

        public async Task<IdentityResult> SetRefreshTokenAsync(ClaimsPrincipal user, string new_token)
        {
            IdentityUser context_user = await UserManager.GetUserAsync(user);
            var result = await UserManager.SetAuthenticationTokenAsync(
                context_user, 
                GoogleOpenIdConnectDefaults.AuthenticationScheme, 
                "refresh_token", 
                new_token);
            return result;
        }

        public async Task<FormsService> CreateFormsServiceAsync(ClaimsPrincipal user)
        {
            var refresh_token = await GetRefreshTokenAsync(user);
            UserCredential cred = await GetCredentialAsync(refresh_token);
            var service = new FormsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });
            return service;
        }

        public async Task<DriveService> CreateDriveServiceAsync(ClaimsPrincipal user)
        {
            var refresh_token = await GetRefreshTokenAsync(user);
            UserCredential cred = await GetCredentialAsync(refresh_token);
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });
            return service;
        }

        public async Task<Form> GetFormAsync(ClaimsPrincipal user, string id)
        {
            var service = await CreateFormsServiceAsync(user);
            var form = await service.Forms.Get(id).ExecuteAsync();
            return form;
        }

        //get all files that are forms from the drive
        public async Task<List<File>> GetAllFormFilesAsync(ClaimsPrincipal user)
        {
            var service = await CreateDriveServiceAsync(user);
            var files = await service.Files.List().ExecuteAsync();
            var forms = files.Files.Where(x => x.MimeType == "application/vnd.google-apps.form").ToList();
            return forms;
        }

        public async Task<List<Form>> GetAllFormsAsync(ClaimsPrincipal user)
        {
            var files = await GetAllFormFilesAsync(user);
            var service = await CreateFormsServiceAsync(user);
            List<Form> forms = new List<Form>();
            foreach (var file in files)
            {
                forms.Add(await service.Forms.Get(file.Id).ExecuteAsync());
            }
            return forms;
        }
    }
}
