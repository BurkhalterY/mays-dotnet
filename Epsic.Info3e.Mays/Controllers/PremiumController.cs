using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Epsic.Info3e.Mays.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Epsic.Info3e.Mays.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PremiumController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public PremiumController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> Enable(string userId = null)
        {
            if (!User.IsInRole("admin") && !User.IsInRole("premium"))
            {
                // Free users can only premium themselves
                userId ??= User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            }

            //TODO Check for credit card

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Any(r => string.Equals(r, "premium", System.StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest();
            }

            await _userManager.AddToRoleAsync(user, "premium");

            return Ok();
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> Disable(string userId = null)
        {
            if (!User.IsInRole("admin"))
            {
                if (User.IsInRole("premium"))
                {
                    userId = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                }
                else
                {
                    return Forbid();
                }
            }

            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Any(r => string.Equals(r, "premium", System.StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest();
            }

            await _userManager.RemoveFromRoleAsync(user, "premium");

            return Ok();
        }
    }
}
