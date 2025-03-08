using System.ComponentModel.DataAnnotations;

namespace EvaluationBackend.DATA.DTOs.User
{
    public class RegisterForm
    {
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? Password { get; set; }

       // [Required]
        //[EmailAddress]
        //public string? Email { get; set; }
        
        [Required]
        [MinLength(2, ErrorMessage = "User Name must be at least 2 characters")]
        public string UserName { get; set; }
        public string FullName { get; set; }

        [Required]
        public int Role { get; set; }

    }
}