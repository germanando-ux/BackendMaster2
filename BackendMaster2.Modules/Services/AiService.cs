using Azure;
using BackendMaster2.Modules.Dtos;
using BackendMaster2.Modules.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Pgvector;
using System.Net.Http.Json;
using System.Text.Json;

namespace BackendMaster2.Modules.Services;

internal record GeminiEmbeddingResponse(GeminiEmbeddingData? Embedding);
internal record GeminiEmbeddingData(float[]? Values);


public class AiService : IAiService
{
    private readonly IChatClient _chatClient;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AiService(IChatClient chatClient, HttpClient httpClient, IConfiguration configuration)
    {
        _chatClient = chatClient;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    /// <summary>
    /// Método para generar Embeddings a partir de un texto. LLamanado a la API de Google Gemini para generar el embedding y devolverlo como un Vector compatible con pgvector.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Vector?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        // 1. Leemos la configuración
        string apiKey = _configuration["AiSettings:GeminiEmbeddingApiKey"];
        string modelId = _configuration["AiSettings:GeminiEmbeddingModelId"];

        // 2. Construimos la URL nativa de Google Gemini
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelId}:embedContent?key={apiKey}";

        // 3. Creamos el payload exacto que pide la API de Gemini
        var requestBody = new
        {
            model = modelId,
            content = new { parts = new[] { new { text } } },
            outputDimensionality = 1536
        };

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, requestBody, cancellationToken); 
        response.EnsureSuccessStatusCode();

        // 5. Leemos y deserializamos directamente al record tipado
        var result = await response.Content.ReadFromJsonAsync<GeminiEmbeddingResponse>(cancellationToken: cancellationToken);

        // 6. Devolvemos el tipo Vector que espera pgvector
        if (result?.Embedding?.Values == null)
        {
            return null;
        }

        return new Vector(result.Embedding.Values);
    }


    public async Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default)
    {

            ChatResponse result = await _chatClient.GetResponseAsync(prompt, new ChatOptions { Temperature = 0.2f },  cancellationToken: cancellationToken);

            return result.Text ?? string.Empty;
    }


    /// <summary>
    /// Método para generar Embeddings a partir de un texto. LLamanado a la API de Google Gemini para generar el embedding y devolverlo como un Vector compatible con pgvector.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<SearchProductIntentDto> GenerateProductSearchIntent(string text, CancellationToken cancellationToken = default)
    {
        string prompt = $@"Eres un asistente experto en comercio electrónico. Tu tarea es analizar la consulta de un usuario y extraer su intención de búsqueda en formato JSON estricto.

        Reglas de extracción:
        1. ProductQuery (string): El término principal del producto. CONSERVA todos los adjetivos descriptivos, estilo o uso (ej: 'calentito', 'ligero', 'impermeable', 'para correr'). NO los normalices ni los elimines, ya que se usarán para búsqueda semántica por vectores. Ignora categorías o características técnicas específicas, céntrate en la descripción natural.
        2. ColorQuery (string): El color mencionado, si lo hay (ej: 'verde aguamarina', 'burdeos'). Si no hay color, deja este campo vacío.
        3. MinPrice (decimal|null): Precio mínimo si se menciona un rango (ej: 'entre 20 y 50' -> 20). Si no, null.
        4. MaxPrice (decimal|null): Precio máximo si se menciona (ej: 'menos de 50' -> 50). Si no, null.
        5. SizeNames (array de strings): Lista de tallas específicas mencionadas (ej: 'talla 40 y 42' -> ['40', '42']). No intentes inferir rangos, solo extrae las tallas explícitas. Si no se mencionan, deja el array vacío [].
        6. MinStock (int): Si el usuario implica disponibilidad ('en stock', 'disponible'), establece este valor en 1. Si no se menciona, establece 0 (sin filtro de stock).

        Ejemplo 1:
        Texto: 'zapatillas de correr rojas baratas por menos de 60 euros en talla 42'
        JSON: {{
          ""ProductQuery"": ""zapatillas de correr baratas"",
          ""ColorQuery"": ""rojas"",
          ""MinPrice"": null,
          ""MaxPrice"": 60,
          ""SizeNames"": [""42""],
          ""MinStock"": 0
        }}

        Ejemplo 2:
        Texto: 'abrigo de lana calentito disponible'
        JSON: {{
          ""ProductQuery"": ""abrigo de lana calentito"",
          ""ColorQuery"": """",
          ""MinPrice"": null,
          ""MaxPrice"": null,
          ""SizeNames"": [],
          ""MinStock"": 1
        }}

        Texto del usuario: ""{text}""

        Devuelve SOLO el objeto JSON válido, sin markdown, sin comillas externas y sin explicaciones adicionales.";

        ChatResponse result = await _chatClient.GetResponseAsync(prompt, new ChatOptions { Temperature = 0.0f }, cancellationToken: cancellationToken);

        // Blindaje contra markdown residual (```json ... ```)
        string cleanJson = result.Text.Replace("```json", "").Replace("```", "").Trim();

        var intent = JsonSerializer.Deserialize<SearchProductIntentDto>(cleanJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return intent ?? new SearchProductIntentDto();
    }
}
