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
    /// Método para generar Embeddings a partir de un texto
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
    /// Traducimos a un json la consulta del usuario por medio de un chat de ia
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<SearchProductIntentDto> GenerateProductSearchIntent(string text, CancellationToken cancellationToken = default)
    {
        //string prompt = $@"Analiza el siguiente texto y determina la intención de búsqueda de producto. 
        //                Devuelve un JSON con las propiedades: ProductQuery (string), ColorQuery (string), MaxPrice (decimal), MinStock (int). Texto: {text}.
        //                Devuelve SOLO el objeto JSON válido, sin texto adicional, sin explicaciones y sin etiquetas markdown";

        string prompt = $@"Analiza el siguiente texto de búsqueda de un usuario en una tienda online.
                        Devuelve un JSON con las propiedades: 
                        - ProductQuery (string): El término principal del producto. CONSERVA adjetivos descriptivos como 'calentito', 'ligero', 'impermeable', etc. NO los normalices ni los elimines.
                        - ColorQuery (string): El color mencionado, si lo hay.
                        - MaxPrice (decimal|null): Precio máximo si se menciona.
                        - MinStock (int|null): Stock mínimo si se menciona.

                        Texto del usuario: ""{text}""

                        Devuelve SOLO el objeto JSON válido, sin markdown, sin explicaciones.";

        ChatResponse result = await _chatClient.GetResponseAsync(prompt, new ChatOptions { Temperature = 0.0f }, cancellationToken: cancellationToken);

        // Blindaje contra markdown residual (```json ... ```)
        string cleanJson = result.Text.Replace("```json", "").Replace("```", "").Trim();

        var intent = JsonSerializer.Deserialize<SearchProductIntentDto>(cleanJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return intent ?? new SearchProductIntentDto();
    }
}
