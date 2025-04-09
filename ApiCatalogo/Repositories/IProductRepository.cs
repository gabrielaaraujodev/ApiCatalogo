using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetProdructsByCategory(int id);
        PagedList<Product> GetCategories(ProductsParameters productsParams);
    }
}
