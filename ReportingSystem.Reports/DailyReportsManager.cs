namespace ReportingSystem.Reports
{
    using System;
    using System.Collections.Generic;

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

        /// <summary>
        /// Computes time of work for concrete day of some employer.
        /// </summary>
        /// <param name="employerID">Unique identifier of employer.</param>
        /// <param name="day">Date of day of computation.</param>
        /// <returns>Protocol that collect result of computation and notifications of system.</returns>
        public ReportProtocol<TimeSpan> TimeOfWorkForDay(int employerID, DateTime day)
        {
            ReportProtocol<TimeSpan> protocol = new ReportProtocol<TimeSpan>();

            List<EmployerTimeStamp> employerStamps = this.collectStapmsForDailyReport<TimeSpan>(employerID, day, protocol);

            return protocol;
        }

        /// <summary>
        /// Gathers all data for daily report, check it and complete if necessary.
        /// Add important notification to protocol.
        /// </summary>
        /// <typeparam name="T">Type of protocol result.</typeparam>
        /// <param name="employerID">Target employer's unique identifier.</param>
        /// <param name="day">Reporting day.</param>
        /// <param name="protocol">Protocol of reporting.</param>
        /// <returns>Checked stamps for daily reporting.</returns>
        private List<EmployerTimeStamp> collectStapmsForDailyReport<T>(int employerID, DateTime day, ReportProtocol<T> protocol)
        {
            List<EmployerTimeStamp> employerStamps = this.employerStampsSource.GetByEmployerIDForDay(employerID, day);

            if (employerStamps.Count == 0)
            {
                return employerStamps;
            }

            this.verifyFirstStamp<T>(employerStamps, employerID, day, protocol);

            return employerStamps;
        }

        /// <summary>
        /// Check out first element of collection of stamps 
        /// to add if require non existent first In-stamp.
        /// </summary>
        /// <typeparam name="T">Type of result of protocol.</typeparam>
        /// <param name="employerStamps">Collection that should be checked.</param>
        /// <param name="employerID">Unique identifier of target employer.</param>
        /// <param name="day">Date of day of reporting.</param>
        /// <param name="protocol">Protocol of reporting.</param>
        private void verifyFirstStamp<T>(List<EmployerTimeStamp> employerStamps, int employerID, DateTime day, ReportProtocol<T> protocol)
        {
            // if first record is not in-stamp.
            if (employerStamps[0].Type != StampType.In)
            {
                protocol.Notifications.Add(new Notification("First In-stamp was not found.", NotificationType.Warning));
                protocol.Notifications.Add(new Notification("Begin of target day was added as first In-stamp.", NotificationType.Message));

                EmployerTimeStamp firstInStamp = new EmployerTimeStamp();
                firstInStamp.EmployerID = employerID;
                firstInStamp.Type = StampType.In;

                // set beginning of target day.
                firstInStamp.Time = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
            }
        }
    }
}
