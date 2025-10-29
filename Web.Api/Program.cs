using Microsoft.EntityFrameworkCore;
using NLog.Web;
using Web.Api.Dto.Response;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;

namespace Web.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add Connection String and DbContext
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Register DbContext with Singleton lifetime
            builder.Services.AddDbContext<TaskManagerAppDBContext>(options =>
                options.UseSqlServer(connectionString), ServiceLifetime.Singleton);

            // Register UnitOfWork as Singleton
            builder.Services.AddSingleton<UnitOfWork>();

            // Register ValidCheck as Singleton
            builder.Services.AddSingleton<ValidCheck>();

            // NLog: Setup NLog for Dependency injection
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            // Add Bind StatusChange settings from appsettings.json
            builder.Services.Configure<StatusChange>(builder.Configuration.GetSection("StatusSetting"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
