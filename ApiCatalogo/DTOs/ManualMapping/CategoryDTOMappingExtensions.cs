using ApiCatalogo.Models;

namespace ApiCatalogo.DTOs.ManualMapping
{
    public static class CategoryDTOMappingExtensions
    {
        public static CategoryDTO? ToCategoryDTO(this Category category)
        {
            if (category is null)
                return null;

            return new CategoryDTO
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ImageUrl = category.ImageUrl,
            };
        }

        public static Category? ToCategory(this CategoryDTO CategoryDTO)
        {
            if (CategoryDTO is null)
                return null;

            return new Category
            {
                CategoryId = CategoryDTO.CategoryId,
                Name = CategoryDTO.Name,
                ImageUrl = CategoryDTO.ImageUrl,
            };
        }

        public static IEnumerable<CategoryDTO> ToCategoryDtoList(this IEnumerable<Category> categoriesList)
        {
            if (categoriesList is null || !categoriesList.Any())
                return new List<CategoryDTO>();

            return categoriesList.Select(categoriesList => new CategoryDTO
            {
                CategoryId = categoriesList.CategoryId,
                Name = categoriesList.Name,
                ImageUrl = categoriesList.ImageUrl,
            });
        }
    }
}
