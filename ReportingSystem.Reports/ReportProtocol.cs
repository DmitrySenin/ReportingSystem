namespace ReportingSystem.Reports
{
    using System.Collections.Generic;

    /// <summary>
    /// Result protocol of reporting.
    /// </summary>
    /// <typeparam name="T">Type of returned result.</typeparam>
    public class ReportProtocol<T>
    {
        /// <summary>
        /// Gets/sets target result of report.
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// Gets/sets flag that identifies success of reporting.
        /// </summary>
        public bool IsSucceed { get; set; }

        /// <summary>
        /// Notifications detected by system during reporting.
        /// </summary>
        public List<Notification> Notifications { get; set; }
    }
}
