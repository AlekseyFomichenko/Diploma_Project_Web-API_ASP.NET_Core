using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MessageService.Db
{
    public sealed class MessageContextFactory : IDesignTimeDbContextFactory<MessageContext>
    {
        public MessageContext CreateDbContext(string[] args)
        {
            var basePath = ResolveProjectPath();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not set.");

            var optionsBuilder = new DbContextOptionsBuilder<MessageContext>();
            optionsBuilder.UseNpgsql(connectionString);
            return new MessageContext(optionsBuilder.Options);
        }

        private static string ResolveProjectPath()
        {
            var currentDir = Directory.GetCurrentDirectory();
            if (File.Exists(Path.Combine(currentDir, "appsettings.json")))
                return currentDir;
            var underSrc = Path.Combine(currentDir, "src", "MessageService");
            if (File.Exists(Path.Combine(underSrc, "appsettings.json")))
                return underSrc;
            var dir = Path.GetDirectoryName(typeof(MessageContextFactory).Assembly.Location) ?? currentDir;
            for (var i = 0; i < 6; i++)
            {
                if (string.IsNullOrEmpty(dir))
                    break;
                if (File.Exists(Path.Combine(dir, "appsettings.json")))
                    return dir;
                dir = Path.GetDirectoryName(dir);
            }
            throw new InvalidOperationException("Could not locate appsettings.json for design-time configuration.");
        }
    }
}
