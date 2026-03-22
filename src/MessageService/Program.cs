using System.Security.Cryptography;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MessageService.Db;
using MessageService.Repo;
using MessageService.Interfaces;
using MessageService;

namespace MessageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
builder.Services.AddSingleton<IMapper>(mapperConfig.CreateMapper());
            builder.Services.AddDbContext<MessageContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(container =>
            {
                container.RegisterType<MessageRepo>().As<IMessageRepo>();
            });

            var publicKey = RSA.Create();
            var pemContent = builder.Configuration["Jwt:RsaPublicKeyPem"];
            if (string.IsNullOrEmpty(pemContent))
            {
                var rsaPublicPath = builder.Configuration["Jwt:RsaPublicKeyPath"] ?? "rsa/public_key.pem";
                if (!Path.IsPathRooted(rsaPublicPath))
                {
                    var asmDir = Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? AppContext.BaseDirectory;
                    rsaPublicPath = Path.Combine(asmDir, rsaPublicPath.Replace('/', Path.DirectorySeparatorChar));
                }
                pemContent = File.ReadAllText(rsaPublicPath, System.Text.Encoding.UTF8);
            }
            if (!string.IsNullOrEmpty(pemContent))
            {
                if (pemContent.Length >= 3 && pemContent[0] == '\uFEFF')
                    pemContent = pemContent[1..];
                pemContent = pemContent.Trim();
            }
            if (!string.IsNullOrEmpty(pemContent) && pemContent.StartsWith("-----BEGIN", StringComparison.Ordinal))
                publicKey.ImportFromPem(pemContent);
            else
                throw new InvalidOperationException("JWT RSA public key could not be loaded. Set Jwt:RsaPublicKeyPath or Jwt:RsaPublicKeyPem.");
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
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MessageContext>();
                db.Database.Migrate();
            }
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
