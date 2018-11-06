namespace DiffManager.Common
{
    public enum DiffComparisonResultType
    {
        /// <summary>
        /// There are no differences.
        /// </summary>
        Equal,

        /// <summary>
        /// Left and right do not have the same length.
        /// </summary>
        LengthNotEqual,

        /// <summary>
        /// There are differences.
        /// </summary>
        NotEqual
    }
}