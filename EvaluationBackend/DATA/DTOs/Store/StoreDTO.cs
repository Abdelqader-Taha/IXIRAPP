using EvaluationBackend.Entities;
using System.ComponentModel.DataAnnotations;

namespace EvaluationBackend.DATA.DTOs.Store
{
    public class StoreDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        // public AppUser User { get; set; }

        public  bool Deleted { get; set; }
        [Required]
        public string StoreName { get; set; }

        [Required]
        public string ProductType { get; set; }

        public string? City { get; set; }

        [Required]
        public string StoreType { get; set; }

        [Required]
        //[RegularExpression(@"^\d{4}-\d{3}-\d{4}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        public string? StoreLogo { get; set; }

        public string PlatformType { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Followers must be a non-negative number.")]
        public int Followers { get; set; }

        [Url]
        public string Link { get; set; }

    }
}
