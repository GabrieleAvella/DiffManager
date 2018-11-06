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
    public class DiffService_UpdateAsync_Should
    {
        private readonly DiffService diffService;

        private readonly DbContextOptions<DifferenceContext> contextOptions;

        public DiffService_UpdateAsync_Should()
        {
            this.contextOptions = new DbContextOptionsBuilder<DifferenceContext>()
                .UseInMemoryDatabase("InMemoryDbForTesting").Options;
            var context = new DifferenceContext(this.contextOptions);

            DifferenceAsyncRepository differenceAsyncRepository = new DifferenceAsyncRepository(context);
            DifferenceUnitOfWork differenceUnitOfWork = new DifferenceUnitOfWork(context, differenceAsyncRepository);

            var diffsFinder = new DiffsFinder();

            this.diffService = new DiffService(diffsFinder, differenceUnitOfWork);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("new left data", null)]
        [InlineData(null, "new right data")]
        [InlineData("new left data", "new right data")]
        public async void UpdateTheDiffDataOfAnExistingResource(string left, string right)
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
            var newLeftInput = left == null
                                   ? null
                                   : Convert.FromBase64String(
                                       Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(left)));

            var newRightInput = right == null
                                   ? null
                                   : Convert.FromBase64String(
                                       Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(right)));

            var diffToUpdate = new DifferenceForUpdateDto { Left = newLeftInput, Right = newRightInput };

            await this.diffService.UpdateAsync(id, diffToUpdate);

            // Assert
            Difference difference;
            using (var differenceContext = new DifferenceContext(this.contextOptions))
            {
                difference = differenceContext.Differences.Find(id);
            }
            
            Assert.NotEqual(
                "LF data",
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToBase64String(difference.Left))));
            Assert.NotEqual(
                "RT data",
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Convert.ToBase64String(difference.Right))));
        }
    }
}
