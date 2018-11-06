namespace DiffManager.DataContracts
{
    public class DiffComparisonItemDto
    {
        /// <summary>
        /// Gets or sets the offset of the difference.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets the length of the difference.
        /// </summary>
        public int Length { get; set; }
    }
}
