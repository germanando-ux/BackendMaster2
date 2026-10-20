using BackendMaster2.Modules.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Pgvector;

namespace BackendMaster2.Modules.Services;

public class AiService : IAiService
{
    private readonly Kernel _kernel;    
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public AiService(Kernel kernel, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
    {
        _kernel = kernel;
        _embeddingGenerator = embeddingGenerator;   
    }

    public async Task<Vector?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        // 1. Pedimos a Semantic Kernel que genere el embedding
        var embeddings = await _embeddingGenerator.GenerateAsync([text], cancellationToken: cancellationToken);
        // 2. Convertimos el resultado en el tipo Vector de pgvector
        var embedding = embeddings[0];
        var vector = new Vector(embedding.Vector.ToArray());
        return vector;

    }

    public async Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default)
    {
        try
        {
            Console.WriteLine(">>> ANTES de InvokePromptAsync");

            var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);

            Console.WriteLine(">>> DESPUÉS de InvokePromptAsync");
            return result.GetValue<string>() ?? string.Empty;
        }
        catch (Exception ex)
        {
            // ESTO ES CRÍTICO: nos dirá si hay un error de red, clave o configuración
            Console.WriteLine($">>> ERROR EN SEMANTIC KERNEL: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($">>> INNER EXCEPTION: {ex.InnerException.Message}");
            }
            throw; // Relanzamos para que el flujo falle visiblemente
        }
    }


}
