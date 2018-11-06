namespace DiffManager.Services
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using AutoMapper;

    using DiffManager.Common;
    using DiffManager.Common.Exceptions;
    using DiffManager.Common.Interfaces;
    using DiffManager.Domains.UnitOfWorks;
    using DiffManager.Models;
    using DiffManager.Services.DataContracts;
    using DiffManager.Services.Interfaces;

    /// <summary>
    /// The Difference service.
    /// </summary>
    public class DiffService : IDiffService
    {
        private readonly IDiffsFinder diffsFinder;

        private readonly IDifferenceUnitOfWork differenceUnitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffService"/> class.
        /// </summary>
        /// <param name="diffsFinder">The differences finder.</param>
        /// <param name="differenceUnitOfWork">The difference unit of work.</param>
        public DiffService(IDiffsFinder diffsFinder, IDifferenceUnitOfWork differenceUnitOfWork)
        {
            this.diffsFinder = diffsFinder;
            this.differenceUnitOfWork = differenceUnitOfWork;
        }

        /// <inheritdoc />
        public async Task AddLeftDiffItemAsync(
            Guid id,
            byte[] item,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.AddDiffItemAsync(id, item, DiffDataType.Left, cancellationToken);
        }

        /// <inheritdoc />
        public async Task AddRightDiffItemAsync(
            Guid id,
            byte[] item,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.AddDiffItemAsync(id, item, DiffDataType.Right, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ComparisonResult> GetDifferencesAsync(
            Guid id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var difference =
                await this.differenceUnitOfWork.DifferenceAsyncRepository.RetrieveAsync(id, cancellationToken);

            if (difference == null)
            {
                throw new ResourceNotFoundException("No difference was found with the specified ID", id);
            }

            var comparisonResult = new ComparisonResult();

            try
            {
                var diffs = this.diffsFinder.Find(difference.Left, difference.Right);

                comparisonResult.Result =
                    diffs.Any() ? DiffComparisonResultType.NotEqual : DiffComparisonResultType.Equal;

                comparisonResult.Differences = diffs;
            }
            catch (DiffsDataLengthNotEqualException)
            {
                comparisonResult.Result = DiffComparisonResultType.LengthNotEqual;
            }

            return comparisonResult;
        }

        /// <inheritdoc />
        public async Task<DifferenceDto> CreateAsync(
            DifferenceForCreationDto difference,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (difference == null)
            {
                throw new ArgumentNullException(nameof(difference));
            }

            var differenceEntity = Mapper.Map<Difference>(difference);

            this.differenceUnitOfWork.DifferenceAsyncRepository.Insert(differenceEntity);
            if (!await this.differenceUnitOfWork.SaveChangesAsync(cancellationToken))
            {
                throw new Exception("Creating the difference failed on save.");
            }

            return Mapper.Map<DifferenceDto>(differenceEntity);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Guid id, DifferenceForUpdateDto difference, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (difference == null)
            {
                throw new ArgumentNullException(nameof(difference));
            }

            Difference differenceToUpdate = await this.differenceUnitOfWork.DifferenceAsyncRepository.RetrieveAsync(id, cancellationToken);
            if (differenceToUpdate == null)
            {
                throw new ResourceNotFoundException("No difference was found with the specified ID", id);
            }

            Difference updatedDifference = Mapper.Map(difference, differenceToUpdate);

            this.differenceUnitOfWork.DifferenceAsyncRepository.Update(updatedDifference);
            if (!await this.differenceUnitOfWork.SaveChangesAsync(cancellationToken))
            {
                throw new Exception($"Updating the difference {id} failed on save.");
            }
        }

        private async Task AddDiffItemAsync(
            Guid id,
            byte[] item,
            DiffDataType dataType,
            CancellationToken cancellationToken)
        {
            // Retrieves the difference from the data store
            var difference =
                await this.differenceUnitOfWork.DifferenceAsyncRepository.RetrieveAsync(id, cancellationToken);

            var isNewDifference = difference == null;

            if (isNewDifference)
            {
                // This is a new record
                difference = new Difference { Id = id };

                // Inserts it into the data store
                this.differenceUnitOfWork.DifferenceAsyncRepository.Insert(difference);
            }

            // Updates the value with the new data
            if (dataType == DiffDataType.Left)
            {
                difference.Left = item;
            }
            else
            {
                difference.Right = item;
            }

            if (!isNewDifference)
            {
                // This is not really needed with EF, but the Update is called for consistancy reasons
                // (in case the Unit of Work handles another ORM later on.)
                this.differenceUnitOfWork.DifferenceAsyncRepository.Update(difference);
            }

            // Saves the changes to the data store
            if (!await this.differenceUnitOfWork.SaveChangesAsync(cancellationToken))
            {
                throw new Exception("Add difference failed on save.");
            }
        }
    }
}
