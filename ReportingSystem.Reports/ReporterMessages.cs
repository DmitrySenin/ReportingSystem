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
    }
}
