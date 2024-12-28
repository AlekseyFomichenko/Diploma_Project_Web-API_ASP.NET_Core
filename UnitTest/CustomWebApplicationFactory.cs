using MessageManager;
using MessageService.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTest
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Здесь можно регистрировать моки для зависимостей
                var serviceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMessageRepo));
                if (serviceDescriptor != null)
                {
                    services.Remove(serviceDescriptor);
                }
                services.AddScoped<IMessageRepo, MockMessageRepo>();
            });
        }
    }
}
