namespace BackendMaster2.Modules.Interfaces;

/// <summary>
/// Abstracción del servicio de caché distribuida (Redis).
/// Centraliza la serialización/deserialización JSON y la invalidación,
/// para que los servicios de negocio no dupliquen esa lógica.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Lee y deserializa un valor de la caché. Devuelve null si la clave no existe (cache miss).
    /// </summary>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Serializa y guarda un valor en caché con un TTL (por defecto 60 minutos).
    /// </summary>
    Task SetAsync<T>(string key, T data, TimeSpan? expiration = null);

    /// <summary>
    /// Invalida (borra) una o varias claves de caché.
    /// Ej: await RemoveAsync("maestros:colors:actives", "maestros:colors:all");
    /// </summary>
    Task RemoveAsync(params string[] keys);
}