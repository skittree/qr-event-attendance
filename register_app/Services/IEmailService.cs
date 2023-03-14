using System;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Drawing;
using QRCoder;
using MimeKit.Utils;

namespace register_app.Services
{
    public interface IEmailService
    {
        public void SendNotification(string targetEmail, string targetName, string attendeeKey, string eventName, DateTime eventTime);

        public byte[] getQRCode(string attendeeKey);

    }

    public class EmailService : IEmailService

    {
        private string GmailServer { get; }

        private string Email { get; }

        private string Password { get; }

        public EmailService ()
        {
            
            GmailServer = "smtp.gmail.com";
            Email = "sas.events.tools@gmail.com";
            Password = "r3g1str@t10n";
        }

        public void SendNotification(string targetEmail, string targetName, string attendeeKey, string eventName, DateTime eventTime)
        {
            try
            {
                var email = new MimeMessage();

                email.From.Add(new MailboxAddress("SAS Event Registration System", Email));

                email.To.Add(new MailboxAddress(targetName, targetEmail));

                email.Subject = "*Notification* Registration for SAS event";

                var image_bytes = getQRCode(attendeeKey);

                var image_id = MimeUtils.GenerateMessageId();

                var image_type = "image/png";


                var builder = new BodyBuilder();

                builder.HtmlBody = "<b>Hello, " + targetName + " !</b><div><br>Thank you for registering for event: " + eventName + ".<br><br>Event Date: " + eventTime.Date.ToString() + " <br><br>Event starts at: " + eventTime.TimeOfDay.ToString() + "<br><>br>Please use QR code in the attached files, to authorize at the entrance. <br><br> Will be happy to see you soon!div/>";

                builder.Attachments.Add(image_id, image_bytes, ContentType.Parse(image_type));

                email.Body = builder.ToMessageBody();

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect(GmailServer, 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    smtp.Authenticate(Email, Password);

                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            }


        public byte[] getQRCode(string attendeeKey)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(attendeeKey, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);
                return qrCodeAsPngByteArr;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
