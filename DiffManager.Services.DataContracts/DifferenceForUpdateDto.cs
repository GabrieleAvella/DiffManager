namespace DiffManager.Services.DataContracts
{
    using DiffManager.Common.Converters;

    using Newtonsoft.Json;

    /// <summary>
    /// The DTO for updating a difference.
    /// </summary>
    public class DifferenceForUpdateDto
    {
        /// <summary>
        /// Gets or sets the left value of the difference.
        /// </summary>
        public byte[] Left { get; set; }

        /// <summary>
        /// Gets or sets the right value of the difference.
        /// </summary>
        public byte[] Right { get; set; }
    }
}
