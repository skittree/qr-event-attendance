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
using register_app.ViewModels;
using Google.Apis.Pubsub.v1;
using Google.Apis.Gmail.v1;

namespace register_app.Services
{
    public interface IFormService
    {
        Task<List<File>> GetAllFormFilesAsync(); //drive api called once
        Task<List<Form>> GetAllFormsAsync(); //drive api called once + forms api called for every form 
        Task<Form> GetFormAsync(string id); //forms api called
        Task<string> CreateFormAsync(EventCreateViewModel model);
        Task<IdentityResult> SetRefreshTokenAsync(string new_token);

        Task<GmailService> GetGmailService();
        Task<List<AttendeeCreateViewModel>> GetFormResponsesAsync(string formid);

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

        private async Task<UserCredential> GetCredentialAsync(string refresh_token)
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

        private async Task<string> GetRefreshTokenAsync()
        {
            IdentityUser context_user = await UserManager.FindByNameAsync("admin");
            var refresh_token = await UserManager.GetAuthenticationTokenAsync(
                context_user,
                GoogleOpenIdConnectDefaults.AuthenticationScheme,
                "refresh_token"
            );

            if (refresh_token == null)
            {
                throw new ArgumentNullException("refreshToken");
            }
            return refresh_token;
        }

        public async Task<IdentityResult> SetRefreshTokenAsync(string new_token)
        {
            IdentityUser context_user = await UserManager.FindByNameAsync("admin");
            var result = await UserManager.SetAuthenticationTokenAsync(
                context_user, 
                GoogleOpenIdConnectDefaults.AuthenticationScheme, 
                "refresh_token", 
                new_token);
            return result;
        }

        private async Task<FormsService> CreateFormsServiceAsync()
        {
            var refresh_token = await GetRefreshTokenAsync();
            UserCredential cred = await GetCredentialAsync(refresh_token);
            var service = new FormsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });
            return service;
        }

        private async Task<DriveService> CreateDriveServiceAsync()
        {
            var refresh_token = await GetRefreshTokenAsync();
            UserCredential cred = await GetCredentialAsync(refresh_token);
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });
            return service;
        }

        public async Task<GmailService> GetGmailService()
        {
            var refresh_token = await GetRefreshTokenAsync();
            UserCredential cred = await GetCredentialAsync(refresh_token);
            var service = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });
            return service;
        }

        public async Task<string> CreateFormAsync(EventCreateViewModel model)
        {
            var service = await CreateFormsServiceAsync();

            Form form = new Form {
                Info = new Info {
                    Title = model.Name,
                    DocumentTitle = model.Name 
                }
            };

            var result = await service.Forms.Create(form).ExecuteAsync();

            string formid = result.FormId;
            BatchUpdateFormRequest update = new BatchUpdateFormRequest
            {
                Requests = new List<Request> {
                    new Request {
                        UpdateFormInfo = new UpdateFormInfoRequest {
                            Info = new Info {
                                Title = model.Name,
                                Description = model.Description
                            },
                            UpdateMask = "*"
                        }
                    },
                    new Request {
                        CreateItem = new CreateItemRequest {
                            Item = new Item {
                                Title = "Email",
                                Description = "Enter your email",
                                QuestionItem = new QuestionItem {
                                    Question = new Question {
                                        TextQuestion = new TextQuestion{ },
                                        Required = true
                                    }
                                },
                            },
                            Location = new Location { Index = 0 }
                        }
                    },
                    new Request {
                        CreateItem = new CreateItemRequest {
                            Item = new Item {
                                Title = "Name",
                                Description = "Enter your real name",
                                QuestionItem = new QuestionItem {
                                    Question = new Question {
                                        TextQuestion = new TextQuestion{ },
                                        Required = true
                                    }
                                },
                            },
                            Location = new Location { Index = 1 }
                        }
                    }
                }
            };

            await service.Forms.BatchUpdate(update, formid).ExecuteAsync();
            await AddWatchAsync(formid);

            return formid;
        }

        private async Task<Watch> AddWatchAsync(string formid)
        {
            var service = await CreateFormsServiceAsync();

            // Set up the watch request.
            var watchRequest = service.Forms.Watches.Create(new CreateWatchRequest()
            {
                Watch = new Watch
                {
                    EventType = "RESPONSES",
                    Target = new WatchTarget
                    {
                        Topic = new CloudPubsubTopic
                        {
                            TopicName = "projects/sas-event-registration-system/topics/watches"
                        }
                    }
                }
            }, formid);

            // Register the watch request with the Google Forms API.
            Watch response = await watchRequest.ExecuteAsync();
            return response;
        }

        public async Task<Form> GetFormAsync(string id)
        {
            var service = await CreateFormsServiceAsync();
            var form = await service.Forms.Get(id).ExecuteAsync();
            return form;
        }

        //get all files that are forms from the drive
        public async Task<List<File>> GetAllFormFilesAsync()
        {
            var service = await CreateDriveServiceAsync();
            var files = await service.Files.List().ExecuteAsync();
            var forms = files.Files.Where(x => x.MimeType == "application/vnd.google-apps.form").ToList();
            return forms;
        }

        public async Task<List<AttendeeCreateViewModel>> GetFormResponsesAsync(string formid)
        {
            var service = await CreateFormsServiceAsync();
            var response_list = await service.Forms.Responses.List(formid).ExecuteAsync();
            var responses = response_list.Responses;
            List<AttendeeCreateViewModel> viewmodels = new List<AttendeeCreateViewModel>();
            foreach (var response in responses)
            {
                AttendeeCreateViewModel model = new AttendeeCreateViewModel();
                var answers = response.Answers.Values.ToArray();
                model.Email = answers[1].TextAnswers.Answers[0].Value;
                model.Name = answers[0].TextAnswers.Answers[0].Value;
                viewmodels.Add(model);
            }
            return viewmodels;
        }

        public async Task<List<Form>> GetAllFormsAsync()
        {
            var files = await GetAllFormFilesAsync();
            var service = await CreateFormsServiceAsync();
            List<Form> forms = new List<Form>();
            foreach (var file in files)
            {
                forms.Add(await service.Forms.Get(file.Id).ExecuteAsync());
            }
            return forms;
        }
    }
}
