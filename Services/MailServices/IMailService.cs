namespace api.Services.MailServices
{
    public interface IMailService
    {
        void SendEmail(Messages message);
        Task<bool> IsValidEmailAsync(string email);
    }
}
