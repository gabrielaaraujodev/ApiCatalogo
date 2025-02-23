using ApiCatalogo.Context;
using ApiCatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()
    {
        try
        {
            var products = _context.Products.ToList();

            if (products is null)
                return NotFound("Lista de produtos inexistente.");

            return products;
        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                "Houve um problema ao resgatar a lista de produtos.");
        }
    }

    [HttpGet("{id:int}", Name="ObterProduto")]
    public ActionResult<Product> Get(int id)
    {
        try
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);

            if (product is null)
                return NotFound($"Produto com o id = {id} inexistente.");

            return product;
        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                "Houve um problema ao resgatar o produto.");
        }
    }

    [HttpPost]
    public ActionResult Post(Product product)
    {
        try
        {
            if (product is null)
                return BadRequest("Houve um problema em adicionar o novo produto.");

            _context.Products.Add(product);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto", new { id = product.ProductId, product });
        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                "Houve um problema em adicionar o produto.");
        }        
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Product product)
    {
        try
        {
            if (id != product.ProductId)
                return BadRequest("Houve um problema em alterar o produto.");

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(product);
        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                "Houve um problema em alterar o produto.");
        }   
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        try
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);

            if (product is null)
                return NotFound("Houve um problema em deleter o produto.");

            _context.Products.Remove(product);
            _context.SaveChanges();

            return Ok(product);
        }
        catch (Exception)
        {

            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Houve um problema em remover o produto de id = {id}.");
        }
    }
}
