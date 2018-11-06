namespace DiffManager.Domains.Repositories
{
    using System;

    using DiffManager.Common;
    using DiffManager.Domains.Contexts;
    using DiffManager.Domains.RepositoryInterfaces;
    using DiffManager.Models;

    /// <summary>
    /// The Difference repository.
    /// </summary>
    public class DifferenceAsyncRepository : GenericAsyncRepository<Difference>, IDifferenceAsyncRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DifferenceAsyncRepository"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public DifferenceAsyncRepository(DifferenceContext context)
            : base(context)
        {
        }

        /// <inheritdoc />
        public override Difference Insert(Difference entity)
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            return base.Insert(entity);
        }
    }
}
