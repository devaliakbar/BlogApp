using BlogApp.Data;
using BlogApp.Services;
using BlogApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data.Repository;
using NLog;
using NLog.Extensions.Logging;
using NLog.Common;

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

            ///Setting Log with splunk
            LogManager.LoadConfiguration("nlog.xml");
            InternalLogger.LogToConsole = true;
            InternalLogger.LogLevel = NLog.LogLevel.Trace;
            services.AddLogging(l => l.AddNLog()
            ).Configure<LoggerFilterOptions>(c => c.MinLevel = Microsoft.Extensions.Logging.LogLevel.Trace);



            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBlogRepository, BlogRepository>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}