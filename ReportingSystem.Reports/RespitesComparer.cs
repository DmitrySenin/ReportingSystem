namespace ReportingSystem.Reports
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Compares respites.
    /// </summary>
    public class RespitesComparer : IComparer<Respite>
    {
        /// <summary>
        /// Compare two instance of <see cref="Respite"/> class.
        /// Firstly compare start time, then end time.
        /// </summary>
        /// <param name="x">First respite to compare.</param>
        /// <param name="y">Second respite to compare.</param>
        /// <returns>Negative value if first respite less than second one.
        /// Positive if first greater than second.
        /// Zero if instances are equal.</returns>
        public int Compare(Respite x, Respite y)
        {
            if (x.StartTime != y.StartTime)
            {
                return x.StartTime < y.StartTime ? -1 : 1;
            }
            else
            {
                if (x.EndTime != y.EndTime)
                {
                    return x.EndTime < y.EndTime ? -1 : 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
