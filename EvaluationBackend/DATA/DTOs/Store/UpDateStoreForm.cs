using System;
using System.ComponentModel.DataAnnotations;

namespace EvaluationBackend.DATA.DTOs.Store
{
    public class UpDateStoreForm
    {
        public string Link { get; set; }
        [Required]
        public string StoreName { get; set; }

        [Required]
        public Guid ProductId { get; set; }  // Reference to Product entity

        public string? City { get; set; }

        [Required]
        public string StoreType { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string? StoreLogo { get; set; }

        [Required]
        public string PlatformType { get; set; }

        [Required]
        public int? Followers { get; set; }

    }
}
