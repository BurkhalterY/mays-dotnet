using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Epsic.Info3e.Mays.Models;
using Epsic.Info3e.Mays.Services;

namespace Epsic.Info3e.Mays.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("Create")]
        /// <summary>
        /// Creates a new user, without logging them in
        /// </summary>
        /// <param name="user">Request with the user's name, password and email</param>
        /// <returns>Either an ok with the created user, or a badrequest with the errors</returns>
        public async Task<IActionResult> Create([FromBody] CreateAccountRequest user)
        {
            var (newUser, errors) = await _authService.RegisterAsync(user);

            if (newUser != null)
            {
                return Ok(newUser);
            }

            return BadRequest(new CreateAccountResponse
            {
                Result = false,
                Message = string.Join(Environment.NewLine, errors)
            });
        }

        [HttpPost]
        [Route("Login")]
        /// <summary>
        /// Logs an user in
        /// </summary>
        /// <param name="user">Request with the user's password, and the user's name or email</param>
        /// <returns>Ok with the jwttoken, or badrequest with a message</returns>
        public async Task<IActionResult> Login([FromBody] LoginRequest user)
        {
            var userFound = await _authService.LoginAsync(user);

            if (userFound != null)
            {
                return Ok(new AuthResponse {
                    Result = true,
                    Token = await _authService.GenerateJwtTokenAsync(userFound)
                });
            }

            // Nous ne voulons pas donner trop d'informations sur la raison de l'échec de la demande pour des raisons de sécurité.
            return BadRequest(new AuthResponse
            {
                Result = false,
                Message = "Invalid authentication request"
            });
        }
    }
}
