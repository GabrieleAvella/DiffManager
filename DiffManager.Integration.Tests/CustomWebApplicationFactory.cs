namespace DiffManager.Integration.Tests
{
    using DiffManager.Domains.Contexts;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(
                services =>
                    {
                        // Create a new service provider.
                        var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase()
                            .BuildServiceProvider();

                        // Add a database context (ApplicationDbContext) using an in-memory 
                        // database for testing.
                        services.AddDbContext<DifferenceContext>(
                            options =>
                                {
                                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                                    options.UseInternalServiceProvider(serviceProvider);
                                });
                    });
        }
    }
}
