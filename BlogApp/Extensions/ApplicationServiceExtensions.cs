using BlogApp.Data;
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
                opt.UseNpgsql("Host=localhost;Port=5432;Database=blogapp;Username=username;Password=password");
            });
            services.AddCors();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}