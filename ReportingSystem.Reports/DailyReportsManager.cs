namespace ReportingSystem.Reports
{
    using System;

    using ReportingSystem.Core;

    /// <summary>
    /// Makes daily reports.
    /// </summary>
    public class DailyReportsManager
    {
        /// <summary>
        /// Source of employers time stamps to get information for reports.
        /// </summary>
        private readonly IEmployerStampsSource employerStampsSource;

        /// <summary>
        /// Create instance of <see cref="DailyReportsManager"/> class.
        /// </summary>
        /// <param name="employerStampsSource">Connection between report manager and source for reporting.</param>
        /// <exception cref="ArgumentNullException">Thrown when passed stamps source is null reference.</exception>
        public DailyReportsManager(IEmployerStampsSource employerStampsSource)
        {
            if (employerStampsSource == null)
            {
                throw new ArgumentNullException("Reference to source of data can't be null.");
            }

            this.employerStampsSource = employerStampsSource;
        }
    }
}
