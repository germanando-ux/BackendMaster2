using Pgvector;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared.Extensions;

public static class VectorExtensions
{
    /// <summary>
    /// Calcula la similitud del coseno entre dos vectores en memoria (C# puro).
    /// Devuelve un valor entre 0.0 (opuestos) y 1.0 (idénticos).
    /// </summary>
    public static double CalculateCosineSimilarity(this Vector? v1, Vector? v2)
    {

        if (v1 == null || v2 == null) return 0.0;

        // Convertimos a ReadOnlySpan para acceso eficiente
        ReadOnlySpan<float> span1 = v1.ToArray().AsSpan();
        ReadOnlySpan<float> span2 = v2.ToArray().AsSpan();

        double dotProduct = 0.0;
        double norm1 = 0.0;
        double norm2 = 0.0;

        for (int i = 0; i < span1.Length; i++)
        {
            float val1 = span1[i];
            float val2 = span2[i];

            dotProduct += val1 * val2;
            norm1 += val1 * val1;
            norm2 += val2 * val2;
        }

        norm1 = Math.Sqrt(norm1);
        norm2 = Math.Sqrt(norm2);

        // Evitar división por cero
        if (norm1 == 0 || norm2 == 0) return 0.0;

        double result = dotProduct / (norm1 * norm2);
        return result;  
    }
}
