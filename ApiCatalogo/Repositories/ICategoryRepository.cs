using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using System.Runtime.InteropServices;
using X.PagedList;

namespace ApiCatalogo.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IPagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParams);
    Task<IPagedList<Category>> GetCategoriesFilterNameAsync(CategoriesFilterName categoriesParams);

}
