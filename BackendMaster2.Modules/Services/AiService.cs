using BackendMaster2.Modules.Interfaces;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Services;

public class AiService : IAiService
{
    private readonly Kernel _kernel;

    public AiService(Kernel kernel)
    {
        _kernel = kernel;
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
