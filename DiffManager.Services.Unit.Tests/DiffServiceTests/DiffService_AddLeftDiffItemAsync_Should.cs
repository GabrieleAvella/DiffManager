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
    public class DiffService_AddLeftDiffItemAsync_Should
    {
        private readonly DiffService diffService;

        private readonly DbContextOptions<DifferenceContext> contextOptions;

        public DiffService_AddLeftDiffItemAsync_Should()
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
        public async void AddTheLeftDiffData()
        {
            // Arrange
            var leftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Left data")));

            Guid id = Guid.NewGuid();

            // Act
            await this.diffService.AddLeftDiffItemAsync(id, leftInput);

            // Assert
            Difference difference;
            using (var differenceContext = new DifferenceContext(this.contextOptions))
            {
                difference = differenceContext.Differences.Find(id);
            }

            Assert.NotNull(difference);
            Assert.Equal(
                "Left data",
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToBase64String(difference.Left))));
        }

        [Fact]
        public async void UpdateTheLeftDiffDataGivenAnExistingId()
        {
            // Arrange
            var leftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Left data")));

            Guid id = Guid.NewGuid();

            using (var differenceContext = new DifferenceContext(this.contextOptions))
            {
                Difference diff = new Difference { Id = id, Left = leftInput };

                differenceContext.Differences.Add(diff);
                differenceContext.SaveChanges();
            }

            // Act
            var newLeftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("New left data")));
            await this.diffService.AddLeftDiffItemAsync(id, newLeftInput);

            // Assert
            Difference difference;
            using (var differenceContext = new DifferenceContext(this.contextOptions))
            {
                difference = differenceContext.Differences.Find(id);
            }

            Assert.NotNull(difference);
            Assert.Equal(
                "New left data",
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToBase64String(difference.Left))));
        }
    }
}
