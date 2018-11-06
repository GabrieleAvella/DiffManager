namespace DiffManager.Services.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using DiffManager.Services.DataContracts;

    public interface IDiffService
    {
        /// <summary>
        /// Adds the left difference data asynchronously.
        /// </summary>
        /// <param name="id">The ID</param>
        /// <param name="item">The data of the difference</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Returns a <see cref="Task"/></returns>
        Task AddLeftDiffItemAsync(
            Guid id,
            byte[] item,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds the right difference data asynchronously.
        /// </summary>
        /// <param name="id">The ID</param>
        /// <param name="item">The data of the difference</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Returns a <see cref="Task"/></returns>
        Task AddRightDiffItemAsync(
            Guid id,
            byte[] item,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieves the differences between left and right data asynchronously.
        /// </summary>
        /// <param name="id">The ID</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Returns the result of the comparison and the differences if any.</returns>
        Task<ComparisonResult> GetDifferencesAsync(
            Guid id,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates the specified difference asynchronously.
        /// </summary>
        /// <param name="difference">The difference to save.</param>
        /// <param name="cancellationToken">
        /// A <see cref="T:CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>The saved difference.</returns>
        Task<DifferenceDto> CreateAsync(
            DifferenceForCreationDto difference,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Fully updates a difference asynchronously.
        /// </summary>
        /// <param name="id">The ID of the difference to update.</param>
        /// <param name="difference">The difference to update to.</param>
        /// <param name="cancellationToken">
        /// A <see cref="T:CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateAsync(Guid id, DifferenceForUpdateDto difference, CancellationToken cancellationToken = default(CancellationToken));
    }
}