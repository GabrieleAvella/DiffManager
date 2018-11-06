namespace DiffManager.Common.Unit.Tests
{
    using System;
    using System.Linq;

    using DiffManager.Common.Exceptions;

    using Xunit;

    public class DiffsFinder_Find_Should
    {
        private readonly DiffsFinder diffsFinder;

        public DiffsFinder_Find_Should()
        {
            this.diffsFinder = new DiffsFinder();
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("the same data", "the same data")]
        public void ReturnNoDifferencesGivenTheSameValuesOrNull(string left, string right)
        {
            var leftInput = left == null
                                ? null
                                : Convert.FromBase64String(
                                    Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(left)));

            var rightInput = right == null
                                ? null
                                : Convert.FromBase64String(
                                    Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(right)));

            var result = this.diffsFinder.Find(leftInput, rightInput);

            Assert.False(result.Any());
        }

        [Theory]
        [InlineData("some data", null)]
        [InlineData(null, "some data")]
        [InlineData("some data", "some other data")]
        public void ReturnNotTheSameLength(string left, string right)
        {
            var leftInput = left == null    
                                ? null
                                : Convert.FromBase64String(
                                    Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(left)));

            var rightInput = right == null
                                 ? null
                                 : Convert.FromBase64String(
                                     Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(right)));

            Exception ex =
                Assert.Throws<DiffsDataLengthNotEqualException>(() => this.diffsFinder.Find(leftInput, rightInput));

            Assert.Equal("The length of left and right must be the same.", ex.Message);
        }

        [Fact]
        public void ReturnOneDifference()
        {
            var leftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("LF data")));

            var rightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("RT data")));

            var result = this.diffsFinder.Find(leftInput, rightInput);

            Assert.Single(result);
            Assert.Collection(result, pair => Assert.True(pair.Key == 0 && pair.Value == 2));
        }

        [Fact]
        public void ReturnManyDifferences()
        {
            var leftInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("LF data L data LF")));

            var rightInput =
                Convert.FromBase64String(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("RT data R data RT")));

            var result = this.diffsFinder.Find(leftInput, rightInput);

            Assert.Equal(3, result.Count);
            Assert.Collection(
                result,
                pair => Assert.True(pair.Key == 0 && pair.Value == 2),
                pair => Assert.True(pair.Key == 8 && pair.Value == 1),
                pair => Assert.True(pair.Key == 15 && pair.Value == 2));
        }
    }
}
