namespace DiffManager.Domains.UnitOfWorks
{
    using System.Threading;
    using System.Threading.Tasks;

    using DiffManager.Domains.RepositoryInterfaces;

    public interface IDifferenceUnitOfWork
    {
        /// <summary>
        /// Gets the Difference repository.
        /// </summary>
        IDifferenceAsyncRepository DifferenceAsyncRepository { get; }

        /// <summary>
        /// Saves all changes made to the context asynchronously.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="T:CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// Returns true if the save was successful.
        /// </returns>
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}