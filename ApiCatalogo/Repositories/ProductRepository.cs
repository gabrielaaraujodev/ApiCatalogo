using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using X.PagedList;

namespace ApiCatalogo.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository (AppDbContext context) : base (context)
        {
        }

        public async Task<IEnumerable<Product>> GetProdructsByCategoryAsync(int id)
        {
            var products = await GetAllAsync();
            var categoriesProducts = products.Where(p => p.CategoryId == id);
            return categoriesProducts;
        }

        //public IEnumerable<Product> GetProdructs(ProductsParameters productsParams)
        //{
        //    return GetAll()
        //        .OrderBy(p => p.Name)
        //        .Skip((productsParams.PageNumber - 1) * productsParams.PageSize)
        //        .Take(productsParams.PageSize).ToList();
        //}

        public async Task<IPagedList<Product>> GetProductsAsync(ProductsParameters productsParams)
        {
            var products = await GetAllAsync();
            var orderProducts = products.OrderBy(P => P.ProductId).AsQueryable();
            var result = await orderProducts.ToPagedListAsync(productsParams.PageNumber, productsParams.PageSize);

            return result;
        }

        public async Task<IPagedList<Product>> GetProductsFilterPriceAsync(ProductsFilterPrice productsFilterPrice)
        {
            var products = await GetAllAsync();

            if (productsFilterPrice.Price.HasValue && !string.IsNullOrEmpty(productsFilterPrice.PriceStandart))
            {
                if (productsFilterPrice.PriceStandart.Equals("maior", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Price > productsFilterPrice.Price.Value).OrderBy(p => p.Price);
                } else if (productsFilterPrice.PriceStandart.Equals("menor", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Price < productsFilterPrice.Price.Value).OrderBy(p => p.Price);
                } else if (productsFilterPrice.PriceStandart.Equals("igual", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Price == productsFilterPrice.Price.Value).OrderBy(p => p.Price);

                }
            }

            var filterProducts = await products.ToPagedListAsync(productsFilterPrice.PageNumber, productsFilterPrice.PageSize);

            return filterProducts;
        }
    }
}
