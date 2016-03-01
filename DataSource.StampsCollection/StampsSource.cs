namespace DataSource.StampsCollection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ReportingSystem.Core;

    /// <summary>
    /// Contains employers time stamps in built-in collection 
    /// without connection with outer sources.
    /// </summary>
    public class StampsSource : IEmployerStampsSource
    {
        /// <summary>
        /// Records of time stamps.
        /// </summary>
        private List<EmployerTimeStamp> employersStamps;

        /// <summary>
        /// Create instance of <see cref="StampsSource"/> class.
        /// </summary>
        public StampsSource()
        {
            employersStamps = new List<EmployerTimeStamp>()
            {
                new EmployerTimeStamp() { EmployerID = 1, Type = StampType.In, Time = new DateTime(2016, 2, 29, 9, 30, 0)}, // Emp1. In : 9.30 am
                new EmployerTimeStamp() { EmployerID = 1, Type = StampType.Out, Time = new DateTime(2016, 2, 29, 9, 45, 0)}, // Emp1. Out : 9.45 am
                new EmployerTimeStamp() { EmployerID = 1, Type = StampType.In, Time = new DateTime(2016, 2, 29, 10, 00, 0)}, // Emp1. In : 10.00 am
                new EmployerTimeStamp() { EmployerID = 1, Type = StampType.Out, Time = new DateTime(2016, 2, 29, 18, 30, 0)}, // Emp1. Out : 6.30 pm
                
                new EmployerTimeStamp() { EmployerID = 2, Type = StampType.In, Time = new DateTime(2016, 2, 29, 9, 0, 0)}, // Emp2. In : 9.00 am
                new EmployerTimeStamp() { EmployerID = 2, Type = StampType.Out, Time = new DateTime(2016, 2, 29, 9, 18, 0)}, // Emp2. Out : 9.18 am
                new EmployerTimeStamp() { EmployerID = 2, Type = StampType.Out, Time = new DateTime(2016, 2, 29, 18, 30, 0)}, // Emp2. Out : 6.30 pm
                new EmployerTimeStamp() { EmployerID = 2, Type = StampType.Out, Time = new DateTime(2016, 2, 29, 18, 30, 0)}, // Emp2. Out : 6.30 pm
                new EmployerTimeStamp() { EmployerID = 2, Type = StampType.In, Time = new DateTime(2016, 2, 29, 19, 30, 0)}, // Emp2. Out : 7.30 pm
            };
        }

        /// <summary>
        /// Find all data in contained collection.
        /// </summary>
        /// <returns>All built-in stamps.</returns>
        public List<EmployerTimeStamp> GetAll()
        {
            return employersStamps;
        }

        /// <summary>
        /// Finds all records belong to one employer.
        /// </summary>
        /// <param name="EmployerID">Unique identifier of employer.</param>
        /// <returns>Collection with stamps of some employer or empty collection.</returns>
        public List<EmployerTimeStamp> GetByEmployerID(int EmployerID)
        {
            return this.addEmployerIDCondition(this.GetAll(), EmployerID).ToList();
        }

        /// <summary>
        /// Finds all records related with an employer for a day.
        /// </summary>
        /// <param name="EmployerID">Unique identifier of employer.</param>
        /// <param name="Day">Date of target day.</param>
        /// <returns>Collection with requested stamps or empty list.</returns>
        public List<EmployerTimeStamp> GetByEmployerIDForDay(int EmployerID, DateTime Day)
        {
            return this.addDayCondition(this.addEmployerIDCondition(this.GetAll(), EmployerID), Day).ToList();
        }

        /// <summary>
        /// Add restriction of finding to source collection to search records of concrete employer.
        /// </summary>
        /// <param name="employersStamps">Collection that should be constrained.</param>
        /// <param name="employerID">Target employer's unique identifier.</param>
        /// <returns>Lazy collection which should be transform to some collection to find necessary data.</returns>
        private IEnumerable<EmployerTimeStamp> addEmployerIDCondition(IEnumerable<EmployerTimeStamp> employersStamps, int employerID)
        {
            return employersStamps.Where(s => s.EmployerID == employerID);
        }

        /// <summary>
        /// Add restriction of finding to source collection to search records for concrete day.
        /// </summary>
        /// <param name="employersStamps">Collection that should be constrained.</param>
        /// <param name="day">Target day.</param>
        /// <returns>Lazy collection which should be transform to some collection to find necessary data.</returns>
        private IEnumerable<EmployerTimeStamp> addDayCondition(IEnumerable<EmployerTimeStamp> employersStamps, DateTime day)
        {
            return employersStamps.Where(s => s.Time.Date == day.Date);
        }
    }
}
