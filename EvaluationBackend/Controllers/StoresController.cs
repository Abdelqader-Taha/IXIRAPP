using EvaluationBackend.DATA.DTOs.Store;
using EvaluationBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvaluationBackend.Controllers
{
    [Authorize(Roles = "Admin,DataEntry")]
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoresController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStoresAsync(int pageNumber = 1, int pageSize = 10)
        {
            var (stores, totalStors, error) = await _storeService.GetAllStores(pageNumber, pageSize);

            if (!string.IsNullOrEmpty(error))
            {
                return NotFound(new { message = error });
            }

            if (stores == null || !stores.Any())
            {
                return Ok(new { message = "No stores available.", paginationMeta = new { currentPage = pageNumber, pageSize, totalStors = 0, totalPages = 0 } });
            }

            var paginationMeta = new
            {
                currentPage = pageNumber,
                pageSize = pageSize,
                totalStores = totalStors,
                totalPages = (int)Math.Ceiling(totalStors / (double)pageSize)
            };


            return Ok(new { stores, paginationMeta });
        }


        [HttpGet("distinct-product-types")]
        public async Task<IActionResult> GetDistinctProductTypes()
        {
            var (productTypes, error) = await _storeService.GetDistinctProductTypes();

            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            return Ok(productTypes);
        }

        [HttpGet("stores-by-product-type")]
        public async Task<IActionResult> GetStoresByProductType([FromQuery] List<string> productTypes)
        {
            // If the productTypes list is empty or null, return an error
            if (productTypes == null || !productTypes.Any())
            {
                return BadRequest(new { message = "Product types are required." });
            }

            // Call the service method with the list of product types
            var (stores, error) = await _storeService.GetStoresByProductType(productTypes);

            // If there was an error, return a BadRequest response with the error message
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(new { message = error });
            }

            // Return the stores as a successful response
            return Ok(stores);
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreByIdAsync(Guid id)
        {
            var (store, error) = await _storeService.GetStoreById(id);
            if (error != null)
            {
                return NotFound(new { message = error });
            }
            return Ok(store);
        }

        [Authorize] // Ensure the user is authenticatedz
        [HttpPost]
        
        public async Task<IActionResult> CreateStoreAsync([FromBody] CreateStoreForm req)
        {
            if (req == null)
            {
                return BadRequest(new { message = "Invalid store data" });
            }


            var (store, error) = await _storeService.CreateStore( req);
            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            return Ok(store);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditStoreByIdAsync(Guid id, [FromBody] UpDateStoreForm req)
        {
            if (req == null)
            {
                return BadRequest(new { message = "Invalid store data" });
            }

            var (updatedStore, error) = await _storeService.UpdateStore(id, req);
            if (error != null)
            {
                return NotFound(new { message = error });
            }
            return Ok(updatedStore);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStoreByIdAsync(Guid id)
        {
            var (success, error) = await _storeService.DeleteStore(id);
            if (!success)
            {
                return NotFound(new { message = error });
            }
            return NoContent();
        }

        [HttpPut("undelete/{id}")]
        public async Task<IActionResult> UnDelete(Guid id)
        {
            var (store, error) = await _storeService.UnDeleteStore(id);

            if (error !=null)
            {
                return NotFound(new { message = error });


            }

            return Ok(store);
        }

    }
}
