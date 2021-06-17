using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Epsic.Info3e.Mays.Config;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Epsic.Info3e.Mays.Services
{
    class DbAuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtConfig _jwtConfig;

        public DbAuthService(UserManager<User> userManager,
                             IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        public async Task<(User result, IEnumerable<string> errors)> RegisterAsync(CreateAccountRequest user)
        {
            var created = new User { Email = user.Email, UserName = user.Name };

            var result = await _userManager.CreateAsync(created, user.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(created, "user");

                return (created, null);
            }

            return (null, result.Errors.Select(x => x.Description));
        }

        public async Task<User> LoginAsync(LoginRequest user)
        {
            User existingUser;
            if (user.Input.Contains('@'))
            {
                existingUser = await _userManager.FindByEmailAsync(user.Input);
            }
            else
            {
                existingUser = await _userManager.FindByNameAsync(user.Input);
            }

            if (existingUser != null)
            {
                var correct = await _userManager.CheckPasswordAsync(existingUser, user.Password);

                if (correct)
                {
                    return existingUser;
                }
            }

            return null;
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // Nous obtenons notre secret à partir des paramètres de l'application.
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            // Nous devons utiliser les claims qui sont des propriétés de notre token et qui donnent des informations sur le token.
            // qui appartiennent à l'utilisateur spécifique à qui il appartient
            // donc il peut contenir son id, son nom, son email. L'avantage est que ces informations
            // sont générées par notre serveur qui est valide et de confiance.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("Username", user.UserName),
                    new Claim("Avatar", user.Avatar == null ? "" : user.Avatar)
                }),
                Claims = new Dictionary<string, object>(),
                Expires = DateTime.UtcNow.AddDays(1),
                // ici, nous ajoutons l'information sur l'algorithme de cryptage qui sera utilisé pour décrypter notre token.
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            foreach (var role in roles)
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));

            foreach (var claim in claims)
                tokenDescriptor.Claims.TryAdd(claim.Type, claim.Value);

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }
    }
}
