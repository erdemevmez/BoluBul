using BoluBul.Data;
using BoluBul.Models;
using BoluBul.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BoluBul.Services.Implementations
{
    public class BusinessStatService : IBusinessStatService
    {
        private readonly ApplicationDbContext _context;

        public BusinessStatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task IncrementViewAsync(int businessId)
        {
            return IncrementAsync(businessId, stat =>
            {
                stat.ViewCount++;
                stat.LastViewedAt = DateTime.UtcNow;
            });
        }

        public Task IncrementPhoneClickAsync(int businessId)
        {
            return IncrementAsync(businessId, stat => stat.PhoneClickCount++);
        }

        public Task IncrementWhatsAppClickAsync(int businessId)
        {
            return IncrementAsync(businessId, stat => stat.WhatsAppClickCount++);
        }

        public Task IncrementDirectionClickAsync(int businessId)
        {
            return IncrementAsync(businessId, stat => stat.DirectionClickCount++);
        }

        private async Task IncrementAsync(int businessId, Action<BusinessStat> increment)
        {
            var stat = await _context.BusinessStats.FirstOrDefaultAsync(s => s.BusinessId == businessId);

            if (stat is null)
            {
                stat = new BusinessStat { BusinessId = businessId };
                _context.BusinessStats.Add(stat);
            }

            increment(stat);
            await _context.SaveChangesAsync();
        }
    }
}
