using BackendMaster2.Modules.Dtos;
using Pgvector;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Interfaces;

public interface IAiService
{
    //Enriquece la descripción de un producto a partir de un prompt y devuelve el texto enriquecido
    Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default);
    /// <summary>
    /// Método para generar Embeddings a partir de un texto. LLamanado a la API de Google Gemini para generar el embedding y devolverlo como un Vector compatible con pgvector.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Vector?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    Task<SearchProductIntentDto> GenerateProductSearchIntent(string text, CancellationToken cancellationToken = default);

}