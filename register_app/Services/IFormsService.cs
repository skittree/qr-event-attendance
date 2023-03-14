


using AutoMapper;
using Microsoft.AspNetCore.Identity;
using register_app.Data;
using Google.Apis.Forms.v1;
using System.Net.Http;
using Google.Apis.Services;
using System.Threading.Tasks;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Drive.v3;
using System.Linq;
using System.Collections.Generic;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Forms.v1.Data;

namespace register_app.Services
{
    public interface IFormService
    {
        Task<List<File>> GetAllFormFilesAsync(); //drive api called once
        Task<List<Form>> GetAllFormsAsync(); //drive api called once + forms api called for every form 
        Task<Form> GetFormAsync(string id); //forms api called once

    }
    //give permissions to app
    [GoogleScopedAuthorize(DriveService.ScopeConstants.DriveReadonly, FormsService.ScopeConstants.FormsBody)]
    public class FormService : IFormService
    {
        private ApplicationDbContext Context { get; }
        private IMapper Mapper { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private IGoogleAuthProvider Auth { get; }

        public FormService(ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            IGoogleAuthProvider auth)
        {
            Context = context;
            Mapper = mapper;
            UserManager = userManager;
            Auth = auth;
        }
        public async Task<Form> GetFormAsync(string id)
        {
            GoogleCredential cred = await Auth.GetCredentialAsync();
            var service = new FormsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });
            var form = await service.Forms.Get(id).ExecuteAsync();
            return form;
        }
        //get all files that are forms from the drive
        public async Task<List<File>> GetAllFormFilesAsync()
        {
            GoogleCredential cred = await Auth.GetCredentialAsync();
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });
            var files = await service.Files.List().ExecuteAsync();
            var forms = files.Files.Where(x => x.MimeType == "application/vnd.google-apps.form").ToList();
            return forms;
        }
        public async Task<List<Form>> GetAllFormsAsync()
        {
            var files = await GetAllFormFilesAsync();
            GoogleCredential cred = await Auth.GetCredentialAsync();
            var service = new FormsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });
            List<Form> forms = new List<Form>();
            foreach (var file in files)
            {
                forms.Add(await service.Forms.Get(file.Id).ExecuteAsync());
            }
            return forms;
        }
    }
}
