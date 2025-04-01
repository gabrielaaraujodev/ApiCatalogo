using ApiCatalogo.Models;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Controllers;
[Route("[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IRepository<Category> _repository;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(IRepository<Category> repository, ILogger<CategoriesController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Category>> Get()
    {
        var category = _repository.GetAll();

        return Ok(category);
    }

    [HttpGet("{id:int}", Name ="ObterCategoria")]
    public ActionResult<Category> Get(int id)
    {    
        var category = _repository.Get(c => c.CategoryId == id);

        if (category is null)
        {
            _logger.LogWarning($"Categoria com id = {id} não encontrada.");
            return NotFound($"Categoria com id = {id} não encontrada.");
        }

        return Ok(category);
    }

    /*[HttpGet("products")]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesProductsAsync()
    {
        return await _context.Categories.AsNoTracking().Include(p => p.Products).ToListAsync();
    }*/

    [HttpPost]
    public ActionResult Post(Category category)
    { 
        if (category is null)
        {
            _logger.LogWarning("$Dados Inválidos.");
            return BadRequest("$Dados Inválidos.");
        }

        var createdCategory = _repository.Create(category);

        return new CreatedAtRouteResult("ObterCategoria", new { id = createdCategory.CategoryId }, createdCategory);
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Category category)
    {
        if (id != category.CategoryId)
        {
            _logger.LogWarning("Dados Inválidos.");
            return BadRequest("Dados Inválidos.");
        }

        _repository.Update(category);

        return Ok(category); 
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var category = _repository.Get(c => c.CategoryId == id);

        if (category is null)
        {
            _logger.LogWarning($"Categoria de id = {id} não encontrada.");
            return NotFound($"Categoria de id = {id} não encontrada.");
        }

        var deletedCategory = _repository.Delete(category);

        return Ok(deletedCategory);      
    }
}
