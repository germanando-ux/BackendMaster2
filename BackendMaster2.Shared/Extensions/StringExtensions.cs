using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared.Extensions;

public static class StringExtensions
{
    // <summary>
    /// Trunca el texto a la longitud máxima indicada y añade "..." si es necesario.
    /// </summary>
    public static string Truncate(this string texto, int longitudMaxima)
    {
        // Si el texto es nulo o vacío, lo devolvemos tal cual
        if (string.IsNullOrEmpty(texto))
        {
            return texto;
        }

        // Si el texto ya es más corto que el límite, no hace nada
        if (texto.Length <= longitudMaxima)
        {
            return texto;
        }

        // Si es más largo, lo cortamos y añadimos "..."
        return texto.Substring(0, longitudMaxima) + "...";
    }
}
