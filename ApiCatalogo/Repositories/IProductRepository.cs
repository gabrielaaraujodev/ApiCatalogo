using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetProdructsByCategory(int id);
        PagedList<Product> GetProducts(ProductsParameters productsParams);
        PagedList<Product> GetProductsFilterPrice(ProductsFilterPrice productsFilterPrice);

    }
}
