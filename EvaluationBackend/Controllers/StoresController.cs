using EvaluationBackend.DATA.DTOs.Store;
using EvaluationBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvaluationBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoresController(IStoreService storeService)
        {
            _storeService = storeService;
        }



        [Authorize(Roles = "Admin,DataEntry")]
        [HttpGet("by-creator-id")]
        public async Task<IActionResult> GetStoresByUserId(int pageNumber = 1, int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Assuming the userId is stored under the NameIdentifier claim
            if (userIdClaim == null)
            {
                return BadRequest(new { message = "User ID is not found in the token." });
            }

            Guid userId;
            if (!Guid.TryParse(userIdClaim.Value, out userId))
            {
                return BadRequest(new { message = "Invalid User ID in the token." });
            }

            var result = await _storeService.GetStoresByUserId(userId, pageNumber, pageSize);

            if (!string.IsNullOrEmpty(result.error))
            {
                return BadRequest(new { message = result.error });
            }

            var storesList = result.stores ?? new List<StoreDTO>();

            var paginationMeta = new
            {
                currentPage = pageNumber,
                pageSize = pageSize,
                totalStores = result.stores.Count(),
                totalPages = result.stores.Count() > 0 ? (int)Math.Ceiling(result.stores.Count() / (double)pageSize) : 0
            };

            return Ok(new
            {
                data = storesList,
                paginationMeta,
                message = storesList.Any() ? null : "No stores found"
            });
        }

        [Authorize(Roles = "DataEntry,Admin")]

        [HttpGet]
        public async Task<IActionResult> GetAllStoresAsync(int pageNumber = 1, int pageSize = 10)
        {
            var (stores, totalStors, error) = await _storeService.GetAllStores(pageNumber, pageSize);

            if (!string.IsNullOrEmpty(error))
            {
                return NotFound(new { message = error });
            }
            var storeslist = stores ?? new List<StoreDTO>();

            var paginationMeta = new
            {
                currentPage = pageNumber,
                pageSize = pageSize,
                totalStores = totalStors,
                totalPages = totalStors > 0 ? (int)Math.Ceiling(totalStors / (double)pageSize) : 0
            };


            return Ok(

                 new
                 {
                     data = storeslist,
                     paginationMeta,
                     message = storeslist.Any() ? null : "No stores available."

                 }
                );
        }

        [Authorize(Roles = "DataEntry")]

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

        [Authorize(Roles = "DataEntry")]
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


        [Authorize(Roles = "DataEntry")]

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

        [Authorize(Roles = "DataEntry")]

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

        [ApiExplorerSettings(IgnoreApi = true)]

        [Authorize(Roles = "DataEntry")]

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
