using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductRepository repository, ILogger<ProductsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()
    {
        var products = _repository.GetProducts().ToList();

        if (products is null)
        {
            _logger.LogWarning("Lista de produtos inexistentes.");
            return NotFound("Lista de produtos inexistente.");
        }

        return Ok(products);
    }

    [HttpGet("{id:int}", Name="ObterProduto")]
    public ActionResult<Product> Get(int id)
    {

        var product = _repository.GetProduct(id);

        if (product is null)
        {
            _logger.LogWarning($"Produto com o id = {id} inexistente.");
            return NotFound($"Produto com o id = {id} inexistente.");
        }

        return Ok(product);
    }

    [HttpPost]
    public ActionResult Post(Product product)
    {
        if (product is null)
        {
            _logger.LogWarning($"Houve um problema em adicionar o novo produto de nome {product?.Name}.");
            return BadRequest($"Houve um problema em adicionar o novo produto de nome {product?.Name}.");
        }

        var newProduct = _repository.Create(product);

        return new CreatedAtRouteResult("ObterProduto", new { id = newProduct.ProductId, newProduct });       
    }

    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Product product)
    { 
        if (id != product.ProductId)
        {
            _logger.LogWarning($"Houve um problema em alterar o produto de id = {id}");
            return BadRequest($"Houve um problema em alterar o produto de id = {id}");
        }

        bool att = _repository.Update(product);

        if (att) 
            return Ok(product);

        return StatusCode(500, $"Falha ao atualizar o produto de id = {id}");
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var deleted = _repository.Delete(id);

        if (deleted)
            return Ok($"Produto de id = {id} foi excluido.");

        return StatusCode(500, $"Falha ao excluir o produto de id = {id}");
    }
}
