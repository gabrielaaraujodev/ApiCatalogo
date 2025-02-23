using ApiCatalogo.Context;
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

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Category>> Get()
    {
        try
        {
            return _context.Categories.AsNoTracking().ToList();
        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                "Houve um problema ao acessar a lista de categorias.");
        }
    }

    [HttpGet("{id:int}", Name ="ObterCategoria")]
    public ActionResult<Category> Get(int id)
    {
        try
        {
            var category = _context.Categories.FirstOrDefault(p => p.CategoryId == id);

            if (category == null)
                return NotFound("Houve um problema com o id da categoria.");

            return Ok(category);
        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Houve um problema ao acessar categoria de id = {id}.");
        }
    }

    [HttpGet("products")]
    public ActionResult<IEnumerable<Category>> GetCategoriesProducts()
    {
        try
        {
            return _context.Categories.Include(p => p.Products).ToList();
        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Houve um problema ao acessar a lista de categorias com seus produtos.");
        }
    }

    [HttpPost]
    public ActionResult Post(Category category)
    {
        try
        {
            if (category is null)
                return BadRequest("A categoria está incorreta.");

            _context.Categories.Add(category);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterCategoria", new { id = category.CategoryId }, category);
        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Houve um problema adicionar uma nova categoria.");
        }      
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Category category)
    {
        try
        {
            if (id != category.CategoryId)
                return BadRequest("Houve um problema em modificar o id da categoria.");

            _context.Entry(category).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(category);
        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Houve um problema modificar uma nova categoria.");
        }
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        try
        {
            var category = _context.Categories.FirstOrDefault(p => p.CategoryId == id);

            if (category == null)
                return NotFound("Houve um problema de achar o id da categoria.");

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok(category);
        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Houve um problema remover esta categoria.");
        }
    }
}
