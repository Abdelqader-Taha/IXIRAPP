using System.ComponentModel.DataAnnotations;

namespace EvaluationBackend.DATA.DTOs.roles
{
    public class RoleForm
    {
        [Required]
        public string Name { get; set; }
    }
}