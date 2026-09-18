namespace BackendMaster2.Api.Models.Dtos;



/// <summary>
/// Lo que el front manda al hacer login.
/// Solo email y contraseña: si viniera algo más en el JSON, se ignora.
/// </summary>
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Lo que la API devuelve tras un login o un refresh correctos.
/// Los dos tokens y hasta cuándo vive el access.
/// </summary>
public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAtUtc { get; set; }
}

/// <summary>
/// El sobre de refresh y revoke: solo viaja el refresh token.
/// </summary>
public class RefreshRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
