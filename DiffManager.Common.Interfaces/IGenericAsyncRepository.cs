namespace DiffManager.Common.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The interface for the asynchronous generic repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that this repository handles.</typeparam>
    public interface IGenericAsyncRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Asynchronously retrieves an entity from the database.
        /// </summary>
        /// <param name="id">
        /// The ID of the entity.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="T:CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        Task<TEntity> RetrieveAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Inserts an entity into the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// Updates an entity from the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Delete(TEntity entity);
    }
}