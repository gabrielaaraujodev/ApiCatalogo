using ApiCatalogo.Context;
using ApiCatalogo.Models;

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
    }
}
