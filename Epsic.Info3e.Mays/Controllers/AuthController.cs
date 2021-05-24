using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Epsic.Info3e.Mays.Config;
using Epsic.Info3e.Mays.Models;

namespace Epsic.Info3e.Mays.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthController(UserManager<User> userManager, IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateAccountRequest user)
        {
            var newUser = new User { Email = user.Email, UserName = user.Name };

            var isCreated = await _userManager.CreateAsync(newUser, user.Password);

            if (isCreated.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "user");
                return Ok(newUser);
            }

            return BadRequest(new CreateAccountResponse
            {
                Result = false,
                Message = string.Join(Environment.NewLine, isCreated.Errors.Select(x => x.Description).ToList())
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest user)
        {
            User existingUser;
            if (user.Input.Contains('@'))
            {
                // Vérifier si l'utilisateur avec le même email existe
                existingUser = await _userManager.FindByEmailAsync(user.Input);
            } else {
                // User entered their name instead of their email
                existingUser = await _userManager.FindByNameAsync(user.Input);
            }
            if (existingUser != null)
            {
                // Maintenant, nous devons vérifier si l'utilisateur a entré le bon mot de passe.
                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

                if (isCorrect)
                {
                    var roles = await _userManager.GetRolesAsync(existingUser);
                    var claims = await _userManager.GetClaimsAsync(existingUser);

                    var jwtToken = GenerateJwtToken(existingUser, roles, claims);

                    return Ok(new AuthResponse
                    {
                        Result = true,
                        Token = jwtToken
                    });
                }
            }

            // Nous ne voulons pas donner trop d'informations sur la raison de l'échec de la demande pour des raisons de sécurité.
            return BadRequest(new AuthResponse
            {
                Result = false,
                Message = "Invalid authentication request"
            });
        }

        private string GenerateJwtToken(User user, IList<string> roles, IList<Claim> claims)
        {
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
