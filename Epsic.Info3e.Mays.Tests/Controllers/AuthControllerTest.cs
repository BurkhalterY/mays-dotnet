using System.Threading.Tasks;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;

namespace Epsic.Info3e.Mays.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTest : ApiControllerTestBase
    {

        public static object[] CreateSuccessCases =
        {
            new object[] {"jeff", "jeff@hotmail.com", "JeffTheB3st!"},
            new object[] {"joe", "joe@hotmail.com", "J0eM4M4+"},
            new object[] {"jim", "jim@hotmail.com", "jim'sBetter7hanJeff"},
        };

        [TestCaseSource(nameof(CreateSuccessCases))]
        public async Task CreateSuccess(string username, string email, string password)
        {
            var result = await PostAsync<CreateAccountRequest, IdentityUser>("api/Auth/Create", new CreateAccountRequest {
                Email = email,
                Name = username,
                Password = password,
            });

            Assert.AreEqual(username, result.UserName);
            Assert.AreEqual(email, result.Email);
        }

        public static object[] CreateFailureCases =
        {
            // Already used users
            new object[] {"pierre", "pierre@hotmail.com", "¡P1erre!"},
            // Password too short
            new object[] {"evil joe", "joe@evil.com", "Le8est+"},
            // Password not complex enough
            new object[] {"1Diot", "1umb@dmu1.com", "L3M3ill3ur"},
            new object[] {"i2iot", "d2mb@dm2b.com", "LeMe!lleur"},
            new object[] {"iD3ot", "du3b@d3ub.com", "L3m3!lleur"},
            new object[] {"iDi4t", "dum4@4mub.com", "L3M3!LL3UR"},
            // Already used email
            new object[] {"jeff", "pierre@hotmail.com", "¡P1erre!"},
            // Email for username
            new object[] {"joe@mama.com", "joe@mama.com", "¡P1erre!"},
        };

        [TestCaseSource(nameof(CreateFailureCases))]
        public async Task CreateFailure(string username, string email, string password)
        {
            var result = await PostAsync<CreateAccountRequest, CreateAccountResponse>("api/Auth/Create", new CreateAccountRequest {
                Email = email,
                Name = username,
                Password = password,
            });

            Assert.IsFalse(result.Result);
        }

        public static object[] LoginSuccessCases =
        {
            // Login with usernames
            new object[] { "pierre", "P1erre++" },
            new object[] { "paul", "P4ul+-+-" },
            new object[] { "jacques", "Jacqu3s-" },
            // Login with emails
            new object[] { "pierre@hotmail.com", "P1erre++" },
            new object[] { "paul@gmail.com", "P4ul+-+-" },
            new object[] { "jacques@bluwin.com", "Jacqu3s-" },
        };

        [TestCaseSource(nameof(LoginSuccessCases))]
        public async Task LoginSuccess(string input, string password)
        {
            var result = await PostAsync<LoginRequest, AuthResponse>("api/Auth/Login", new LoginRequest {
                Input = input,
                Password = password,
            });

            Assert.IsTrue(result.Result);
        }

        public static object[] LoginFailureCases =
        {
            // Login with nonexistant user
            new object[] { "joe", "Joe1+23*" },
            new object[] { "joe@mama.com", "Joe1+23*" },
            // Login with wrong password
            new object[] { "paul", "P1erre++" },
            new object[] { "paul@gmail.com", "P1erre++" },
        };

        [TestCaseSource(nameof(LoginFailureCases))]
        public async Task LoginFailure(string input, string password)
        {
            var result = await PostAsync<LoginRequest, AuthResponse>("api/Auth/Login", new LoginRequest {
                Input = input,
                Password = password,
            });

            Assert.IsFalse(result.Result);
        }
    }
}
