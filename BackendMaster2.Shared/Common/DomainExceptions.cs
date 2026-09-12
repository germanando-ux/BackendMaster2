using System;
using System.Collections.Generic;
using System.Text;


///Errores de negocio: se lanzan desde el dominio y el middleware los traduce a HTTP.
namespace BackendMaster2.Shared.Common
{
    /// <summary>
    /// Base de todos los errores de negocio. El middleware los traduce a HTTP.
    /// </summary>
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// El recurso pedido no existe. El middleware la traduce a 404.
    /// </summary>
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Se intenta crear un recurso que ya existe. El middleware la traduce a 409.
    /// </summary>
    public class DuplicateResourceException : DomainException
    {
        public DuplicateResourceException(string message) : base(message)
        {
        }
    }
}
