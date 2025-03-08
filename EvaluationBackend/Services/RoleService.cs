using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EvaluationBackend.DATA;
using EvaluationBackend.DATA.DTOs.roles;
using EvaluationBackend.Entities;
using EvaluationBackend.Repository;

namespace EvaluationBackend.Services
{
    public interface IRoleService
    {
         Task<(List<RoleDto>? roles, int? totalCount, string? error)> GetAll();
         
         Task<(RoleDto? role, string? error)> Add(RoleForm role);
         
         Task<(RoleDto? roleDto, string? error)> Edit(int id, RoleForm role);
         
         Task<(RoleDto? roleDto, string? error)> Delete(int id);
         

         
    }
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly DataContext _dataContext;

        public RoleService( IMapper mapper, IRepositoryWrapper repositoryWrapper, DataContext dataContext)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _dataContext = dataContext;
        }

        public async Task<(List<RoleDto>? roles, int? totalCount, string? error)> GetAll()
        {
           var (data, totalCount) = await _repositoryWrapper.Role.GetAll<RoleDto>(0);
              return (_mapper.Map<List<RoleDto>>(data), totalCount, null);
        }
        public async Task<(RoleDto? role, string? error)> Add(RoleForm roleForm)
        {
            var check = await _repositoryWrapper.Role.Get<RoleDto>(x => x.Name == roleForm.Name);
            if (check != null)
            {
                return (null, "Role already exists");
            }
            
            var role = new Role()
            {
                Name = roleForm.Name
            };
            var response = await _repositoryWrapper.Role.Add(role);
            if (response == null)
            {
                return (null, "Error");
            }
            return (_mapper.Map<RoleDto>(role), null);
        }
        public async Task<(RoleDto? roleDto, string? error)> Edit(int id, RoleForm role)
        {
            var roleEntity = await _repositoryWrapper.Role.Get(x => x.Id == id);
            if (roleEntity == null)
            {
                return (null, "Role not found");
            }

            roleEntity.Name = role.Name;
            var response = await _repositoryWrapper.Role.Update(roleEntity);
            if (response == null)
            {
                return (null, "Error");
            }
            return (_mapper.Map<RoleDto>(response), null);
        }
        public async Task<(RoleDto? roleDto, string? error)> Delete(int id)
        {
            var role = await _repositoryWrapper.Role.Get(x => x.Id == id);
            if (role == null)
            {
                return (null, "Role not found");
            }

            var response = await _repositoryWrapper.Role.Delete(id);
            if (response == null)
            {
                return (null, "Error");
            }
            return (_mapper.Map<RoleDto>(response), null);
        }


       
    }
}