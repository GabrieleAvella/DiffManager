namespace DiffManager.DataContracts
{
    using System.ComponentModel.DataAnnotations;

    using DiffManager.Common.Converters;

    using Newtonsoft.Json;

    public class DiffItemDto
    {
        /// <summary>
        /// Gets or sets the value of the difference.
        /// </summary>
        [Required]
        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Value { get; set; }
    }
}
