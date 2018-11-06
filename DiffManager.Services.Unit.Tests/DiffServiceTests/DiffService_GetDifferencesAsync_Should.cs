namespace DiffManager.Services.Unit.Tests.DiffServiceTests
{
    using System;

    using DiffManager.Common;
    using DiffManager.Common.Exceptions;
    using DiffManager.Domains.Contexts;
    using DiffManager.Domains.Repositories;
    using DiffManager.Domains.UnitOfWorks;
    using DiffManager.Models;

    using Microsoft.EntityFrameworkCore;

    using Xunit;

    using DiffService = Services.DiffService;

    [Collection("DiffService collection")]
    public class DiffService_GetDifferencesAsync_Should
    {
        private readonly DiffService diffService;

        private readonly DbContextOptions<DifferenceContext> contextOptions;

        public DiffService_GetDifferencesAsync_Should()
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
        public void ReturnDifferences()
        {
            // Arrange
            var leftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("LF data")));

            var rightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("RT data")));

            Guid id = Guid.NewGuid();

            using (var differenceContext = new DifferenceContext(this.contextOptions))
            {
                Difference diff = new Difference { Id = id, Left = leftInput, Right = rightInput };

                differenceContext.Differences.Add(diff);
                differenceContext.SaveChanges();
            }

            // Act
            var comparisonResult = this.diffService.GetDifferencesAsync(id).Result;

            // Assert
            Assert.Equal(DiffComparisonResultType.NotEqual, comparisonResult.Result);
            Assert.Single(comparisonResult.Differences);
            Assert.Collection(comparisonResult.Differences, pair => Assert.True(pair.Key == 0 && pair.Value == 2));
        }

        [Fact]
        public void ReturnNoDifferences()
        {
            // Arrange
            var leftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("same data")));

            var rightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("same data")));

            Guid id = Guid.NewGuid();

            using (var differenceContext = new DifferenceContext(this.contextOptions))
            {
                Difference diff = new Difference { Id = id, Left = leftInput, Right = rightInput };

                differenceContext.Differences.Add(diff);
                differenceContext.SaveChanges();
            }

            // Act
            var comparisonResult = this.diffService.GetDifferencesAsync(id).Result;

            // Assert
            Assert.Equal(DiffComparisonResultType.Equal, comparisonResult.Result);
        }

        [Fact]
        public void ReturnLengthNotEqual()
        {
            // Arrange
            var leftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("some data")));

            var rightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("some other data")));

            Guid id = Guid.NewGuid();

            using (var differenceContext = new DifferenceContext(this.contextOptions))
            {
                Difference diff = new Difference { Id = id, Left = leftInput, Right = rightInput };

                differenceContext.Differences.Add(diff);
                differenceContext.SaveChanges();
            }

            // Act
            var comparisonResult = this.diffService.GetDifferencesAsync(id).Result;

            // Assert
            Assert.Equal(DiffComparisonResultType.LengthNotEqual, comparisonResult.Result);
        }

        [Fact]
        public async System.Threading.Tasks.Task ReturnResourceNotFound()
        {
            // Arrange
            Guid id = Guid.NewGuid();

            // Act
            Exception ex =
                await Assert.ThrowsAsync<ResourceNotFoundException>(() => this.diffService.GetDifferencesAsync(id));

            // Assert
            Assert.Equal("No difference was found with the specified ID", ex.Message);
        }
    }
}
