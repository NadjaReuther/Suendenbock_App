using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;

namespace Suendenbock_App.Services
{
    public interface ICachedDataService
    {
        Task<List<LightCard>> GetLightCardsAsync();
        Task<List<Abenteuerrang>> GetAbenteuerrangeAsync();
        Task<List<Anmeldungsstatus>> GetAnmeldungsstatusseAsync();
        Task<List<Infanterie>> GetInfanterieAsync();
    }
    public class CachedDataService : ICachedDataService
    {
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _context;

        public CachedDataService(IMemoryCache cache, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
        }

        public async Task<List<LightCard>> GetLightCardsAsync()
        {
            return await _cache.GetOrCreateAsync("LightCards", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24); // Cache for 1 day
                return await _context.LightCards.ToListAsync();
            });
        }

        public async Task<List<Abenteuerrang>> GetAbenteuerrangeAsync()
        {
            return await _cache.GetOrCreateAsync("Abenteuerrang", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24); // Cache for 1 day
                return await _context.Abenteuerraenge.ToListAsync();
            });
        }

        public Task<List<Anmeldungsstatus>> GetAnmeldungsstatusseAsync()
        {
            return _cache.GetOrCreateAsync("Anmeldungsstatus", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24); // Cache for 1 day
                return await _context.Anmeldungsstati.ToListAsync();
            });
        }

        public Task<List<Infanterie>> GetInfanterieAsync()
        {
            return _cache.GetOrCreateAsync("Infanterie", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
                return await _context.Infanterien.ToListAsync();
            });
        }
    }
}
