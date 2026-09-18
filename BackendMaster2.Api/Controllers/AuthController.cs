using BackendMaster2.Api.Interface;
using BackendMaster2.Api.Models.Dtos;
using BackendMaster2.Api.Services;
using BackendMaster2.Modules.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendMaster2.Api.Controllers
{
    /// <summary>
    /// Endpoints de autenticación: login, refresh y revoke.
    /// Sin una sola línea de base de datos: todo lo pregunta al service.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        
        private readonly IAuthService _AuthService;

        public AuthController(IAuthService authService)
        {
            _AuthService = authService;
        }

        /// <summary>
        /// POST /api/auth/login — comprueba email y contraseña y entrega los dos tokens.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
        {
            // null cubre los tres fallos (no existe, contraseña mal, desactivado):
            // misma respuesta para todos, no regalamos información al atacante.
            var user = await _AuthService.ValidateCredentialsAsync(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized("Credenciales inválidas");
            }

            var (accessToken, expiresAt) = _AuthService.CreateAccessToken(user);
            var refreshToken = await _AuthService.CreateRefreshTokenAsync(user.Id);

            return Ok(new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAtUtc = expiresAt
            });
        }

        /// <summary>
        /// POST /api/auth/refresh — renueva el access token usando el refresh.
        /// El refresh se devuelve tal cual: hoy no hay rotación.
        /// </summary>
        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshRequest request)
        {
            var user = await _AuthService.ValidateRefreshTokenAsync(request.RefreshToken);

            if (user == null)
            {
                return Unauthorized("Refresh token inválido o caducado");
            }

            var (accessToken, expiresAt) = _AuthService.CreateAccessToken(user);

            return Ok(new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = request.RefreshToken,
                AccessTokenExpiresAtUtc = expiresAt
            });
        }

        /// <summary>
        /// POST /api/auth/revoke — logout: mata la sesión. El refresh queda
        /// marcado como revocado y no servirá para renovar nunca más.
        /// Devuelve 200 siempre: logout idempotente, no filtra si el token existía.
        /// </summary>
        [HttpPost("revoke")]
        public async Task<ActionResult> Revoke([FromBody] RefreshRequest request)
        {
            await _AuthService.RevokeRefreshTokenAsync(request.RefreshToken);
            return Ok();
        }
    }
}
