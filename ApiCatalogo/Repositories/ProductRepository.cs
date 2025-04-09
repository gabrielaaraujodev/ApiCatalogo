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

        public PagedList<Product> GetProducts(ProductsParameters productsParams)
        {
            var products = GetAll().OrderBy(P => P.ProductId).AsQueryable();
            var orderProducts = PagedList<Product>.ToPagedList(products, productsParams.PageNumber, productsParams.PageSize);

            return orderProducts;
        }

        public PagedList<Product> GetProductsFilterPrice(ProductsFilterPrice productsFilterPrice)
        {
            var products = GetAll().AsQueryable();

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

            var filterProducts = PagedList<Product>.ToPagedList(products, productsFilterPrice.PageNumber, productsFilterPrice.PageSize);

            return filterProducts;
        }
    }
}
