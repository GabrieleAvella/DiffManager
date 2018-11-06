namespace DiffManager.Services.Unit.Tests.DiffServiceTests
{
    using System;

    using DiffManager.Common;
    using DiffManager.Domains.Contexts;
    using DiffManager.Domains.Repositories;
    using DiffManager.Domains.UnitOfWorks;
    using DiffManager.Models;

    using Microsoft.EntityFrameworkCore;

    using Xunit;

    using DiffService = Services.DiffService;

    [Collection("DiffService collection")]
    public class DiffService_AddRightDiffItemAsync_Should
    {
        private readonly DiffService diffService;

        private readonly DbContextOptions<DifferenceContext> contextOptions;

        public DiffService_AddRightDiffItemAsync_Should()
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
        public async void AddTheRightDiffData()
        {
            // Arrange
            var rightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Right data")));

            Guid id = Guid.NewGuid();

            // Act
            await this.diffService.AddRightDiffItemAsync(id, rightInput);

            // Assert
            Difference difference;
            using (var differenceContext = new DifferenceContext(this.contextOptions))
            {
                difference = differenceContext.Differences.Find(id);
            }

            Assert.NotNull(difference);
            Assert.Equal(
                "Right data",
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToBase64String(difference.Right))));
        }

        [Fact]
        public async void UpdateTheRightDiffDataGivenAnExistingId()
        {
            // Arrange
            var rightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Right data")));

            Guid id = Guid.NewGuid();

            using (var differenceContext = new DifferenceContext(this.contextOptions))
            {
                Difference diff = new Difference { Id = id, Right = rightInput };

                differenceContext.Differences.Add(diff);
                differenceContext.SaveChanges();
            }

            // Act
            var newRightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("New right data")));
            await this.diffService.AddRightDiffItemAsync(id, newRightInput);

            // Assert
            // Assert
            Difference difference;
            using (var differenceContext = new DifferenceContext(this.contextOptions))
            {
                difference = differenceContext.Differences.Find(id);
            }

            Assert.NotNull(difference);
            Assert.Equal(
                "New right data",
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToBase64String(difference.Right))));
        }
    }
}
