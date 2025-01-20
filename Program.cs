using InfoPoster_backend.Extensions;
using InfoPoster_backend.Middlewares;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Services.Selectel_API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System.Security.Cryptography;

namespace InfoPoster_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            var connection = builder.Configuration["ConnectionStrings:DefaultConnection"];
            builder.Services.AddScoped<IJWTService, JWTService>();
            builder.Services.AddScoped<LoginService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddHttpClient<SelectelAuthService>();

            builder.Services.AddDbContext<PostersContext>(opt => opt.UseMySql(connection, ServerVersion.AutoDetect(connection)));
            builder.Services.AddDbContext<AccountContext>(opt => opt.UseMySql(connection, ServerVersion.AutoDetect(connection)));
            builder.Services.AddDbContext<OrganizationContext>(opt => opt.UseMySql(connection, ServerVersion.AutoDetect(connection)));
            builder.Services.AddDbContext<OfferContext>(opt => opt.UseMySql(connection, ServerVersion.AutoDetect(connection)));
            builder.Services.AddDbContext<ArticleContext>(opt => opt.UseMySql(connection, ServerVersion.AutoDetect(connection)));

            builder.Services.AddScoped<CategoryRepository>();
            builder.Services.AddScoped<AccountRepository>();
            builder.Services.AddScoped<PosterRepository>();
            builder.Services.AddScoped<OrganizationRepository>();
            builder.Services.AddScoped<FileRepository>();
            builder.Services.AddScoped<StatisticRepository>();
            builder.Services.AddScoped<OfferRepository>();
            builder.Services.AddScoped<ArticleRepository>();

            builder.Services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });

            builder.Services.AddSingleton<RsaSecurityKey>(provider =>
            {

                RSA rsa = RSA.Create();
                rsa.ImportRSAPublicKey(
                    source: Convert.FromBase64String(builder.Configuration["Protection:JwtPublic"]),
                    bytesRead: out int _
                );

                return new RsaSecurityKey(rsa);
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Asymmetric", options =>
            {
                SecurityKey rsa = builder.Services.BuildServiceProvider().GetRequiredService<RsaSecurityKey>();
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    RequireSignedTokens = true,
                    ValidAudience = builder.Configuration["Protection:JwtAudience"],
                    ValidIssuer = builder.Configuration["Protection:JwtIssuer"],
                    IssuerSigningKey = rsa
                };
            });

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddMediatR(cfg =>
            {
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
            app.ConfigureExceptionHandler();
            app.UseHttpsRedirection();

            app.Use((context, next) =>
            {
                var token = context.Request.Cookies["AccessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + token);
                }

                return next();
            });

            app.UseStaticFiles();
            app.UseRouting();

            if (!app.Environment.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "wwwroot";

                //if (app.Environment.IsDevelopment())
                //{
                //    spa.UseAngularCliServer(npmScript: "start");
                //}
            });

            app.UseAuthorization();
            app.UseMiddleware<DefaultLangMiddleware>();

            app.MapControllers();
            //app.MapGet("/", () => "Hello world");

            app.Run();
        }
    }
}
