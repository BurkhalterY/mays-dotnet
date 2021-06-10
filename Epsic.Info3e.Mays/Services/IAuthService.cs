using System.Collections.Generic;
using System.Threading.Tasks;
using Epsic.Info3e.Mays.Models;

namespace Epsic.Info3e.Mays.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">Request with the informations of the new user</param>
        /// <returns>An instance of User on success, a list of errors on failure, null for the other on either</returns>
        public Task<(User result, IEnumerable<string> errors)> RegisterAsync(CreateAccountRequest user);

        /// <summary>
        /// Logs an user in
        /// </summary>
        /// <param name="user">Request with the login informations</param>
        /// <returns>The User on success, null on failure</returns>
        public Task<User> LoginAsync(LoginRequest user);

        /// <summary>
        /// Generates a jwttoken for an user
        /// </summary>
        /// <param name="user">User to generate the token for</param>
        /// <returns>The token generated</returns>
        public Task<string> GenerateJwtTokenAsync(User user);
    }
}
