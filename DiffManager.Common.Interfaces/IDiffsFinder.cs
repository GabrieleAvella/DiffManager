namespace DiffManager.Common.Interfaces
{
    using System.Collections.Generic;
    
    public interface IDiffsFinder
    {
        /// <summary>
        /// Finds the differences between the left and right data.
        /// </summary>
        /// <param name="left">The left data</param>
        /// <param name="right">The right data</param>
        /// <exception cref="DiffsDataLengthNotEqualException">The length of left and right data is not the same.</exception>
        /// <returns>
        /// Returns a dictionary of differences, 
        /// where the key represents the offset and the value represents the length.
        /// If both the inputs are NULL, the algorithm assumes that there are no differences.
        /// </returns>
        IDictionary<int, int> Find(byte[] left, byte[] right);
    }
}