using System;
using System.Threading.Tasks;
using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Epsic.Info3e.Mays.Services
{
    public class DbPremiumService : IPremiumService
    {
        private readonly MaysDbContext _context;
        private readonly ILogger<DbPremiumService> _logger;
        private readonly UserManager<User> _userManager;

        public DbPremiumService(MaysDbContext context,
                                ILogger<DbPremiumService> logger,
                                UserManager<User> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<bool> MakePremiumAsync(string userId, CreditCard creditCard)
        {
            if (true) // accept any card for the moment
            {
                var user = await GetUser(userId);
                user.ExpirationDate = DateTime.Now.AddMonths(1);
                user.AutoRenew = creditCard.AutoRenew;
                _context.Users.Update(user);

                await _context.SaveChangesAsync();

                await _userManager.AddToRoleAsync(user, "premium");

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task CancelSubscriptionAsync(string userId)
        {
            var user = await GetUser(userId);
            user.AutoRenew = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        private async Task<User> GetUser(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
    }
}
