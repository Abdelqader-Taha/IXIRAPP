using IXIR.Entities;
using IXIR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;

using System.Linq.Expressions;
using System.Threading.Tasks;
using IXIR.DATA.DTOs.Product;

namespace IXIR.Controllers
{
    [Authorize (Roles="DataEntry")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            return Ok(product);
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Ensure pageSize is at least 1; if it's 0, return a custom message
                if (pageSize <= 0)
                {
                    return BadRequest(new { message = "Page size must be greater than 0." });
                }

                var (products, totalCount, error) = await _productService.GetProducts(pageNumber, pageSize);

                if (!string.IsNullOrEmpty(error) || products == null || !products.Any())
                {
                    return Ok(new
                    {
                        products = new List<ProductDTO>(), // Return an empty list
                        totalCount = 0,
                        pageNumber,
                        pageSize,
                        totalPages = 0,
                        message = "No products available."
                    });
                }

                return Ok(new
                {
                    products,
                    totalCount,
                    pageNumber,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while fetching products.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductForm req)
        {
            try
            {
                if (req == null || string.IsNullOrWhiteSpace(req.Name))
                    return BadRequest("Product name cannot be empty");

                var createdProductDTO = await _productService.CreateProduct(req.Name);
                return CreatedAtAction(nameof(GetProducts), new { id = createdProductDTO.Id }, createdProductDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpDateProductForm req)
        {
            try
            {
                if (req == null || string.IsNullOrWhiteSpace(req.Name))
                    return BadRequest("Product name cannot be empty");

                var updatedProductDTO = await _productService.UpdateProduct(id, req.Name);
                if (updatedProductDTO == null)
                    return NotFound($"Product with ID {id} not found");

                return Ok(updatedProductDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                var success = await _productService.DeleteProduct(id);
                if (!success)
                    return NotFound($"Product with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
