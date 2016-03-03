namespace ReportingSystem.Reports
{
    /// <summary>
    /// Contains messages used by daily reports manager.
    /// </summary>
    internal static class ReporterMessages
    {
        /// <summary>
        /// Describes situation when source of stamps is null.
        /// </summary>
        public static readonly string StampsSourceReferenceIsNull = "Reference to source of data can't be null.";

        /// <summary>
        /// Describing message when reference to protocol is null.
        /// </summary>
        public static readonly string ProtocolReferenceIsNull = "Protocol can't be null reference.";

        /// <summary>
        /// Desctibing message when reference to collection of notifications is null reference.
        /// </summary>
        public static readonly string NotificationsRefrenceIsNull = "Reference to collection of notifications can't be null.";

        /// <summary>
        /// Describing message when reference to stamps collections is null.
        /// </summary>
        public static readonly string StampsCollectionReferenceIsNull = "Collection of stamps can't be null reference.";

        /// <summary>
        /// Describing message when for employer system can't find any stamps for day.
        /// </summary>
        public static readonly string EmployerStampsNotFound = "Could not found stamps of employer for requested day.";

        /// <summary>
        /// Describing message when first in-stamp was not found.
        /// </summary>
        public static readonly string FirstInStampNotFound = "First In-stamp was not found.";

        /// <summary>
        /// Describing message when beginning of target day was added as first In-stamp.
        /// </summary>
        public static readonly string BeginDayAsInStampAdded = "Begin of target day was added as first In-stamp.";

        /// <summary>
        /// Describing message of situation when system can't find last out-stamp for day and employer.
        /// </summary>
        public static readonly string LastOutStampNotFound = "Last out-stamp was not found.";

        /// <summary>
        /// Describing message of situation when first found Out-stamp of next day 
        /// was added as last out-stamp.
        /// </summary>
        public static readonly string FirstOutNextDayAsLast = "First Out-stamp of next day was added as last Out-stamp.";

        /// <summary>
        /// Describing message when end of target day was added as last Out-stamp.
        /// </summary>
        public static readonly string EndDayAsLastOutStamp = "End of target day was added as last Out-stamp.";

        /// <summary>
        /// Describing message when two equal stamps was found and one of them was removed.
        /// </summary>
        public static readonly string OneOfEqualStampsRemoved = "Equal stamps was found. One of them was removed.";

        /// <summary>
        /// Describing message when new stamp was added between two of same type.
        /// </summary>
        public static readonly string NewStampInMiddleBetweenSameType = "Found two followed stamps of one type. Add new between them at the middle.";
    }
}
