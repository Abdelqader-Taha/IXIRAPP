using EvaluationBackend.DATA.DTOs.Store;
using EvaluationBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvaluationBackend.Controllers
{
    [Authorize(Roles = "DataEntry")]
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateStoreAsync([FromBody] CreateStoreForm req)
        {
            if (req == null)
            {
                return BadRequest(new { message = "Invalid store data" });
            }

            // Get the current user ID from the token claims
            var userIdClaim = User.FindFirst("id") ?? User.FindFirst("sub"); // Adjust claim name as needed

            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "Invalid user ID format" });
            }

            // Pass the user ID to the service
            var (store, error) = await _storeService.CreateStore(req, userId);

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
            return Ok( new {Message= "Store updated successfully." });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStoreByIdAsync(Guid id)
        {
            var (success, error) = await _storeService.DeleteStore(id);
            if (!success)
            {
                return NotFound(new { message = error });
            }
            return Ok(new { message = "Store deleted successfully." });
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
