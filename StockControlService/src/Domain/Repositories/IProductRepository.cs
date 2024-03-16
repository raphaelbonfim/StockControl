using Domain.Models;

namespace Domain.Repositories
{
    public interface IProductRepository
    {
        Task SaveOrUpdateAsync(Product aggregate, CancellationToken cancellationToken = default);
        Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
