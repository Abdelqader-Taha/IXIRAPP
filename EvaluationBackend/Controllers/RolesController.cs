using EvaluationBackend.DATA.DTOs.roles;
using EvaluationBackend.Services;
using EvaluationBackend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvaluationBackend.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RolesController : BaseController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]

        [HttpGet]
        public async Task<ActionResult<Respons<RoleDto>>> GetAll() => Ok(await _roleService.GetAll(),0);
        [ApiExplorerSettings(IgnoreApi = true)]

        [HttpPost]
        public async Task<ActionResult<RoleDto>> Add(RoleForm roleForm) => Ok(await _roleService.Add(roleForm));
        [ApiExplorerSettings(IgnoreApi = true)]

        [HttpPut("{id}")]
        public async Task<ActionResult<RoleDto>> Edit(int id, RoleForm roleForm) => Ok(await _roleService.Edit(id, roleForm));
        [ApiExplorerSettings(IgnoreApi = true)]

        [HttpDelete("{id}")]
        public async Task<ActionResult<RoleDto>> Delete(int id) => Ok(await _roleService.Delete(id));
        
        //[HttpPost("{id}/permissions")]
       // public async Task<IActionResult> AddPermissionToRoleAsync(int id, AssignPermissionsDto permissions) => Ok(await _roleService.AddPermissionToRole(id, permissions));
        
    }
}