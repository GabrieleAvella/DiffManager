﻿namespace DiffManager.DataContracts
{
    using System.ComponentModel.DataAnnotations;

    using DiffManager.Common.Converters;

    using Newtonsoft.Json;

    /// <summary>
    /// The DTO for creating a difference.
    /// </summary>
    public class DifferenceForCreationDto
    {
        /// <summary>
        /// Gets or sets the left value of the difference.
        /// </summary>
        [Required]
        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Left { get; set; }

        /// <summary>
        /// Gets or sets the right value of the difference.
        /// </summary>
        [Required]
        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Right { get; set; }
    }
}
