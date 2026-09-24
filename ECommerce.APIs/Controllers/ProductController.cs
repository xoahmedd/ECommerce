using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ECommerce.APIs.DTOs;
using ECommerce.APIs.Errors;
using ECommerce.APIs.Helpers;
using ECommerce.Core.Entities.Product;
using ECommerce.Core.Reposiroties_Contracts;
using ECommerce.Core.Specification;

namespace ECommerce.APIs.Controllers
{

    public class ProductController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<ProductCategory> _productCategoryRepo;
        private readonly IMapper _mapper;

        public ProductController(IGenericRepository<Product> productsRepo,
            IGenericRepository<ProductBrand> productBrandRepo,
            IGenericRepository<ProductCategory> productCategoryRepo,
            IMapper mapper)
        {
            _productsRepo = productsRepo;
            _productBrandRepo = productBrandRepo;
            _productCategoryRepo = productCategoryRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetAllProducts([FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductWithBrandAndCategorySpecification(specParams);

            var products = await _productsRepo.GetAllWithSpecAsync(spec);

            var data = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductToReturnDto>>(products);

            var countSpec = new ProductsWithFilterationForCountSpecification(specParams);

            var count = await _productsRepo.GetCountAsync(countSpec);

            return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex, specParams.PageSize, count, data));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndCategorySpecification(id);

            var product = await _productsRepo.GetByIdWithSpecAsync(spec);

            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }

            return Ok(_mapper.Map<Product, ProductToReturnDto>(product));
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetAllBrands()
        {
            var brands = await _productBrandRepo.GetAllAsync();

            return Ok(brands);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetAllCategories()
        {
            var categories = await _productCategoryRepo.GetAllAsync();

            return Ok(categories);
        }

    }
}

