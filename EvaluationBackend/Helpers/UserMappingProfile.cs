using AutoMapper;
using EvaluationBackend.DATA.DTOs;
using EvaluationBackend.DATA.DTOs.roles;
using EvaluationBackend.DATA.DTOs.Store;
using EvaluationBackend.DATA.DTOs.User;
using EvaluationBackend.Entities;
using OneSignalApi.Model;


namespace EvaluationBackend.Helpers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {

            CreateMap<AppUser, UserDto>()
              .ForMember(r => r.Role, opt => opt.MapFrom(src => src.Role.Name)); // Ensure role mapping
              //.ForMember(r => r.Deleted, opt => opt.MapFrom(src => src.Deleted));

            CreateMap<RegisterForm,App>().ForAllMembers( opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Role, RoleDto>();
            CreateMap<AppUser, AppUser>();
            

            CreateMap<Store,StoreDTO>();

        }
    }
}