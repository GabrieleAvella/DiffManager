namespace DiffManager.Integration.Tests.DiffsController
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Text;
    using System.Threading.Tasks;

    using DiffManager.DataContracts;
    using DiffManager.Domains.Contexts;
    using DiffManager.Models;

    using Microsoft.Extensions.DependencyInjection;

    using Xunit;

    [Collection("DiffsController integration tests collection")]
    public class DiffsControllerTests_SetLeftDiff
    {
        private const string BaseUrl = "v1/diff";

        private readonly TestHostCollectionFixture testHostCollectionFixture;

        public DiffsControllerTests_SetLeftDiff(TestHostCollectionFixture testHostCollectionFixture)
        {
            this.testHostCollectionFixture = testHostCollectionFixture;
        }

        [Fact]
        public async Task ShouldAddTheLeftDiff()
        {
            // Arrange
            var leftInput = Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes("Left data")));

            Guid id = Guid.NewGuid();

            // Act
            var requestContent = new ObjectContent<DiffItemDto>(
                new DiffItemDto { Value = leftInput },
                new JsonMediaTypeFormatter());

            var response =
                await this.testHostCollectionFixture.Client.PostAsync($"{BaseUrl}/{id}/left", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Difference addedDiff;
            using (var scope = this.testHostCollectionFixture.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DifferenceContext>();

                addedDiff = context.Differences.Find(id);
            }

            Assert.NotNull(addedDiff);
            Assert.Equal(Convert.ToBase64String(leftInput), Convert.ToBase64String(addedDiff.Left));
        }

        [Fact]
        public async Task ShouldUpdateTheLeftDiffWhenItExistsAlready()
        {
            // Arrange
            Guid id = Guid.NewGuid();

            var leftInput = Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes("Left data")));

            using (var scope = this.testHostCollectionFixture.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DifferenceContext>();

                Difference diff = new Difference { Id = id, Left = leftInput };

                context.Differences.Add(diff);
                context.SaveChanges();
            }

            // Act
            var newLeftInput =
                Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes("New left data")));
            var requestContent = new ObjectContent<DiffItemDto>(
                new DiffItemDto { Value = newLeftInput },
                new JsonMediaTypeFormatter());

            var response =
                await this.testHostCollectionFixture.Client.PostAsync($"{BaseUrl}/{id}/left", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Difference updatedDiff;
            using (var scope = this.testHostCollectionFixture.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DifferenceContext>();

                updatedDiff = context.Differences.Find(id);
            }

            var updatedDiffData = Convert.ToBase64String(updatedDiff.Left);

            Assert.NotNull(updatedDiff);
            Assert.NotEqual(Convert.ToBase64String(leftInput), updatedDiffData);
            Assert.Equal(Convert.ToBase64String(newLeftInput), updatedDiffData);
        }
    }
}
