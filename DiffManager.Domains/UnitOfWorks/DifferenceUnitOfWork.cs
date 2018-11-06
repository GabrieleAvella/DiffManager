namespace DiffManager.Domains.UnitOfWorks
{
    using System.Threading;
    using System.Threading.Tasks;

    using DiffManager.Domains.Contexts;
    using DiffManager.Domains.RepositoryInterfaces;

    /// <summary>
    /// The Difference unit of work.
    /// </summary>
    public class DifferenceUnitOfWork : IDifferenceUnitOfWork
    {
        private readonly DifferenceContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DifferenceUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="differenceAsyncRepository">
        /// The Difference repository.
        /// </param>
        public DifferenceUnitOfWork(DifferenceContext context, IDifferenceAsyncRepository differenceAsyncRepository)
        {
            this.context = context;
            this.DifferenceAsyncRepository = differenceAsyncRepository;
        }

        /// <inheritdoc />
        public IDifferenceAsyncRepository DifferenceAsyncRepository { get; }

        /// <inheritdoc />
        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
