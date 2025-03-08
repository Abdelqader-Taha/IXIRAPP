using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string ProductType { get; set; }

        public string? City { get; set; }

        [Required]
        public string StoreType { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string? StoreLogo { get; set; }

        public string PlatformType { get; set; }

        [Required]
        public int Followers { get; set; }

        [Url]
        public string Link { get; set; }
    }
    }
