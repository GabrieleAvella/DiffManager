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
    public class DiffsControllerTests_SetRightDiff
    {
        private const string BaseUrl = "v1/diff";

        private readonly TestHostCollectionFixture testHostCollectionFixture;

        public DiffsControllerTests_SetRightDiff(TestHostCollectionFixture testHostCollectionFixture)
        {
            this.testHostCollectionFixture = testHostCollectionFixture;
        }

        [Fact]
        public async Task ShouldAddTheRightDiff()
        {
            // Arrange
            var rightInput = Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes("Right data")));

            Guid id = Guid.NewGuid();

            // Act
            var requestContent = new ObjectContent<DiffItemDto>(
                new DiffItemDto { Value = rightInput },
                new JsonMediaTypeFormatter());

            var response =
                await this.testHostCollectionFixture.Client.PostAsync($"{BaseUrl}/{id}/right", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Difference addedDiff;
            using (var scope = this.testHostCollectionFixture.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DifferenceContext>();

                addedDiff = context.Differences.Find(id);
            }

            Assert.NotNull(addedDiff);
            Assert.Equal(Convert.ToBase64String(rightInput), Convert.ToBase64String(addedDiff.Right));
        }

        [Fact]
        public async Task ShouldUpdateTheRightDiffWhenItExistsAlready()
        {
            // Arrange
            Guid id = Guid.NewGuid();

            var rightInput = Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes("Right data")));

            using (var scope = this.testHostCollectionFixture.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DifferenceContext>();

                Difference diff = new Difference { Id = id, Right = rightInput };

                context.Differences.Add(diff);
                context.SaveChanges();
            }

            // Act
            var newRightInput =
                Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes("New right data")));
            var requestContent = new ObjectContent<DiffItemDto>(
                new DiffItemDto { Value = newRightInput },
                new JsonMediaTypeFormatter());

            var response =
                await this.testHostCollectionFixture.Client.PostAsync($"{BaseUrl}/{id}/right", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Difference updatedDiff;
            using (var scope = this.testHostCollectionFixture.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DifferenceContext>();

                updatedDiff = context.Differences.Find(id);
            }

            var updatedDiffData = Convert.ToBase64String(updatedDiff.Right);

            Assert.NotNull(updatedDiff);
            Assert.NotEqual(Convert.ToBase64String(rightInput), updatedDiffData);
            Assert.Equal(Convert.ToBase64String(newRightInput), updatedDiffData);
        }
    }
}
