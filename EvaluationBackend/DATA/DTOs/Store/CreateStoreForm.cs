using System.ComponentModel.DataAnnotations;

namespace EvaluationBackend.DATA.DTOs.Store
{
    public class CreateStoreForm
    {
        public Guid UserId { get; set; }

        [Required]
        public string StoreName { get; set; }

        [Required]
        public string ProductType { get; set; }

        public string? City { get; set; }

        [Required]
        public string StoreType { get; set; }


        [Required]
        public string PhoneNumber { get; set; }

        public string? StoreLogo { get; set; }

        [Required]
        public string PlatformType { get; set; }

        [Required]
        public int Followers { get; set; }

        
        public string? Link { get; set; }
    }
}
