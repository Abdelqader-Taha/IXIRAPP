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
        public async Task<IActionResult> GetProducts( int pageNumber = 1, int pageSize = 10)
        {
            

                var (products, totalProducts, error) = await _productService.GetAllProducts(pageNumber, pageSize);

                if (!string.IsNullOrEmpty(error) )
                {
                  return BadRequest(new { message = error });

                }
              var productlist = products ?? new List<ProductDTO>();

            var paginationMeta = new
            {
                currentpage = pageNumber,
                pageSize = pageSize,
                totalProducts=totalProducts,
                totalPages = totalProducts > 0 ? (int)Math.Ceiling(totalProducts / (double)pageSize) : 0
            };
            return Ok(new
            {
                data=productlist,
                paginationMeta,
                message= productlist.Any()? null:" No product found"
            });




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
