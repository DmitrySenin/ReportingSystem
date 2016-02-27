namespace ReportingSystem.Core
{
    using System;

    /// <summary>
    /// Represents time of some action of employer.
    /// </summary>
    class EmployerTimeStamp
    {
        /// <summary>
        /// Gets/sets unique identifier of employer.
        /// </summary>
        public int EmployerID { get; set; }

        /// <summary>
        /// Gets/sets time of employer's action.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets/sets type of action.
        /// </summary>
        public StampType Type { get; set; }
    }
}
