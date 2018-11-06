namespace DiffManager.Common
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using DiffManager.Common.Interfaces;

    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The asynchronous generic repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that this repository handles.</typeparam>
    public abstract class GenericAsyncRepository<TEntity> : IGenericAsyncRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericAsyncRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected GenericAsyncRepository(DbContext context)
        {
            this.Context = context;
            this.DbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        protected DbContext Context { get; }

        /// <summary>
        /// Gets the db set.
        /// </summary>
        protected DbSet<TEntity> DbSet { get; }

        /// <inheritdoc />
        public virtual async Task<TEntity> RetrieveAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var keyValues = new object[] { id };

            return await this.DbSet.FindAsync(keyValues, cancellationToken);
        }

        /// <inheritdoc />
        public virtual TEntity Insert(TEntity entity)
        {
            return this.DbSet.Add(entity)?.Entity;
        }

        /// <inheritdoc />
        public virtual TEntity Update(TEntity entity)
        {
            return this.DbSet.Update(entity)?.Entity;
        }

        /// <inheritdoc />
        public virtual void Delete(TEntity entity)
        {
            this.DbSet.Remove(entity);
        }
    }
}
