namespace DiffManager.Common
{
    using System.Collections.Generic;

    using DiffManager.Common.Exceptions;
    using DiffManager.Common.Interfaces;

    /// <summary>
    /// The differences finder.
    /// </summary>
    public class DiffsFinder : IDiffsFinder
    {
        /// <inheritdoc />
        public IDictionary<int, int> Find(byte[] left, byte[] right)
        {
            var differences = new Dictionary<int, int>();

            // Checks if both the data are null
            // In this case there are no differences.
            if (left == null && right == null)
            {
                return differences;
            }

            var leftLength = left?.Length;

            // If the length doesn't match
            // The data cannot be compared.
            // Returns an exception.
            if (leftLength != right?.Length)
            {
                throw new DiffsDataLengthNotEqualException(
                    $"The length of {nameof(left)} and {nameof(right)} must be the same.");
            }

            // Iterates the data to find contiguous differences 
            var diffIdx = -1;
            for (var i = 0; i < leftLength; i++)
            {
                // Checks if there is a difference
                if (left[i] != right[i])
                {
                    // If the index of the difference is -1
                    // it means that it is the beginning of a new offset
                    if (diffIdx < 0)
                    {
                        // Adds a new key in the dictionary
                        diffIdx = i;
                        differences.Add(i, 1);
                    }
                    else
                    {
                        // Otherwise increment the length
                        differences[diffIdx] = differences[diffIdx] + 1;
                    }
                }
                else
                {
                    // If there is no difference reset the index
                    diffIdx = -1;
                }
            }

            return differences;
        }
    }
}
