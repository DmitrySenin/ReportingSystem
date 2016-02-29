namespace ReportingSystem.Reports
{
    using System.Collections.Generic;
    using System.Linq;

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

        /// <summary>
        /// Create instance of <see cref="ReportProtocol{T}"/> class.
        /// </summary>
        public ReportProtocol()
        {
            this.Result = default(T);
            this.Notifications = new List<Notification>();
        }

        /// <summary>
        /// Collect notification by type.
        /// </summary>
        /// <param name="type">Target type of notification.</param>
        /// <returns>Collection of notifications contains records only with specified type or empty collection.</returns>
        public List<Notification> NotificationsByType(NotificationType type)
        {
            var notifications = (from notification in this.Notifications
                                 where notification.Type == type
                                 select notification).ToList();

            return notifications;
        }
    }
}
