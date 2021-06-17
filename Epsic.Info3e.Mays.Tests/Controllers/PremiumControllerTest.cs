using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Epsic.Info3e.Mays.Models;
using NUnit.Framework;

namespace Epsic.Info3e.Mays.Tests.Controllers
{
    [TestFixture]
    public class PremiumControllerTest : ApiControllerTestBase
    {
        public static object[] SubscribeSuccessCases =
        {
            new object[] {"0000000000000000", "000"},
        };

        [TestCaseSource(nameof(SubscribeSuccessCases))]
        public async Task SubscribeSuccess(string cardNumber, string securityCode)
        {
            var token = await PostAsync<LoginRequest, AuthResponse>("api/Auth/Login", new LoginRequest {
                Input = "pierre",
                Password = "P1erre++",
            });

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

            var result = await PostAsync<CreditCard, PaymentResponse>("api/Premium/Subscribe", new CreditCard {
                CardNumber = cardNumber,
                SecurityCode = securityCode,
                Mount = 1,
                Year = 0,
                Holder = "pierre",
                AutoRenew = false,
            });

            // Thankfully, login doesn't check if the user is already logged in
            token = await PostAsync<LoginRequest, AuthResponse>("api/Auth/Login", new LoginRequest {
                Input = "pierre",
                Password = "P1erre++",
            });

            var stream = token.Token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;

            Assert.True(result.Result, result.Message);
            Assert.True(tokenS.Claims.Any(c => c.Type == "role" && c.Value.ToLower() == "premium"), "User doesn't have premium role");
        }
    }
}
