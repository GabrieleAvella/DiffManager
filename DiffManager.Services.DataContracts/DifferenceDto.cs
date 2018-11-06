namespace DiffManager.Services.DataContracts
{
    using System;

    public class DifferenceDto
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the value of the left data.
        /// </summary>
        public byte[] Left { get; set; }

        /// <summary>
        /// Gets or sets the value of the right data.
        /// </summary>
        public byte[] Right { get; set; }
    }
}
