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
        public Task SendEmail(string targetEmail, string targetName, string attendeeKey, string eventName, DateTime eventTime);


    }

    public class EmailService : IEmailService

    {
        private ApplicationDbContext Context { get; }
        private IMapper Mapper { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private ClientSecrets Secrets { get; }

        private IFormService FormService { get; }

        public EmailService(ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            IOptions<ClientSecrets> secret,
            IFormService formService)
        {
            Context = context;
            Mapper = mapper;
            UserManager = userManager;
            Secrets = secret.Value;
            FormService = formService;

        }

        public async Task SendEmail(string targetEmail, string targetName, string attendeeKey, string eventName, DateTime eventTime)
        {

            // Create the Gmail service.
            var service = await FormService.GetGmailService();

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
