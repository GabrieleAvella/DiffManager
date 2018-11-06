namespace DiffManager.Services.Unit.Tests.DiffServiceTests
{
    using System;

    using DiffManager.Common;
    using DiffManager.Domains.Contexts;
    using DiffManager.Domains.Repositories;
    using DiffManager.Domains.UnitOfWorks;
    using DiffManager.Models;
    using DiffManager.Services.DataContracts;

    using Microsoft.EntityFrameworkCore;

    using Xunit;

    using DiffService = Services.DiffService;

    [Collection("DiffService collection")]
    public class DiffService_CreateAsync_Should
    {
        private readonly DiffService diffService;

        private readonly DbContextOptions<DifferenceContext> contextOptions;

        public DiffService_CreateAsync_Should()
        {
            this.contextOptions = new DbContextOptionsBuilder<DifferenceContext>()
                .UseInMemoryDatabase("InMemoryDbForTesting").Options;
            var context = new DifferenceContext(this.contextOptions);

            DifferenceAsyncRepository differenceAsyncRepository = new DifferenceAsyncRepository(context);
            DifferenceUnitOfWork differenceUnitOfWork = new DifferenceUnitOfWork(context, differenceAsyncRepository);

            var diffsFinder = new DiffsFinder();

            this.diffService = new DiffService(diffsFinder, differenceUnitOfWork);
        }

        [Fact]
        public async void CreateTheDifferenceResource()
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
            DifferenceDto createdDiff = await this.diffService.CreateAsync(diffToCreate);

            // Assert
            Assert.NotNull(createdDiff);

            Difference difference;
            using (var differenceContext = new DifferenceContext(this.contextOptions))
            {
                difference = differenceContext.Differences.Find(createdDiff.Id);
            }
            
            Assert.Equal(
                "LF data",
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToBase64String(difference.Left))));
            Assert.Equal(
                "RT data",
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToBase64String(difference.Right))));
        }
    }
}
