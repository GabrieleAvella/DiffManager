namespace DiffManager.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Difference
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the left data.
        /// </summary>
        public byte[] Left { get; set; }

        /// <summary>
        /// Gets or sets the right data.
        /// </summary>
        public byte[] Right { get; set; }
    }
}
