using MailKit.Security;
using MimeKit;

namespace InfoPoster_backend.Services
{
    public class EmailService
    {
        public async Task Send(string message, string email, string from, string subject)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(subject, "jack@cityguide.vn"));
            emailMessage.To.Add(new MailboxAddress(string.Empty, string.IsNullOrEmpty(email) ? email.ToLowerInvariant() : email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("pro205.emailserver.vn", 465, SecureSocketOptions.SslOnConnect).ConfigureAwait(false);
                await client.AuthenticateAsync("jack@cityguide.vn", "g7EuAf]7R9").ConfigureAwait(false); // Error -- One or more errors occurred. (504: 5.7.4 Unrecognized authentication type)
                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}
