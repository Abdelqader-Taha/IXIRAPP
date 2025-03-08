using System.ComponentModel.DataAnnotations;

namespace EvaluationBackend.DATA.DTOs.Store
{
    public class UpDateStoreForm
    {
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

        //[Url]
        public string Link { get; set; }
        //public bool? Deleted { get; set; } // Nullable, in case it’s not provided in the update request

    }
}
