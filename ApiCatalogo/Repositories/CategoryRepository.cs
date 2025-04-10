using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using X.PagedList;

namespace ApiCatalogo.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) :base(context)
    {}

    public async Task<IPagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParams)
    {
        var categories = await GetAllAsync();
        var orderCategories = categories.OrderBy(c => c.CategoryId).AsQueryable();
        //var result = PagedList<Category>.ToPagedList(orderCategories, categoriesParams.PageNumber, categoriesParams.PageSize);
        var result = await orderCategories.ToPagedListAsync(categoriesParams.PageNumber, categoriesParams.PageSize);

        return result;
    }

    public async Task<IPagedList<Category>> GetCategoriesFilterNameAsync(CategoriesFilterName categoriesParams)
    {
        var categories = await GetAllAsync();

        if(!string.IsNullOrEmpty(categoriesParams.Name))
            categories = categories.Where(c => c.Name.Contains(categoriesParams.Name));

        //var categoriesFilter = PagedList<Category>.ToPagedList(categories.AsQueryable(), categoriesParams.PageSize, categoriesParams.PageNumber);

        // Com pacote externo:
        var categoriesFilter = await categories.ToPagedListAsync(categoriesParams.PageNumber, categoriesParams.PageSize);

        return categoriesFilter;
    }
}
