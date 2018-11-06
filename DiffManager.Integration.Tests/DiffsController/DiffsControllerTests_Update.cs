namespace DiffManager.Integration.Tests.DiffsController
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;

    using DiffManager.DataContracts;
    using DiffManager.Domains.Contexts;
    using DiffManager.Models;

    using Microsoft.Extensions.DependencyInjection;

    using Xunit;

    [Collection("DiffsController integration tests collection")]
    public class DiffsControllerTests_Update
    {
        private const string BaseUrl = "v1/diff";

        private readonly TestHostCollectionFixture testHostCollectionFixture;

        public DiffsControllerTests_Update(TestHostCollectionFixture testHostCollectionFixture)
        {
            this.testHostCollectionFixture = testHostCollectionFixture;
        }

        [Fact]
        public async Task ShouldReturn404IfTheDifferenceDoenNotExist()
        {
            // Arrange
            var leftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("LF data")));

            var rightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("RT data")));

            Guid id = Guid.NewGuid();

            // Act
            var diffToUpdate = new DifferenceForUpdateDto { Left = leftInput, Right = rightInput };
            var requestContent = new ObjectContent<DifferenceForUpdateDto>(diffToUpdate, new JsonMediaTypeFormatter());

            var response = await this.testHostCollectionFixture.Client.PutAsync($"{BaseUrl}/{id}", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("new left data", null)]
        [InlineData(null, "new right data")]
        [InlineData("new left data", "new right data")]
        public async Task ShouldUpdateTheDifferenceData(string left, string right)
        {
            // Arrange
            var leftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("LF data")));

            var rightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("RT data")));

            Guid id = Guid.NewGuid();

            using (var scope = this.testHostCollectionFixture.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DifferenceContext>();

                Difference diff = new Difference { Id = id, Left = leftInput, Right = rightInput };

                context.Differences.Add(diff);
                context.SaveChanges();
            }

            // Act  
            var newLeftInput = left == null
                                   ? null
                                   : Convert.FromBase64String(
                                       Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(left)));

            var newRightInput = right == null
                                    ? null
                                    : Convert.FromBase64String(
                                        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(right)));

            var diffToUpdate = new DifferenceForUpdateDto { Left = newLeftInput, Right = newRightInput };

            var requestContent = new ObjectContent<DifferenceForUpdateDto>(diffToUpdate, new JsonMediaTypeFormatter());

            var response = await this.testHostCollectionFixture.Client.PutAsync($"{BaseUrl}/{id}", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            Difference updatedDiff;
            using (var scope = this.testHostCollectionFixture.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DifferenceContext>();

                updatedDiff = context.Differences.Find(id);
            }

            var updatedLeftDiffData = Convert.ToBase64String(updatedDiff.Left);
            var updatedRightDiffData = Convert.ToBase64String(updatedDiff.Right);

            Assert.NotNull(updatedDiff);
            Assert.NotEqual(Convert.ToBase64String(leftInput), updatedLeftDiffData);
            Assert.Equal(
                newLeftInput != null ? Convert.ToBase64String(newLeftInput) : string.Empty,
                updatedLeftDiffData);
            Assert.NotEqual(Convert.ToBase64String(rightInput), updatedRightDiffData);
            Assert.Equal(
                newRightInput != null ? Convert.ToBase64String(newRightInput) : string.Empty,
                updatedRightDiffData);
        }
    }
}
