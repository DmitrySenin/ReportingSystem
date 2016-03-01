namespace ReportingSystem.Reports.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;
    using NUnit.Framework;

    using ReportingSystem.Core;

    /// <summary>
    /// Test of daily reporter manager.
    /// Used name convention: [MethodUnderTest]_[State]_[Expected].
    /// </summary>
    [TestFixture]
    class DailyReportsManagerTests
    {
        /// <summary>
        /// Check existing in collection of stamps first stamp with In type.
        /// </summary>
        [TestCase]
        public void VerifyFirstStamp_NoFirstInStamp_AddBeginOfDayAsFirstInStamp()
        {
            // Arrange
            // Target day is 01.03.2016
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>()
            {
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay }
            };
            int originalStampsCount = stamps.Count;
            IEmployerStampsSource stampsSource = this.createStampsSource(stamps);
            DailyReportsManager dailyReporter = new DailyReportsManager(stampsSource);
            EmployerTimeStamp expectedStamp = new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = new DateTime(targetDay.Year, targetDay.Month, targetDay.Day, 0, 0, 0)};
            ReportProtocol<int> protocol = new ReportProtocol<int>();

            // Act
            dailyReporter.verifyFirstStamp<int>(stamps, targetEmployerID, targetDay, protocol);

            // Assertions
            // Check 
            Assert.AreEqual(stamps.Count, originalStampsCount + 1);
            Assert.That(stamps[0], Is.EqualTo(expectedStamp).Using(new EmployerTimeStampComparer()));
        }

        /// <summary>
        /// Method should not throw any exceptions when working with empty collection.
        /// </summary>
        [TestCase]
        public void VerifyFirstStamp_EmptyCollection_NoExceptions()
        {
            // Arrange
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>();
            IEmployerStampsSource stampsSource = this.createStampsSource(stamps);
            DailyReportsManager dailyReporter = new DailyReportsManager(stampsSource);
            ReportProtocol<int> protocol = new ReportProtocol<int>();

            // Act and Assert
            // Check that call method with empty collection not throwing any exceptions.
            Assert.That(() => dailyReporter.verifyFirstStamp<int>(stamps, targetEmployerID, targetDay, protocol), Throws.Nothing);
        }

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
