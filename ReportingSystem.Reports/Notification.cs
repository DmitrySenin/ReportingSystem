namespace ReportingSystem.Reports
{
    /// <summary>
    /// Represents notification of reporting system about whatever it does.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Describes details of notification.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Describe dangerous of event represented by this notification.
        /// </summary>
        public readonly NotificationType Type;

        /// <summary>
        /// Create instance of class <see cref="Notification"/> class.
        /// </summary>
        /// <param name="description">Message describing notification.</param>
        /// <param name="type">Type of notification.</param>
        public Notification(string description, NotificationType type)
        {
            this.Description = description;
            this.Type = type;
        }
    }
}
