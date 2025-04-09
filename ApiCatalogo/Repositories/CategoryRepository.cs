using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) :base(context)
    {}

    public PagedList<Category> GetCategories(CategoriesParameters categoriesParams)
    {
        var categories = GetAll().OrderBy(c => c.CategoryId).AsQueryable();
        var ordercategories = PagedList<Category>.ToPagedList(categories, categoriesParams.PageNumber, categoriesParams.PageSize);

        return ordercategories;
    }
}
