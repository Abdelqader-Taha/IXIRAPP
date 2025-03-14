using EvaluationBackend.DATA.DTOs.User;
using EvaluationBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneSignalApi.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EvaluationBackend.Controllers
{
    //[Authorize(Roles = "Admin")]

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // User login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginForm loginForm)
        {
            var (token,  error) = await _userService.Login(loginForm);
            if (error != null) return Unauthorized(new { message = error });
            return Ok(new { token,  });
        }



        [Authorize(Roles = "Admin")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterForm registerForm)
        {
            var (user, error) = await _userService.Register(registerForm);

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(new { message = error });
            }

            return Ok(new { message = "Registration successful." }); // Only returning the success message
        }




        [Authorize(Roles = "Admin")]
        [HttpGet("User/{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var (user, error) = await _userService.GetUserById(id);
            if (error != null) return NotFound(new { message = error });
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            var (users, totalUsers, error) = await _userService.GetAllUsers(pageNumber, pageSize);

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(new { message = error });
            }

            if (users == null || !users.Any())
            {
                return Ok(new { message = "No users found", paginationMeta = new { currentPage = pageNumber, pageSize, totalUsers = 0, totalPages = 0 } });
            }

            var paginationMeta = new
            {
                currentPage = pageNumber,
                pageSize = pageSize,
                totalUsers = totalUsers,
                totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize)
            };

            return Ok(new { users, paginationMeta });
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("User/{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserForm updateUserForm, Guid id)
        {
            var (user, error) = await _userService.UpdateUser(updateUserForm, id);
            if (error != null) return NotFound(new { message = error });

            return Ok(new
            {
                id = user.Id,
                userName = user.UserName,
                fullName = user.FullName,
                roleId = user.RoleId,  
                deleted = user.Deleted,
                creationDate = user.CreationDate
            });
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("User/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var (user, error) = await _userService.DeleteUser(id);

            if (error != null)
            {
                return NotFound(new { message = $"User  not found or has already been deleted." });
            }

            return Ok(new { message = "User successfully deleted." });
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePassword([FromRoute] Guid id, [FromBody] ChangePasswordForm changePasswordForm)
        {
            var (user, error) = await _userService.ChangePassword(changePasswordForm, id);

            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            return Ok(new { message = "Password changed successfully", user });
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Get the current user ID from the token claims (we use the "id" claim)
            var userIdClaim = User.FindFirst("id") ?? User.FindFirst("sub"); 

            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { message = "Invalid user ID format" });
            }

            // Fetch the user profile using the userId
            var (user, error) = await _userService.GetUserById(userId);

            if (error != null)
            {
                return NotFound(new { message = error });
            }

            return Ok(user);
        }


    }
}
