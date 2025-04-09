using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository (AppDbContext context) : base (context)
        {
        }

        public IEnumerable<Product> GetProdructsByCategory(int id)
        {
            return GetAll().Where(c => c.CategoryId == id);
        }

        //public IEnumerable<Product> GetProdructs(ProductsParameters productsParams)
        //{
        //    return GetAll()
        //        .OrderBy(p => p.Name)
        //        .Skip((productsParams.PageNumber - 1) * productsParams.PageSize)
        //        .Take(productsParams.PageSize).ToList();
        //}

        public PagedList<Product> GetCategories(ProductsParameters productsParams)
        {
            var products = GetAll().OrderBy(P => P.ProductId).AsQueryable();
            var orderProducts = PagedList<Product>.ToPagedList(products, productsParams.PageNumber, productsParams.PageSize);

            return orderProducts;
        }
    }
}
