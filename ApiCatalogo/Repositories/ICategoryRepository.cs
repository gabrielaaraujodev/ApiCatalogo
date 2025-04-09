using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using System.Runtime.InteropServices;

namespace ApiCatalogo.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    PagedList<Category> GetCategories(CategoriesParameters categoriesParams);
    PagedList<Category> GetCategoriesFilterName(CategoriesFilterName categoriesParams);

}
