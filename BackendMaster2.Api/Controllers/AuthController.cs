using BackendMaster2.Api.Models.Dtos;
using BackendMaster2.Api.Services;
using BackendMaster2.Modules.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendMaster2.Api.Controllers
{
    public class AuthController : Controller
    {
        
        private readonly TokenService _tokenService;

        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// POST /api/auth/login — comprueba email y contraseña y entrega los dos tokens.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
        {
            // null cubre los tres fallos (no existe, contraseña mal, desactivado):
            // misma respuesta para todos, no regalamos información al atacante.
            var user = await _tokenService.ValidateCredentialsAsync(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized("Credenciales inválidas");
            }

            var (accessToken, expiresAt) = _tokenService.CreateAccessToken(user);
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user.Id);

            return Ok(new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAtUtc = expiresAt
            });
        }
    }
}
