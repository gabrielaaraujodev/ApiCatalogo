using ApiCatalogo.Models;

namespace ApiCatalogo.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetProdructsByCategory(int id);
    }
}
