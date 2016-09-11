using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace DuwMonitorWebJob
{
    public class Functions
    {
        [NoAutomaticTrigger]
        public static void CheckDuwFreeDates()
        {
            var duwStorage = new DuwStorage();
            var mailSender = new MailSender();
            var duw = new Duw(duwStorage, mailSender);
            var duwInfo = duw.GetInfoForNext15Days();
            var freeDays = duwInfo.Where(it => it.Value).ToArray();
            if (freeDays.Any())
            {
                Parallel.ForEach(
                    new[] {"dima.oleschenko@gmail.com", "dima.oleshchenko@outlook.com", "mary.redko@gmail.com"},
                    recipient =>
                    {
                        mailSender.SendEmail(new Message
                        {
                            Recepient = recipient,
                            Sender = "duwmonitor@monitoring.com",
                            Subject = "THEY HAVE A FREE DATE!",
                            Body = $"They have free date(s): {string.Join("|", freeDays)}."
                        });
                    });
                
            }
        }
    }
}
