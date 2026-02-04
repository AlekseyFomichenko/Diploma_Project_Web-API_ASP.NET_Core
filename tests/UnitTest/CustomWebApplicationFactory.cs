using MessageService;
using MessageService.Db;
using MessageService.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTest
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public CustomWebApplicationFactory()
        {
            var rsaPath = Path.Combine(AppContext.BaseDirectory, "rsa", "public_key.pem");
            if (File.Exists(rsaPath))
                Environment.SetEnvironmentVariable("Jwt__RsaPublicKeyPath", Path.GetFullPath(rsaPath));
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var messageServiceAssemblyDir = Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? "";
            var repoRoot = Path.GetFullPath(Path.Combine(messageServiceAssemblyDir, "..", "..", "..", "..", ".."));
            var messageServicePath = Path.Combine(repoRoot, "src", "MessageService");
            if (!Directory.Exists(messageServicePath))
                messageServicePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "src", "MessageService"));
            builder.UseContentRoot(messageServicePath);
            builder.ConfigureServices(services =>
            {
                for (var i = services.Count - 1; i >= 0; i--)
                {
                    var d = services[i];
                    if (d.ServiceType == typeof(MessageContext) || d.ServiceType == typeof(DbContextOptions<MessageContext>))
                        services.RemoveAt(i);
                }
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                services.AddSingleton(connection);
                services.AddDbContext<MessageContext>(options => options.UseSqlite(connection));
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMessageRepo));
                if (descriptor != null)
                    services.Remove(descriptor);
                services.AddScoped<IMessageRepo, MockMessageRepo>();
            });
        }
    }
}
