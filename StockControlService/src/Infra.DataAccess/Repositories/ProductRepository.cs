using Domain.Models;
using Domain.Repositories;

namespace Infra.DataAccess.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IUnitOfWorkDomain unitOfWork) : base(unitOfWork)
        {
        }
    }
}
