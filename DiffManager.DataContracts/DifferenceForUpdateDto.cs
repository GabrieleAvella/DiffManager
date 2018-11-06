namespace DiffManager.DataContracts
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
        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Left { get; set; }

        /// <summary>
        /// Gets or sets the right value of the difference.
        /// </summary>
        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Right { get; set; }
    }
}
