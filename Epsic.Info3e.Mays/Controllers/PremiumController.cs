using System.Linq;
using System.Threading.Tasks;
using Epsic.Info3e.Mays.Models;
using Epsic.Info3e.Mays.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epsic.Info3e.Mays.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "user,premium,admin")]
    public class PremiumController : ControllerBase
    {
        private readonly IPremiumService _premiumService;

        public PremiumController(IPremiumService premiumService)
        {
            _premiumService = premiumService;
        }

        [HttpPost]
        [Route("Subscribe")]
        public async Task<IActionResult> Subscribe(CreditCard creditCard)
        {
            if (await _premiumService.MakePremiumAsync(GetUserId(), creditCard))
            {
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
            await _premiumService.CancelSubscriptionAsync(GetUserId());
            return Ok();
        }

        private string GetUserId()
        {
            return User.Claims.FirstOrDefault(x => x.Type == "Id").Value;
        }
    }
}
