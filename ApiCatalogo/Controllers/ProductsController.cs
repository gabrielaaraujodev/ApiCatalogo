using ApiCatalogo.DTOs;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

    public ProductsController(IUnitOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get([FromQuery] ProductsParameters productsParameters)
    {
        var products = await _uof.ProductRepository.GetProductsAsync(productsParameters);

        var metadata = new
        {
            products.Count,
            products.PageSize,
            products.PageCount,
            products.TotalItemCount,
            products.HasNextPage,
            products.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDTO);
    }

    [HttpGet("filter/price/pagination")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsFilterPrice([FromQuery] ProductsFilterPrice productsFilterParameters)
    {
        var products = await _uof.ProductRepository.GetProductsFilterPriceAsync(productsFilterParameters);

        var metadata = new
        {
            products.Count,
            products.PageSize,
            products.PageCount,
            products.TotalItemCount,
            products.HasNextPage,
            products.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDTO);
    }

    /// <summary>
    /// Exibe uma relação de produtos
    /// </summary>
    /// <returns>Retorna uma lista de objetos Produto</returns>
    [HttpGet]
    [Authorize(Policy = "UserOnly")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
    {
        try
        {
            var products = await _uof.ProductRepository.GetAllAsync();

            if (products is null)
            {
                _logger.LogWarning("Lista de produtos inexistentes.");
                return NotFound("Lista de produtos inexistente.");
            }

            var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);


            return Ok(products);
        } catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    /// <summary>
    /// Obtem Produto pelo seu identificador
    /// </summary>
    /// <param name="id">Código do produto</param>
    /// <returns>Um objeto produto</returns>
    [HttpGet("{id:int}", Name="ObterProduto")]
    public async Task<ActionResult<ProductDTO>> Get(int id)
    {

        var product = await _uof.ProductRepository.GetAsync(c => c.ProductId == id);

        if (product is null)
        {
            _logger.LogWarning($"Produto com o id = {id} inexistente.");
            return NotFound($"Produto com o id = {id} inexistente.");
        }

        var productDTO = _mapper.Map<ProductDTO>(product);

        return Ok(productDTO);
    }

    [HttpGet("produtos/{id}")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsCategory(int id)
    {
        var products = await _uof.ProductRepository.GetProdructsByCategoryAsync(id);

        if (products is null)
            return NotFound();

        var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);

        return Ok(productsDTO);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDTO>> Post(ProductDTO productDTO)
    {
        if (productDTO is null)
        {
            _logger.LogWarning($"Houve um problema em adicionar o novo produto de nome {productDTO?.Name}.");
            return BadRequest($"Houve um problema em adicionar o novo produto de nome {productDTO?.Name}.");
        }

        var productNormal = _mapper.Map<Product>(productDTO);

        var newProduct = _uof.ProductRepository.Create(productNormal);
        await _uof.CommitAsync();

        var newProductDTO = _mapper.Map<ProductDTO>(newProduct);

        return new CreatedAtRouteResult("ObterProduto", new { id = newProductDTO.ProductId, newProductDTO });       
    }

    [HttpPatch("{id}/UpdatePartial")]
    public async Task<ActionResult<ProductDtoUpdateResponse>> Patch(int id, JsonPatchDocument<ProductDtoUpdateRequest> patchProductDTO)
    {
        if (patchProductDTO is null || id <= 0)
            return BadRequest();

        var productNormal = await _uof.ProductRepository.GetAsync(c => c.ProductId == id);

        if (productNormal is null)
            return NotFound();

        var productUpdateRequestDTO = _mapper.Map<ProductDtoUpdateRequest>(productNormal);

        patchProductDTO.ApplyTo(productUpdateRequestDTO, ModelState);

        if (!ModelState.IsValid || TryValidateModel(productUpdateRequestDTO))
            return BadRequest(ModelState);

        _mapper.Map(productUpdateRequestDTO, productNormal);

        _uof.ProductRepository.Update(productNormal);
        await _uof.CommitAsync();

        return Ok(_mapper.Map<ProductDtoUpdateResponse>(productNormal));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDTO>> Put(int id, ProductDTO productDTO)
    { 
        if (id != productDTO.ProductId)
        {
            _logger.LogWarning($"Houve um problema em alterar o produto de id = {id}");
            return BadRequest($"Houve um problema em alterar o produto de id = {id}");
        }

        var productNormal = _mapper.Map<Product>(productDTO);

        var productAtt = _uof.ProductRepository.Update(productNormal);
        await _uof.CommitAsync();

        var newProductDTO = _mapper.Map<ProductDTO>(productAtt);

        return Ok(newProductDTO);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProductDTO>> Delete(int id)
    {
        var product = await _uof.ProductRepository.GetAsync(c => c.ProductId == id);

        if (product is null)
            return NotFound("Produto não encontrado ...");

        var deletedProduct = _uof.ProductRepository.Delete(product);
        await _uof.CommitAsync();

        var productDTO = _mapper.Map<ProductDTO>(deletedProduct);

        return Ok(deletedProduct);
    }
}
