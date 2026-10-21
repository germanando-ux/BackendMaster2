using Azure;
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

            ChatResponse result = await _chatClient.GetResponseAsync(prompt, cancellationToken: cancellationToken);

            return result.Text ?? string.Empty;
    }


}
