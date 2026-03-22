using System.Security.Cryptography;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UserService.Db;
using UserService.Repo;
using UserService.Services;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddDbContext<UserContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(container =>
            {
                container.RegisterType<UserRepository>().As<Interfaces.IUserRepo>();
                container.RegisterType<JwtTokenService>().As<Interfaces.IJwtTokenService>();
                container.RegisterType<Models.AuthMock>().As<Interfaces.IUserAuthService>();
            });

            var rsaPublicPath = builder.Configuration["Jwt:RsaPublicKeyPath"] ?? "rsa/public_key.pem";
            if (!Path.IsPathRooted(rsaPublicPath))
            {
                var baseDir = Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? AppContext.BaseDirectory;
                rsaPublicPath = Path.Combine(baseDir, rsaPublicPath.Replace('/', Path.DirectorySeparatorChar));
            }
            var pemContent = File.ReadAllText(rsaPublicPath, System.Text.Encoding.UTF8);
            if (pemContent.Length > 0 && pemContent[0] == '\uFEFF')
                pemContent = pemContent[1..];
            pemContent = pemContent.Trim();
            if (!pemContent.StartsWith("-----BEGIN", StringComparison.Ordinal))
                throw new InvalidOperationException($"JWT RSA public key file does not contain valid PEM: {rsaPublicPath}");
            var publicKey = RSA.Create();
            publicKey.ImportFromPem(pemContent);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new RsaSecurityKey(publicKey)
                };
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
