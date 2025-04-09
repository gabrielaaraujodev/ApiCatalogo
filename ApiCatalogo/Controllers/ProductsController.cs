using ApiCatalogo.DTOs;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiCatalogo.Controllers;

[Route("[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    /*
        Esta implementação é referente ao código específico da interface
        de produto que foi criada.
    */
    //private readonly IProductRepository _productRepository;

    /*
        Esta implementação é referente ao código genérico da interface
        de repository que foi criada.
    */
    //private readonly IRepository<Product> _repository;

    private IUnitOfWork _uof;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ILogger<ProductsController> logger, IUnitOfWork uof, IMapper mapper)
    {
        _logger = logger;
        _uof = uof;
        _mapper = mapper;
    }

    [HttpGet("pagination")]
    public ActionResult<IEnumerable<ProductDTO>> Get([FromQuery] ProductsParameters productsParameters)
    {
        var products = _uof.ProductRepository.GetProducts(productsParameters);

        var metadata = new
        {
            products.TotalCount,
            products.PageSize,
            products.CurrentPage,
            products.TotalPages,
            products.HasNext,
            products.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDTO);
    }

    [HttpGet("filter/price/pagination")]
    public ActionResult<IEnumerable<ProductDTO>> GetProductsFilterPrice([FromQuery] ProductsFilterPrice productsFilterParameters)
    {
        var products = _uof.ProductRepository.GetProductsFilterPrice(productsFilterParameters);

        var metadata = new
        {
            products.TotalCount,
            products.PageSize,
            products.CurrentPage,
            products.TotalPages,
            products.HasNext,
            products.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDTO);
    }

    [HttpGet]
    public ActionResult<IEnumerable<ProductDTO>> Get()
    {
        var products = _uof.ProductRepository.GetAll();

        if (products is null)
        {
            _logger.LogWarning("Lista de produtos inexistentes.");
            return NotFound("Lista de produtos inexistente.");
        }

        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);


        return Ok(products);
    }

    [HttpGet("{id:int}", Name="ObterProduto")]
    public ActionResult<ProductDTO> Get(int id)
    {

        var product = _uof.ProductRepository.Get(c => c.ProductId == id);

        if (product is null)
        {
            _logger.LogWarning($"Produto com o id = {id} inexistente.");
            return NotFound($"Produto com o id = {id} inexistente.");
        }

        var productDTO = _mapper.Map<ProductDTO>(product);

        return Ok(productDTO);
    }

    [HttpGet("produtos/{id}")]
    public ActionResult<IEnumerable<ProductDTO>> GetProductsCategory(int id)
    {
        var products = _uof.ProductRepository.GetProdructsByCategory(id);

        if (products is null)
            return NotFound();

        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDTO);
    }

    [HttpPost]
    public ActionResult<ProductDTO> Post(ProductDTO productDTO)
    {
        if (productDTO is null)
        {
            _logger.LogWarning($"Houve um problema em adicionar o novo produto de nome {productDTO?.Name}.");
            return BadRequest($"Houve um problema em adicionar o novo produto de nome {productDTO?.Name}.");
        }

        var productNormal = _mapper.Map<Product>(productDTO);

        var newProduct = _uof.ProductRepository.Create(productNormal);
        _uof.Commit();

        var newProductDTO = _mapper.Map<ProductDTO>(newProduct);

        return new CreatedAtRouteResult("ObterProduto", new { id = newProductDTO.ProductId, newProductDTO });       
    }

    [HttpPatch("{id}/UpdatePartial")]
    public ActionResult<ProductDtoUpdateResponse> Patch(int id, JsonPatchDocument<ProductDtoUpdateRequest> patchProductDTO)
    {
        if (patchProductDTO is null || id <= 0)
            return BadRequest();

        var productNormal = _uof.ProductRepository.Get(c => c.ProductId == id);

        if (productNormal is null)
            return NotFound();

        var productUpdateRequestDTO = _mapper.Map<ProductDtoUpdateRequest>(productNormal);

        patchProductDTO.ApplyTo(productUpdateRequestDTO, ModelState);

        if (!ModelState.IsValid || TryValidateModel(productUpdateRequestDTO))
            return BadRequest(ModelState);

        _mapper.Map(productUpdateRequestDTO, productNormal);

        _uof.ProductRepository.Update(productNormal);
        _uof.Commit();

        return Ok(_mapper.Map<ProductDtoUpdateResponse>(productNormal));
    }

    [HttpPut("{id:int}")]
    public ActionResult<ProductDTO> Put(int id, ProductDTO productDTO)
    { 
        if (id != productDTO.ProductId)
        {
            _logger.LogWarning($"Houve um problema em alterar o produto de id = {id}");
            return BadRequest($"Houve um problema em alterar o produto de id = {id}");
        }

        var productNormal = _mapper.Map<Product>(productDTO);

        var productAtt = _uof.ProductRepository.Update(productNormal);
        _uof.Commit();

        var newProductDTO = _mapper.Map<ProductDTO>(productAtt);

        return Ok(newProductDTO);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<ProductDTO> Delete(int id)
    {
        var product = _uof.ProductRepository.Get(c => c.ProductId == id);

        if (product is null)
            return NotFound("Produto não encontrado ...");

        var deletedProduct = _uof.ProductRepository.Delete(product);
        _uof.Commit();

        var productDTO = _mapper.Map<ProductDTO>(deletedProduct);

        return Ok(deletedProduct);
    }
}
