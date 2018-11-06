namespace DiffManager.Integration.Tests.DiffsController
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using DiffManager.Domains.Contexts;
    using DiffManager.Models;

    using Microsoft.Extensions.DependencyInjection;

    using Xunit;

    [Collection("DiffsController integration tests collection")]
    public class DiffsControllerTests_GetDifferences
    {
        private const string BaseUrl = "v1/diff";

        private readonly TestHostCollectionFixture testHostCollectionFixture;

        public DiffsControllerTests_GetDifferences(TestHostCollectionFixture testHostCollectionFixture)
        {
            this.testHostCollectionFixture = testHostCollectionFixture;
        }

        [Fact]
        public async Task ShouldReturn404IfTheResourceDoesNotExist()
        {
            // Arrange  
            Guid id = Guid.NewGuid();

            // Act
            var response = await this.testHostCollectionFixture.Client.GetAsync($"{BaseUrl}/{id}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("some data", null)]
        [InlineData(null, "some data")]
        [InlineData("some data", "some other data")]
        public async Task ShouldReturnLengthIsNotTheSame(string left, string right)
        {
            // Arrange
            var leftInput = left == null
                                ? null
                                : Convert.FromBase64String(
                                    Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(left)));

            var rightInput = right == null
                                 ? null
                                 : Convert.FromBase64String(
                                     Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(right)));

            Guid id = Guid.NewGuid();

            using (var scope = this.testHostCollectionFixture.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DifferenceContext>();

                Difference diff = new Difference { Id = id, Left = leftInput, Right = rightInput };

                context.Differences.Add(diff);
                context.SaveChanges();
            }

            // Act
            var response = await this.testHostCollectionFixture.Client.GetAsync($"{BaseUrl}/{id}");
            var content = response.Content.ReadAsAsync<ExpandoObject>().Result as IDictionary<string, object>;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("The length is not the same", content["result"]);
        }

        [Fact]
        public async Task ShouldReturnNoDifferences()
        {
            // Arrange
            var leftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("same data")));

            var rightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("same data")));

            Guid id = Guid.NewGuid();

            using (var scope = this.testHostCollectionFixture.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DifferenceContext>();

                Difference diff = new Difference { Id = id, Left = leftInput, Right = rightInput };

                context.Differences.Add(diff);
                context.SaveChanges();
            }

            // Act
            var response = await this.testHostCollectionFixture.Client.GetAsync($"{BaseUrl}/{id}");
            var content = response.Content.ReadAsAsync<ExpandoObject>().Result as IDictionary<string, object>;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("No differences", content["result"]);
        }

        [Theory]
        [InlineData("LF data", "RT data")]
        [InlineData("LF data L data LF", "RT data R data RT")]
        public async Task ShouldReturnDifferences(string left, string right)
        {
            // Arrange
            var leftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(left)));

            var rightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(right)));

            Guid id = Guid.NewGuid();

            using (var scope = this.testHostCollectionFixture.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DifferenceContext>();

                Difference diff = new Difference { Id = id, Left = leftInput, Right = rightInput };

                context.Differences.Add(diff);
                context.SaveChanges();
            }

            // Act
            var response = await this.testHostCollectionFixture.Client.GetAsync($"{BaseUrl}/{id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = response.Content.ReadAsAsync<ExpandoObject>().Result as IDictionary<string, object>;
            Assert.True((content["result"] as IList<object>)?.Any());
        }
    }
}
