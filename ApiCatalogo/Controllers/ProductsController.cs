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
    /*
        Esta implementação é referente ao código específico da interface
        de produto que foi criada.
    */
    private readonly IProductRepository _productRepository;
    
    /*
        Esta implementação é referente ao código genérico da interface
        de repository que foi criada.
    */
    private readonly IRepository<Product> _repository;

    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IRepository<Product> repository, IProductRepository productRepository, ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()
    {
        var products = _repository.GetAll();

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

        var product = _repository.Get(c => c.ProductId == id);

        if (product is null)
        {
            _logger.LogWarning($"Produto com o id = {id} inexistente.");
            return NotFound($"Produto com o id = {id} inexistente.");
        }

        return Ok(product);
    }

    [HttpGet("produtos/{id}")]
    public ActionResult<IEnumerable<Product>> GetProductsCategory(int id)
    {
        var products = _productRepository.GetProdructsByCategory(id);

        if (products is null)
            return NotFound();

        return Ok(products);
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

        var productAtt = _repository.Update(product);

        return Ok(productAtt);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var product = _repository.Get(c => c.ProductId == id);

        if (product is null)
            return NotFound("Produto não encontrado ...");

        var deletedProduct = _repository.Delete(product);

        return Ok(deletedProduct);
    }
}
