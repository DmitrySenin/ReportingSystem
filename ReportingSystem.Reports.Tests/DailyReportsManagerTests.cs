namespace ReportingSystem.Reports.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;
    using NUnit.Framework;

    using ReportingSystem.Core;

    [TestFixture]
    class DailyReportsManagerTests
    {
        /// <summary>
        /// Create mock of employer time stamps source based on passed stamps.
        /// </summary>
        /// <returns>Instance of time stamps source which manipulates passed data.</returns>
        private IEmployerStampsSource createStampsSource(List<EmployerTimeStamp> stamps)
        {
            Mock<IEmployerStampsSource> mockStampsSource = new Mock<IEmployerStampsSource>();

            mockStampsSource.Setup(m => m.GetAll()).Returns(stamps);

            // Grouped all stamps by employer ID and locate it all to one record.
            var stampsGroupsForEmployer = from stamp in stamps
                                          orderby stamp.Time, stamp.Type
                                          group stamp by stamp.EmployerID into empGroup
                                          select new { EmployerID = empGroup.Key, Stamps = empGroup.Select(s => s).ToList() };
            
            // Setup mock's GetByEmployerID method.
            foreach(var empStamps in stampsGroupsForEmployer)
            {
                mockStampsSource.Setup(m => m.GetByEmployerID(It.Is<int>(id => id == empStamps.EmployerID))).Returns(empStamps.Stamps);
            }

            // Grouped all stamps by employer and day and locate it all to one record.
            var stampsGroupsForEmployerAndDay = from empGroup in stampsGroupsForEmployer
                                                from dayGroup in
                                                       (from dayGroup in empGroup.Stamps
                                                        group dayGroup by dayGroup.Time.Date)
                                                select new { EmployerID = empGroup.EmployerID, Day = dayGroup.Key, Stamps = dayGroup.Select(s => s).ToList() };

            // Setup mock's GetByEmployerIDForDay method.
            foreach (var empDayStamps in stampsGroupsForEmployerAndDay)
            {
                mockStampsSource.Setup(m => m.GetByEmployerIDForDay(It.Is<int>(id => id == empDayStamps.EmployerID), It.Is<DateTime>(day => day == empDayStamps.Day))).Returns(empDayStamps.Stamps);
            }

            return mockStampsSource.Object;
        }
    }
}
