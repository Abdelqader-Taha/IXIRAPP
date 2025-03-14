using IXIR.DATA.DTOs.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EvaluationBackend.DATA.DTOs.Store
{
    public class StoreDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public bool Deleted { get; set; }

        [Required]
        public string StoreName { get; set; }

        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>(); // Updated to return multiple products

        public string? City { get; set; }

        [Required]
        public string StoreType { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string? StoreLogo { get; set; }

        public string PlatformType { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Followers must be a non-negative number.")]
        public int? Followers { get; set; }

        [Url]
        public string? Link { get; set; }
        public DateTime? CreationDate { get; set; } = DateTime.UtcNow;

    }
}
