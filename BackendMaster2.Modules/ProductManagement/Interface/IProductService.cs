using BackendMaster2.Shared.Common;
using BackendMaster2.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.ProductManagement.Interface
{
    // CASOS DE USO del módulo: lo que el mundo exterior puede pedirle.
    public interface IProductService
    {
        Task<Result<IEnumerable<Product>>> GetAllAsync();
        Task<Result<Product>> GetByIdAsync(Guid id);
        Task<Result<Product>> CreateAsync(Product product);
        Task<Result<Product>> UpdateAsync(Product product);
        Task<Result<bool>> DeleteAsync(Guid id);
    }   
}
