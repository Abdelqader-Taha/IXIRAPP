using AutoMapper;
using e_parliament.Interface;
using EvaluationBackend.DATA.DTOs.User;
using EvaluationBackend.Entities;
using EvaluationBackend.Repository;
using Microsoft.EntityFrameworkCore;

namespace EvaluationBackend.Services
{
    public interface IUserService
    {
        Task<(UserDto? user, string? error)> Login(LoginForm loginForm);
        Task<(AppUser? user, string? error)> DeleteUser(Guid id);
        Task<(UserDto? userDto, string? error)> Register(RegisterForm registerForm);
        Task<(AppUser? user, string? error)> UpdateUser(UpdateUserForm updateUserForm, Guid id);
        Task<(UserDto? user, string? error)> GetUserById(Guid id);
        Task<(IEnumerable<UserDto> users, int totalUsers, string? error)> GetAllUsers(int pageNumber, int pageSize);
        Task<(UserDto? user, string? error)> ChangePassword(ChangePasswordForm changePasswordForm, Guid id);

    }

    public class UserService : IUserService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UserService(IRepositoryWrapper repositoryWrapper, IMapper mapper, ITokenService tokenService)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<(UserDto? user, string? error)> Login(LoginForm loginForm)
        {
            var user = await _repositoryWrapper.User.Get(u => u.UserName == loginForm.UserName && !u.Deleted,
                                                         i => i.Include(x => x.Role));

            if (user == null) return (null, "User not found or deleted");
            if (!BCrypt.Net.BCrypt.Verify(loginForm.Password, user.Password)) return (null, "Wrong password");

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Token = _tokenService.CreateToken(userDto, user.Role);
            user.LastActive = DateTime.UtcNow;
            await _repositoryWrapper.Save();
            return (userDto, null);
        }

        public async Task<(AppUser? user, string? error)> DeleteUser(Guid id)
        {
            var user = await _repositoryWrapper.User.GetById(id);
            if (user == null || user.Deleted)
            {
                return (null, "User not found or already deleted");
            }

            user.Deleted = true;
            await _repositoryWrapper.User.Update(user);
            await _repositoryWrapper.Save();
            return (user, null);
        }

        public async Task<(UserDto? userDto, string? error)> Register(RegisterForm registerForm)
        {
            var role = await _repositoryWrapper.Role.Get(r => r.Id == registerForm.Role);
            if (role == null) return (null, "Role not found");

            var user = await _repositoryWrapper.User.Get(u => u.UserName == registerForm.UserName && !u.Deleted);
            if (user != null) return (null, "User already exists");

            var newUser = new AppUser
            {
                UserName = registerForm.UserName,
                FullName = registerForm.FullName,
                Password = BCrypt.Net.BCrypt.HashPassword(registerForm.Password),
                RoleId = role.Id
            };

            await _repositoryWrapper.User.CreateUser(newUser);
            var userDto = _mapper.Map<UserDto>(newUser);
            userDto.Token = _tokenService.CreateToken(userDto, role);
            return (userDto, null);
        }

        public async Task<(AppUser? user, string? error)> UpdateUser(UpdateUserForm updateUserForm, Guid id)
        {
            var user = await _repositoryWrapper.User.Get(u => u.Id == id && !u.Deleted);
            if (user == null) return (null, "User not found or deleted");

            if (!string.IsNullOrEmpty(updateUserForm.UserName) && user.UserName != updateUserForm.UserName)
                user.UserName = updateUserForm.UserName;

            if (!string.IsNullOrEmpty(updateUserForm.FullName) && user.FullName != updateUserForm.FullName)
                user.FullName = updateUserForm.FullName;

            

            // Update RoleId if provided and exists
            if (updateUserForm.RoleId > 0 && user.RoleId != updateUserForm.RoleId)
            {
                var role = await _repositoryWrapper.Role.Get(r => r.Id == updateUserForm.RoleId);
                if (role == null) return (null, "Role not found");
                user.RoleId = role.Id;
            }

            await _repositoryWrapper.User.UpdateUser(user);
            await _repositoryWrapper.Save();

            return (user, null);
        }




        public async Task<(UserDto? user, string? error)> GetUserById(Guid id)
        {
            var user = await _repositoryWrapper.User.Get(
                u => u.Id == id && !u.Deleted,
                include: query => query.Include(u => u.Role) // Include Role here
            );

            if (user == null) return (null, "User not found or deleted");

            var userDto = _mapper.Map<UserDto>(user);

            // Check if the user has the "Data Entry" role, and if so, add the StoreCount
            if (user.Role != null && user.Role.Name == "Data Entry")
            {
                userDto.StoreCount = user.StoreCount;
            }

            return (userDto, null);
        }


        public async Task<(IEnumerable<UserDto> users, int totalUsers, string? error)> GetAllUsers(int pageNumber, int pageSize)
        {
            
            var users = await _repositoryWrapper.User.GetAll(include: query => query.Include(role => role.Role));

           
            var activeUsers = users.data.Where(u => !u.Deleted);

            
            var totalUsers = activeUsers.Count();

          
            var pagedUsers = activeUsers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (!pagedUsers.Any())
            {
                return (null, totalUsers, "No users found");
            }

            
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(pagedUsers);

           
            //foreach (var userDto in userDtos)
            //{
              //  var user = pagedUsers.First(u => u.Id == userDto.Id); 
                ///if (user.Role.Name == "DataEntry")
               /// {
                  //  userDto.StoreCount = user.StoreCount; 
               // }
            //}

            return (userDtos, totalUsers, null);
        }

        public async Task<(UserDto? user, string? error)> ChangePassword(ChangePasswordForm changePasswordForm, Guid id)
        {
            var user = await _repositoryWrapper.User.Get(u => u.Id == id && !u.Deleted);
            if (user == null) return (null, "User not found or deleted");

            user.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordForm.NewPassword);

            await _repositoryWrapper.User.UpdateUser(user);
            await _repositoryWrapper.Save();

            var userDto = _mapper.Map<UserDto>(user);

            return (userDto, null);
        }

    }
}
