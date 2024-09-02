using InfoPoster_backend.Middlewares;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connection = builder.Configuration["ConnectionStrings:DefaultConnection"];

            builder.Services.AddDbContext<PostersContext>(opt => opt.UseMySql(connection, ServerVersion.AutoDetect(connection)));
            builder.Services.AddScoped<CategoryRepository>();
            builder.Services.AddHttpClient<PosterRepository>();

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseMiddleware<DefaultLangMiddleware>();


            app.MapControllers();

            app.Run();
        }
    }
}
