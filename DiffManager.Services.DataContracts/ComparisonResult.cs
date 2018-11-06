﻿namespace DiffManager.Services.DataContracts
{
    using System.Collections.Generic;

    using DiffManager.Common;

    public class ComparisonResult
    {
        /// <summary>
        /// Gets or sets the type of the result generated by the comparison.
        /// </summary>
        public DiffComparisonResultType Result { get; set; }

        /// <summary>
        /// Gets or sets the differences found during the comparison.
        /// </summary>
        public IDictionary<int, int> Differences { get; set; }
    }
}
