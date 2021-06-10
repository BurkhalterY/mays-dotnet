using System;
using System.Linq;
using System.Threading.Tasks;
using Epsic.Info3e.Mays.DbContext;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Epsic.Info3e.Mays.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "user,premium,admin")]
    public class PremiumController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly MaysDbContext _context;

        public PremiumController(UserManager<User> userManager, MaysDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("Subscribe")]
        public async Task<IActionResult> Subscribe(CreditCard creditCard)
        {
            if (true) // accept any card for the moment
            {
                var user = await GetUser();
                user.ExpirationDate = DateTime.Now.AddMonths(1);
                user.AutoRenew = creditCard.AutoRenew;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                await _userManager.AddToRoleAsync(user, "premium");

                return Ok(new PaymentResponse
                {
                    Result = true
                });
            }
            else
            {
                return BadRequest(new PaymentResponse
                {
                    Result = false,
                    Message = "Invalid card"
                });
            }
        }

        [HttpDelete]
        [Route("CancelSubscription")]
        public async Task<IActionResult> CancelSubscription()
        {
            var user = await GetUser();
            user.AutoRenew = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private async Task<User> GetUser()
        {
            return await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(x => x.Type == "Id").Value);
        }
    }
}
