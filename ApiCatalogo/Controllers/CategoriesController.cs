using ApiCatalogo.DTOs;
using ApiCatalogo.DTOs.ManualMapping;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiCatalogo.Controllers;

[EnableCors("PoliticaCORS1")]
[Route("[controller]")]
[ApiController]
// [EnableRateLimiting("fixedWindow")]
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
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get([FromQuery] CategoriesParameters categoriesParameters)
    {
        var categories = await _uof.CategoryRepository.GetCategoriesAsync(categoriesParameters);

        var metadata = new
        {
            categories.Count,
            categories.PageSize,
            categories.PageCount,
            categories.TotalItemCount,
            categories.HasNextPage,
            categories.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var categoriesDTO = categories.ToCategoryDtoList();

        return Ok(categoriesDTO);
    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesFilterName([FromQuery] CategoriesFilterName categoriesFilterParameters)
    {
        var categories = await _uof.CategoryRepository.GetCategoriesFilterNameAsync(categoriesFilterParameters);

        var metadata = new
        {
            categories.Count,
            categories.PageSize,
            categories.PageCount,
            categories.TotalItemCount,
            categories.HasNextPage,
            categories.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var categoriesDTO = categories.ToCategoryDtoList();

        return Ok(categoriesDTO);
    }

    /// <summary>
    /// Obtem uma lista de objetos Categoria
    /// </summary>
    /// <returns>Uma lista de objetos Categoria</returns>

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get()
    {
        var categories = await _uof.CategoryRepository.GetAllAsync();

        if (categories is null) 
            return NotFound("Não existem categorias ...");

        var categoriesDTO =  categories.ToCategoryDtoList();

        return Ok(categoriesDTO);
    }

    /// <summary>
    /// Obtem uma Categoria pelo seu Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Objetos Categoria</returns>

    [EnableCors("PoliticaCORS1")]
    [HttpGet("{id:int}", Name ="ObterCategoria")]
    public async Task<ActionResult<CategoryDTO>> Get(int id)
    {    
        var category = await _uof.CategoryRepository.GetAsync(c => c.CategoryId == id);

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

    /// <summary>
    /// Inclui uma nova Categoria
    /// </summary>
    /// <remarks>
    /// Exemplo de request:
    ///
    ///     POST api/categories
    ///     {
    ///         "categodyId": 1,
    ///         "nome": "category1",
    ///         "imageURL": "http://teste.net/1.jpg
    ///     }
    /// </remarks>
    /// <param name="categoryDTO">objeto Categoria</param>
    /// <returns>O objeto Categoria incluida</returns>
    /// <remarks>Retorna um objeto Categoria incluído</remarks>
    [HttpPost]
    public async Task<ActionResult<CategoryDTO>> Post(CategoryDTO categoryDTO)
    { 
        if (categoryDTO is null)
        {
            _logger.LogWarning("$Dados Inválidos.");
            return BadRequest("$Dados Inválidos.");
        }

        var categoryDtoToNormal = categoryDTO.ToCategory();

        var createdCategory = _uof.CategoryRepository.Create(categoryDtoToNormal);
        await _uof.CommitAsync();

        var categoryMapperDTO = createdCategory.ToCategoryDTO();
        
        return new CreatedAtRouteResult("ObterCategoria", new { id = categoryMapperDTO.CategoryId }, categoryMapperDTO);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDTO>> Put(int id, CategoryDTO categoryDTO)
    {
        if (id != categoryDTO.CategoryId)
        {
            _logger.LogWarning("Dados Inválidos.");
            return BadRequest("Dados Inválidos.");
        }

        var categoryDtoToNormal = categoryDTO.ToCategory();

        var UpdatedCategory = _uof.CategoryRepository.Update(categoryDtoToNormal);
        await _uof.CommitAsync();

        var categoryMapperDTO = UpdatedCategory.ToCategoryDTO();

        return Ok(categoryMapperDTO); 
    }

    [EnableCors("PoliticaCORS2")]
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CategoryDTO>> Delete(int id)
    {
        var category = await _uof.CategoryRepository.GetAsync(c => c.CategoryId == id);

        if (category is null)
        {
            _logger.LogWarning($"Categoria de id = {id} não encontrada.");
            return NotFound($"Categoria de id = {id} não encontrada.");
        }

        var deletedCategory = _uof.CategoryRepository.Delete(category);
        await _uof.CommitAsync();

        var categoryMapperDTO = deletedCategory.ToCategoryDTO();

        return Ok(categoryMapperDTO);      
    }
}
