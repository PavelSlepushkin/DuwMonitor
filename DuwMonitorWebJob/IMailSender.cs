namespace DuwMonitorWebJob
{
    public interface IMailSender
    {
        void SendEmail(Message message);
    }
}