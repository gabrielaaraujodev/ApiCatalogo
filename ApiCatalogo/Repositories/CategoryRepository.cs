using ApiCatalogo.Context;
using ApiCatalogo.Models;

namespace ApiCatalogo.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) :base(context)
    {   
    }
}
