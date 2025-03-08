using System.Globalization;
using e_parliament.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EvaluationBackend.Helpers.OneSignal;
using EvaluationBackend.DATA;
using EvaluationBackend.Helpers;
using EvaluationBackend.Repository;
using EvaluationBackend.Services;
using EvaluationBackend.Entities;
using EvaluationBackend.Services;
using Microsoft.AspNetCore.Identity;   // Corrected name of the namespace

namespace EvaluationBackend.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(options => options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
            services.AddAutoMapper(typeof(UserMappingProfile).Assembly);
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IStoreService,StoreService>();
            services.AddScoped<AuthorizeActionFilter>();





            return services;
        }
    }
}