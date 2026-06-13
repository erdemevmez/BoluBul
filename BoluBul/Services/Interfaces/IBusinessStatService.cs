namespace BoluBul.Services.Interfaces
{
    public interface IBusinessStatService
    {
        Task IncrementViewAsync(int businessId);

        Task IncrementPhoneClickAsync(int businessId);

        Task IncrementWhatsAppClickAsync(int businessId);

        Task IncrementDirectionClickAsync(int businessId);
    }
}
