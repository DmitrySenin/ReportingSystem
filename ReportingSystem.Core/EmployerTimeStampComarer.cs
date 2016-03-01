namespace ReportingSystem.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Comparator of two time stamps.
    /// </summary>
    public class EmployerTimeStampComparer : IComparer<EmployerTimeStamp>
    {
        /// <summary>
        /// Compare two object of employer time stamp.
        /// Firstly, compare employerID, then dates, then types of stamps.
        /// </summary>
        /// <param name="x">First compared stamp.</param>
        /// <param name="y">Second compared stamp.</param>
        /// <returns>Negative value if first less than second. Positive value if first greater that second. Zero if arguments are equal.</returns>
        public int Compare(EmployerTimeStamp x, EmployerTimeStamp y)
        {
            if (x.EmployerID != y.EmployerID)
            {
                return x.EmployerID - y.EmployerID;
            }
            else
            {
                if (x.Time != y.Time)
                {
                    return (x.Time < y.Time) ? -1 : 1;

                }
                else
                {
                    if (x.Type != y.Type)
                    {
                        return x.Type - y.Type;
                    }
                    return 0;
                }
            }
        }
    }
}
