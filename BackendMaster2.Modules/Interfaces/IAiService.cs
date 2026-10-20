using Pgvector;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Interfaces;

public interface IAiService
{
    Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default);
    // Convierte un texto en un vector
    Task<Vector?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);

}