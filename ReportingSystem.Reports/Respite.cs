namespace ReportingSystem.Reports
{
    using System;

    /// <summary>
    /// Information about respite of employer.
    /// </summary>
    public class Respite
    {
        /// <summary>
        /// Begin of respite.
        /// </summary>
        public readonly DateTime StartTime;

        /// <summary>
        /// End of respite.
        /// </summary>
        public readonly DateTime EndTime;

        /// <summary>
        /// Continuance of respite.
        /// </summary>
        public readonly TimeSpan Duration;

        /// <summary>
        /// Create instance of class <see cref="Respite"/> class.
        /// </summary>
        /// <param name="startTime">Time of beginning respite.</param>
        /// <param name="endTime">Time of end respite.</param>
        /// <exception cref="ArgumentNullException">Thrown when begin or end time passed as null.</exception>
        public Respite(DateTime startTime, DateTime endTime)
        {
            if (startTime == null || endTime == null)
            {
                throw new ArgumentNullException("Time of end and beginning respite can't be null.");
            }

            this.StartTime = startTime;
            this.EndTime = endTime;

            this.Duration = this.EndTime - this.StartTime;
        }
    }
}
