namespace DiffManager.Integration.Tests
{
    using System.Net.Http;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;

    using Xunit;

    public class TestHostCollectionFixture : ICollectionFixture<CustomWebApplicationFactory>
    {
        public TestHostCollectionFixture()
        {
            var factory = new WebApplicationFactory<Startup>();
            
            this.Client = factory.CreateClient();

            this.Host = factory.Server?.Host;
        }

        public IWebHost Host { get; }

        public HttpClient Client { get; }
    }
}
