using EvaluationBackend.Entities;
using System.ComponentModel.DataAnnotations;

namespace IXIR.Entities
{
    public class Product:BaseEntity<Guid>
    {
        [Required]
        public string Name  { get; set; }
    }
}
