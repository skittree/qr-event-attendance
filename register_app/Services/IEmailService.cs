using System;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Drawing;
using QRCoder;
using MimeKit.Utils;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using System.Threading.Tasks;
using register_app.Data;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using System.Threading;
using System.Security.Claims;
using Google.Apis.Auth.AspNetCore3;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace register_app.Services
{
    public interface IEmailService
    {
        public Task SendEmail(string targetEmail, string targetName, string attendeeKey, string eventName, DateTime eventTime, ClaimsPrincipal user);


    }

    public class EmailService : IEmailService

    {
        private ApplicationDbContext Context { get; }
        private IMapper Mapper { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private ClientSecrets Secrets { get; }

        public EmailService(ApplicationDbContext context,
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
                throw new ArgumentNullException("refreshToken");
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

        public async Task<GmailService> CreateEmailServiceAsync(ClaimsPrincipal user)
        {
            var refresh_token = await GetRefreshTokenAsync(user);
            UserCredential cred = await GetCredentialAsync(refresh_token);
            var service = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });
            return service;
        }
        public async Task SendEmail(string targetEmail, string targetName, string attendeeKey, string eventName, DateTime eventTime,ClaimsPrincipal user)
        {

            // Create the Gmail service.
            var service = await CreateEmailServiceAsync(user);

            // Create the email message.
            var message = new Message();
            message.Raw = Base64UrlEncode(CreateMessage(targetEmail,targetName, attendeeKey, eventName,eventTime));

            // Send the email.
            var request = service.Users.Messages.Send(message,"me");
            await request.ExecuteAsync();

            

        }
        private byte[] CreateMessage(string targetEmail, string targetName, string attendeeKey, string eventName, DateTime eventTime)
        {
            // Create the email message.
            var msg = new MailMessage();

            msg.To.Add(targetEmail);
            msg.From = new MailAddress("sas.events.tool@gmail.com");
            msg.Subject = "* Notification * Registration for SAS event";

            msg.IsBodyHtml = true;

            string htmlBody = "<b>Hello, " + targetName + " !</b><div><br>Thank you for registering for event: " + eventName + ".<br><br>Event Date: " + eventTime.Date.ToString() + " <br><br>Event starts at: " + eventTime.TimeOfDay.ToString() + "<br><>br>Please use QR code in the attached files, to authorize at the entrance. <br><br> Will be happy to see you soon!div/>";

            msg.Body = htmlBody;

            Bitmap bitmap = getQRCode(attendeeKey); // replace with your own method to get the image as a Bitmap
            MimeMessage mimeMessage = new MimeMessage { };
            if (bitmap != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Position = 0;
                    Attachment imageAttachment = new Attachment(ms, "image.jpeg", MediaTypeNames.Image.Jpeg);
                    msg.Attachments.Add(imageAttachment);
                    mimeMessage = MimeMessage.CreateFromMailMessage(msg);
                }
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(mimeMessage.ToString());

            return bytes;
        }

        private string Base64UrlEncode(byte[] input)
        {
            return System.Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }




        public Bitmap getQRCode(string attendeeKey)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(attendeeKey, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeAsBitmap = qrCode.GetGraphic(20);
                return qrCodeAsBitmap;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
