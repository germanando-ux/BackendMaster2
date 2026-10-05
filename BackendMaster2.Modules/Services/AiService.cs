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
        // InvokePromptAsync envía la cadena directamente al modelo registrado en el Kernel (Groq)
        var result = await _kernel.InvokePromptAsync(prompt, cancellationToken: cancellationToken);

        return result.GetValue<string>() ?? string.Empty;
    }
}
