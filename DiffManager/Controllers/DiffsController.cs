namespace DiffManager.Controllers
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using AutoMapper;

    using DiffManager.Common;
    using DiffManager.Common.Exceptions;
    using DiffManager.DataContracts;
    using DiffManager.Services.DataContracts;
    using DiffManager.Services.Interfaces;

    using Microsoft.AspNetCore.Mvc;

    using DifferenceDto = DataContracts.DifferenceDto;
    using DifferenceForCreationDto = DataContracts.DifferenceForCreationDto;
    using DifferenceForUpdateDto = DataContracts.DifferenceForUpdateDto;

    using DifferenceForCreationServiceDto = Services.DataContracts.DifferenceForCreationDto;
    using DifferenceForUpdateServiceDto = Services.DataContracts.DifferenceForUpdateDto;
    using DifferenceServiceDto = Services.DataContracts.DifferenceDto;

    /// <summary>
    /// The differences controller.
    /// </summary>
    [Route("v1/diff")]
    [ApiController]
    public class DiffsController : ControllerBase
    {
        private readonly IDiffService diffService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffsController"/> class.
        /// </summary>
        /// <param name="diffService">The differences service.</param>
        public DiffsController(IDiffService diffService)
        {
            this.diffService = diffService;
        }

        /// <summary>
        /// Sets the left data for difference.
        /// </summary>
        /// <param name="id">
        /// The ID of the difference.
        /// </param>
        /// <param name="diffData">
        /// The data of the difference.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation Token.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost("{id}/left")]
        public async Task<IActionResult> SetLeftDiff(Guid id, DiffItemDto diffData, CancellationToken cancellationToken)
        {
            await this.diffService.AddLeftDiffItemAsync(id, diffData.Value, cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Sets the right data for difference.
        /// </summary>
        /// <param name="id">
        /// The ID of the difference.
        /// </param>
        /// <param name="diffData">
        /// The data of the difference.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation Token.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost("{id}/right")]
        public async Task<IActionResult> SetRightDiff(Guid id, DiffItemDto diffData, CancellationToken cancellationToken)
        {
            await this.diffService.AddRightDiffItemAsync(id, diffData.Value, cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Gets the differences between the left and right data.
        /// </summary>
        /// <param name="id">
        /// The ID of the differences.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation Token.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDifferences(Guid id, CancellationToken cancellationToken)
        {
            ComparisonResult differences;
            try
            {
                differences = await this.diffService.GetDifferencesAsync(id, cancellationToken);
            }
            catch (ResourceNotFoundException)
            {
                return this.NotFound();
            }

            // Returns a dynamic result based on the status of the comparison
            return this.Ok(ShapeData(differences));
        }

        /// <summary>
        /// Creates the resource.
        /// </summary>
        /// <param name="difference">
        /// The difference.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="T:CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create(
            DifferenceForCreationDto difference,
            CancellationToken cancellationToken)
        {
            var differenceForCreationDto = Mapper.Map<DifferenceForCreationServiceDto>(difference);

            DifferenceServiceDto differenceServiceDto = await this.diffService.CreateAsync(differenceForCreationDto, cancellationToken);

            var differenceToReturn = Mapper.Map<DifferenceDto>(differenceServiceDto);

            return this.Ok(differenceToReturn);
        }

        /// <summary>
        /// Fully updates the resource.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="difference">
        /// The Difference.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="T:CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DifferenceForUpdateDto difference, CancellationToken cancellationToken)
        {
            var differenceForUpdateServiceDto = Mapper.Map<DifferenceForUpdateServiceDto>(difference);

            try
            {
                await this.diffService.UpdateAsync(id, differenceForUpdateServiceDto, cancellationToken);
            }
            catch (ResourceNotFoundException)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        private static ExpandoObject ShapeData(ComparisonResult differences)
        {
            dynamic diffResult = new ExpandoObject();

            switch (differences.Result)
            {
                case DiffComparisonResultType.Equal:
                    diffResult.Result = "No differences";
                    break;
                case DiffComparisonResultType.LengthNotEqual:
                    diffResult.Result = "The length is not the same";
                    break;
                case DiffComparisonResultType.NotEqual:
                    diffResult.Result = differences.Differences
                        .Select(diff => new DiffComparisonItemDto { Offset = diff.Key, Length = diff.Value }).ToArray();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return diffResult;
        }
    }
}