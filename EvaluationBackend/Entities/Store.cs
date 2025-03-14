using IXIR.Entities;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EvaluationBackend.Entities
{
    public class Store : BaseEntity<Guid>
    {
        [Required]
        public Guid UserId { get; set; }  // Foreign Key
        public AppUser User { get; set; }

        [Required]
        public string StoreName { get; set; }

        [Required]
        public string StoreType { get; set; }

        public string? City { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string? StoreLogo { get; set; }

        public string PlatformType { get; set; }

        [Required]
        public int Followers { get; set; }

        [Url]
        public string? Link { get; set; }

        // Many-to-Many Relationship
        public List<Product> Products { get; set; } = new List<Product>();

    }
}
