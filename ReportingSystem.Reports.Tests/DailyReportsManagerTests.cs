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
    internal class DailyReportsManagerTests
    {
        #region VerifyFirstStamp Test

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
            DailyReportsManager dailyReporter = this.createDailyReporter(stamps);
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
        public void VerifyFirstStamp_EmptyCollection_NoArgumentOutOfRangeExceptions()
        {
            // Arrange
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>();
            DailyReportsManager dailyReporter = this.createDailyReporter(stamps);
            ReportProtocol<int> protocol = new ReportProtocol<int>();

            // Act and Assert
            // Check that call method with empty collection not throwing any exceptions.
            Assert.That(() => dailyReporter.verifyFirstStamp<int>(stamps, targetEmployerID, targetDay, protocol), Throws.Exception.Not.TypeOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Check that method do not any changes if first in-stamp for target day exists.
        /// </summary>
        [TestCase]
        public void VerifyFirstStamp_DataIsCorrect_NoAnyChange()
        {
            // Arrange
            // Target day is 01.03.2016
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>()
            {
                // In-stamp at 9.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(9) },

                // Out-stamp at 10.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(10) }
            };
            int originalStampsCount = stamps.Count;
            DailyReportsManager dailyReporter = this.createDailyReporter(stamps);
            ReportProtocol<int> protocol = new ReportProtocol<int>();
            List<EmployerTimeStamp> expectedStamps = new List<EmployerTimeStamp>()
            {
                stamps[0],
                stamps[1]
            };

            // Act
            dailyReporter.verifyFirstStamp<int>(stamps, targetEmployerID, targetDay, protocol);

            // Assertions
            // Check that collections contain same items in same order.
            Assert.That(stamps, Is.EqualTo(expectedStamps));
        }

        #endregion

        #region VerifyFirstStamp Tests

        /// <summary>
        /// Verifying method should add end of target day as last Out-stamp.
        /// </summary>
        [TestCase]
        public void VerifyLastStamp_NoOutStampForTargetDay_AddEndOfTargetDayAsLastOfOutStamp()
        {
            // Arrange
            int targetEmployerID = 1;
            DateTime targetDay = new DateTime(2016, 3, 1);
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>()
            {
                // In-stamp at 6.00 pm of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(18) },

                // In-stamp at 6.00 pm of next day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddDays(1).AddHours(6) }
            };
            var dailyReporter = this.createDailyReporter(stamps);
            var protocol = new ReportProtocol<int>();

            // expected added stamp has Out type and end of target day as time.
            var expectedCollectionSize = stamps.Count + 1;
            var expectedStamp = new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(23).AddMinutes(59).AddSeconds(59) };

            // Act
            dailyReporter.verifyLastStamp<int>(stamps, targetEmployerID, targetDay, protocol);

            // Assert
            // Check that item was added to collection.
            Assert.That(stamps.Count, Is.EqualTo(expectedCollectionSize));
            Assert.That(stamps[stamps.Count - 1], Is.EqualTo(expectedStamp).Using(new EmployerTimeStampComparer()));
        }

        /// <summary>
        /// Check adding of out-stamp next day before 4.00 am.
        /// </summary>
        [TestCase]
        public void VerifyLastStamp_LastOutStampNextDayBefore4am_AddOutStampFromNextDay()
        {
            // Arrange
            int targetEmployerID = 1;
            DateTime targetDay = new DateTime(2016, 3, 1);
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>()
            {
                // In-stamp at 6.00 pm of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(18) },

                // In-stamp at 3.00 pm of next day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddDays(1).AddHours(3) }
            };
            var dailyReporter = this.createDailyReporter(stamps);
            var protocol = new ReportProtocol<int>();

            // expected added stamp has Out type and end of target day as time.
            var expectedStamp = stamps[stamps.Count - 1];

            // Act
            dailyReporter.verifyLastStamp<int>(stamps, targetEmployerID, targetDay, protocol);

            // Assert
            // Check that item was added to collection.
            Assert.That(stamps[stamps.Count - 1], Is.EqualTo(expectedStamp).Using(new EmployerTimeStampComparer()));
        }

        /// <summary>
        /// Verifying method add end of target day as last stamp 
        /// even if exists out-stamp next day but its time after 4 am.
        /// </summary>
        [TestCase]
        public void VerifyLastStamp_LastOutStampNextDayAfter4am_AddEndOfTargetDayAsLastOfOutStamp()
        {
            // Arrange
            int targetEmployerID = 1;
            DateTime targetDay = new DateTime(2016, 3, 1);
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>()
            {
                // In-stamp at 6.00 pm of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(18) },

                // In-stamp at 6.00 am of next day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddDays(1).AddHours(4).AddMinutes(1) }
            };
            var dailyReporter = this.createDailyReporter(stamps);
            var protocol = new ReportProtocol<int>();

            // expected added stamp has Out type and end of target day as time.
            var expectedStamp = new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(23).AddMinutes(59).AddSeconds(59) };

            // Act
            dailyReporter.verifyLastStamp<int>(stamps, targetEmployerID, targetDay, protocol);

            // Assert
            // Check that item was added to collection.
            Assert.That(stamps[stamps.Count - 1], Is.EqualTo(expectedStamp).Using(new EmployerTimeStampComparer()));
        }

        /// <summary>
        /// When working with empty collection should not throw exceptions.
        /// </summary>
        [TestCase]
        public void VerifyLastStamp_EmptyCollection_NoArgumenOutOfRangeExceptions()
        {
            // Arrange
            int targetEmployerID = 1;
            DateTime targetDay = new DateTime(2016, 3, 1);
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>();
            var dailyReporter = this.createDailyReporter(stamps);
            var protocol = new ReportProtocol<int>();

            // Act and assert
            Assert.That(() => dailyReporter.verifyLastStamp<int>(stamps, targetEmployerID, targetDay, protocol), Throws.Exception.Not.TypeOf<ArgumentOutOfRangeException>());
        }

        #endregion

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

        /// <summary>
        /// Create reporter with mock source of stamps.
        /// </summary>
        /// <param name="stamps">Stamps which will used for mock source of data.</param>
        /// <returns>Instance of <see cref="DailyReportsManager"/> with mock source of stamps.</returns>
        private DailyReportsManager createDailyReporter(List<EmployerTimeStamp> stamps)
        {
            return new DailyReportsManager(this.createStampsSource(stamps));
        }
    }
}
