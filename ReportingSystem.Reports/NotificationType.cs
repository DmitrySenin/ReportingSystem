namespace ReportingSystem.Reports
{
    /// <summary>
    /// Helps classify notifications.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Notification is unresolved error.
        /// </summary>
        Error,

        /// <summary>
        /// Notification is resolved error.
        /// </summary>
        Warning,

        /// <summary>
        /// Notification represents information about some action of system
        /// or not dangerous accident.
        /// </summary>
        Message
    }
}