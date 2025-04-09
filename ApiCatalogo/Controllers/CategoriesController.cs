using ApiCatalogo.DTOs;
using ApiCatalogo.DTOs.ManualMapping;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiCatalogo.Controllers;
[Route("[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private IUnitOfWork _uof;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ILogger<CategoriesController> logger, IUnitOfWork uof)
    {
        _logger = logger;
        _uof = uof;
    }

    [HttpGet("pagination")]
    public ActionResult<IEnumerable<CategoryDTO>> Get([FromQuery] CategoriesParameters categoriesParameters)
    {
        var categories = _uof.CategoryRepository.GetCategories(categoriesParameters);

        var metadata = new
        {
            categories.TotalCount,
            categories.PageSize,
            categories.CurrentPage,
            categories.TotalPages,
            categories.HasNext,
            categories.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var categoriesDTO = categories.ToCategoryDtoList();

        return Ok(categoriesDTO);
    }

    public ActionResult<IEnumerable<CategoryDTO>> GetCategoriesFilterName([FromQuery] CategoriesFilterName categoriesFilterParameters)
    {
        var categories = _uof.CategoryRepository.GetCategoriesFilterName(categoriesFilterParameters);

        var metadata = new
        {
            categories.TotalCount,
            categories.PageSize,
            categories.CurrentPage,
            categories.TotalPages,
            categories.HasNext,
            categories.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var categoriesDTO = categories.ToCategoryDtoList();

        return Ok(categoriesDTO);
    }

    [HttpGet]
    public ActionResult<IEnumerable<CategoryDTO>> Get()
    {
        var categories = _uof.CategoryRepository.GetAll();

        if (categories is null) 
            return NotFound("Não existem categorias ...");

        var categoriesDTO = categories.ToCategoryDtoList();

        return Ok(categoriesDTO);
    }

    [HttpGet("{id:int}", Name ="ObterCategoria")]
    public ActionResult<CategoryDTO> Get(int id)
    {    
        var category = _uof.CategoryRepository.Get(c => c.CategoryId == id);

        if (category is null)
        {
            _logger.LogWarning($"Categoria com id = {id} não encontrada.");
            return NotFound($"Categoria com id = {id} não encontrada.");
        }

        var categoryDTO = category.ToCategoryDTO();

        return Ok(category);
    }

    /*[HttpGet("products")]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesProductsAsync()
    {
        return await _context.Categories.AsNoTracking().Include(p => p.Products).ToListAsync();
    }*/

    [HttpPost]
    public ActionResult<CategoryDTO> Post(CategoryDTO categoryDTO)
    { 
        if (categoryDTO is null)
        {
            _logger.LogWarning("$Dados Inválidos.");
            return BadRequest("$Dados Inválidos.");
        }

        var categoryDtoToNormal = categoryDTO.ToCategory();

        var createdCategory = _uof.CategoryRepository.Create(categoryDtoToNormal);
        _uof.Commit();

        var categoryMapperDTO = createdCategory.ToCategoryDTO();
        
        return new CreatedAtRouteResult("ObterCategoria", new { id = categoryMapperDTO.CategoryId }, categoryMapperDTO);
    }

    [HttpPut("{id:int}")]
    public ActionResult<CategoryDTO> Put(int id, CategoryDTO categoryDTO)
    {
        if (id != categoryDTO.CategoryId)
        {
            _logger.LogWarning("Dados Inválidos.");
            return BadRequest("Dados Inválidos.");
        }

        var categoryDtoToNormal = categoryDTO.ToCategory();

        var UpdatedCategory = _uof.CategoryRepository.Update(categoryDtoToNormal);
        _uof.Commit();

        var categoryMapperDTO = UpdatedCategory.ToCategoryDTO();

        return Ok(categoryMapperDTO); 
    }

    [HttpDelete("{id:int}")]
    public ActionResult<CategoryDTO> Delete(int id)
    {
        var category = _uof.CategoryRepository.Get(c => c.CategoryId == id);

        if (category is null)
        {
            _logger.LogWarning($"Categoria de id = {id} não encontrada.");
            return NotFound($"Categoria de id = {id} não encontrada.");
        }

        var deletedCategory = _uof.CategoryRepository.Delete(category);
        _uof.Commit();

        var categoryMapperDTO = deletedCategory.ToCategoryDTO();

        return Ok(categoryMapperDTO);      
    }
}
