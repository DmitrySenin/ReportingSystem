namespace ReportingSystem.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Frame to implement source of stamps of employers actions.
    /// Guarantees that all data will be sorted by date, time and type of stamp.
    /// </summary>
    public interface IEmployerStampsSource
    {
        /// <summary>
        /// Find all data from source.
        /// </summary>
        /// <returns>All records from source.</returns>
        List<EmployerTimeStamp> GetAll();

        /// <summary>
        /// Find all records related with some employer.
        /// </summary>
        /// <param name="EmployerID">Unique identifier of employer.</param>
        /// <returns>All time stamps of some employer.</returns>
        List<EmployerTimeStamp> GetByEmployerID(int EmployerID);

        /// <summary>
        /// Find all data of employer for concrete date.
        /// </summary>
        /// <param name="EmployerID">Unique identifier of employer.</param>
        /// <param name="Day">Date of day to collect data.</param>
        /// <returns>All stamps of employer for concrete day.</returns>
        List<EmployerTimeStamp> GetByEmployerIDForDay(int EmployerID, DateTime Day);
    }
}
