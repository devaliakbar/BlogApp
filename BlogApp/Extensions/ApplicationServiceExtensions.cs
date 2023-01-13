using BlogApp.Data;
using BlogApp.Services;
using BlogApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
             IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseNpgsql(config["DefaultConnection"]);
            });
            services.AddCors();

            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}