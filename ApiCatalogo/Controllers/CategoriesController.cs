using ApiCatalogo.Context;
using ApiCatalogo.Filter;
using ApiCatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Controllers;
[Route("[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(AppDbContext context, ILogger<CategoriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAsync()
    {
            return await _context.Categories.AsNoTracking().ToListAsync();
    }

    [HttpGet("{id:int}", Name ="ObterCategoria")]
    public async Task<ActionResult<Category>> GetAsync(int id)
    {    
        var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(p => p.CategoryId == id);

        if (category == null)
        {
            _logger.LogWarning($"Categoria com id = {id} não encontrada.");
            return NotFound($"Categoria com id = {id} não encontrada.");
        }

        return Ok(category);
    }

    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesProductsAsync()
    {

        return await _context.Categories.AsNoTracking().Include(p => p.Products).ToListAsync();
    }

    [HttpPost]
    public ActionResult Post(Category category)
    {
 
        if (category is null)
        {
            _logger.LogWarning("$Dados Inválidos.");
            return BadRequest("$Dados Inválidos.");
        }

        _context.Categories.Add(category);
        _context.SaveChanges();

        return new CreatedAtRouteResult("ObterCategoria", new { id = category.CategoryId }, category);
    
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Category category)
    {
        if (id != category.CategoryId)
        {
            _logger.LogWarning("Dados Inválidos.");
            return BadRequest("Dados Inválidos.");
        }

        _context.Entry(category).State = EntityState.Modified;
        _context.SaveChanges();

        return Ok(category); 
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var category = _context.Categories.FirstOrDefault(p => p.CategoryId == id);

        if (category == null)
        {
            _logger.LogWarning($"Categoria de id = {id} não encontrada.");
            return NotFound($"Categoria de id = {id} não encontrada.");
        }

        _context.Categories.Remove(category);
        _context.SaveChanges();

        return Ok(category);      
    }
}
