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
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(AppDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAsync()
    {
        var products = await _context.Products.AsNoTracking().ToListAsync();

        if (products is null)
        {
            _logger.LogWarning("Lista de produtos inexistentes.");
            return NotFound("Lista de produtos inexistente.");
        }

        return products;
    }

    [HttpGet("{id:int}", Name="ObterProduto")]
    public async Task<ActionResult<Product>> GetAsync(int id)
    {

        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);

        if (product is null)
        {
            _logger.LogWarning($"Produto com o id = {id} inexistente.");
            return NotFound($"Produto com o id = {id} inexistente.");
        }

        return product;
    }

    [HttpPost]
    public ActionResult Post(Product product)
    {
        if (product is null)
        {
            _logger.LogWarning($"Houve um problema em adicionar o novo produto de nome {product?.Name}.");
            return BadRequest($"Houve um problema em adicionar o novo produto de nome {product?.Name}.");
        }

        _context.Products.Add(product);
        _context.SaveChanges();

        return new CreatedAtRouteResult("ObterProduto", new { id = product.ProductId, product });       
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Product product)
    { 
        if (id != product.ProductId)
        {
            _logger.LogWarning($"Houve um problema em alterar o produto de id = {id}");
            return BadRequest($"Houve um problema em alterar o produto de id = {id}");
        }

        _context.Entry(product).State = EntityState.Modified;
        _context.SaveChanges();

        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductId == id);

        if (product is null)
        {
            _logger.LogWarning($"Houve um problema em deletar o produto de id = {id}");
            return NotFound($"Houve um problema em deletar o produto de id = {id}");
        }

        _context.Products.Remove(product);
        _context.SaveChanges();

        return Ok(product);
    }
}
