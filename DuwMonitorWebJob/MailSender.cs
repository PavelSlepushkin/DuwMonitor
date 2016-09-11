using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DuwMonitorWebJob
{
    public class MailSender: IMailSender
    {
        public void SendEmail(Message message)
        {
            var from = new Email(message.Sender);
            var to = new Email(message.Recepient);
            var content = new Content("text/plain", message.Body);
            var mail = new Mail(from, message.Subject, to, content);

            Task.Run(async () => await SendEmailAsync(mail)).Wait();
        }

        private async Task SendEmailAsync(Mail mail)
        {
            dynamic sg = new SendGridAPIClient("");
            await sg.client.mail.send.post(requestBody: mail.Get());
        }

    }
}