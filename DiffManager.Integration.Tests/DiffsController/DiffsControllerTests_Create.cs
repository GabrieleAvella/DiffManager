namespace DiffManager.Integration.Tests.DiffsController
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;

    using DiffManager.DataContracts;

    using Xunit;

    [Collection("DiffsController integration tests collection")]
    public class DiffsControllerTests_Create
    {
        private const string BaseUrl = "v1/diff";

        private readonly TestHostCollectionFixture testHostCollectionFixture;

        public DiffsControllerTests_Create(TestHostCollectionFixture testHostCollectionFixture)
        {
            this.testHostCollectionFixture = testHostCollectionFixture;
        }

        [Fact]
        public async Task ShouldCreateTheDifferenceResource()
        {
            // Arrange
            var leftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("LF data")));

            var rightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("RT data")));

            // Act
            var diffToCreate = new DifferenceForCreationDto
                                   {
                                       Left = leftInput,
                                       Right = rightInput
                                   };
            var requestContent =
                new ObjectContent<DifferenceForCreationDto>(diffToCreate, new JsonMediaTypeFormatter());

            var response =
                await this.testHostCollectionFixture.Client.PostAsync($"{BaseUrl}", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = response.Content.ReadAsAsync<DifferenceDto>().Result;
            Assert.NotNull(content);

            Assert.Equal(Convert.ToBase64String(leftInput), Convert.ToBase64String(content.Left));

            Assert.Equal(Convert.ToBase64String(rightInput), Convert.ToBase64String(content.Right));
        }
    }
}
