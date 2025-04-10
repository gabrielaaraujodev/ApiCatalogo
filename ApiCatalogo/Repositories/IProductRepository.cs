using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using X.PagedList;

namespace ApiCatalogo.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProdructsByCategoryAsync(int id);
        Task<IPagedList<Product>> GetProductsAsync(ProductsParameters productsParams);
        Task<IPagedList<Product>> GetProductsFilterPriceAsync(ProductsFilterPrice productsFilterPrice);

    }
}
