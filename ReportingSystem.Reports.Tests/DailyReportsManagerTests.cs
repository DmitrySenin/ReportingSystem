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
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);
            EmployerTimeStamp expectedStamp = new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = new DateTime(targetDay.Year, targetDay.Month, targetDay.Day, 0, 0, 0)};
            List<Notification> notifications = new List<Notification>();

            // Act
            dailyReporter.VerifyFirstStamp(stamps, targetEmployerID, targetDay, notifications);

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
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = new List<Notification>();

            // Act and Assert
            // Check that call method with empty collection not throwing any exceptions.
            Assert.That(() => dailyReporter.VerifyFirstStamp(stamps, targetEmployerID, targetDay, notifications), Throws.Nothing);
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
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = new List<Notification>();
            List<EmployerTimeStamp> expectedStamps = new List<EmployerTimeStamp>()
            {
                stamps[0],
                stamps[1]
            };

            // Act
            dailyReporter.VerifyFirstStamp(stamps, targetEmployerID, targetDay, notifications);

            // Assertions
            // Check that collections contain same items in same order.
            Assert.That(stamps, Is.EqualTo(expectedStamps));
        }

        /// <summary>
        /// Checks that system throw exceptions if reference to source is null.
        /// </summary>
        [TestCase]
        public void VerifyFirstStamp_CollectionOfStampsIsNull_ThrowArgumentNullException()
        {
            // Arrange
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = null;
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = new List<Notification>();

            // Act and Assert
            // Check that call method with null collection throws "ArgumentNullException".
            Assert.That(() => dailyReporter.VerifyFirstStamp(stamps, targetEmployerID, targetDay, notifications), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        /// <summary>
        /// Checks that system throw exceptions if reference to protocol is null.
        /// </summary>
        [TestCase]
        public void VerifyFirstStamp_ProtocolReferenceIsNull_ThrowArgumentNullException()
        {
            // Arrange
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>();
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = null;

            // Act and Assert
            // Check that call method with null protocol throws "ArgumentNullException".
            Assert.That(() => dailyReporter.VerifyFirstStamp(stamps, targetEmployerID, targetDay, notifications), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        #endregion

        #region VerifyLastStamp Tests

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
            var dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = new List<Notification>();

            // expected added stamp has Out type and end of target day as time.
            var expectedCollectionSize = stamps.Count + 1;
            var expectedStamp = new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(23).AddMinutes(59).AddSeconds(59) };

            // Act
            dailyReporter.VerifyLastStamp(stamps, targetEmployerID, targetDay, notifications);

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
            var dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = new List<Notification>();

            // expected added stamp has Out type and end of target day as time.
            var expectedStamp = stamps[stamps.Count - 1];

            // Remove last record to pass to method.
            stamps.RemoveAt(stamps.Count - 1);

            // Act
            dailyReporter.VerifyLastStamp(stamps, targetEmployerID, targetDay, notifications);

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
            var dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = new List<Notification>();

            // expected added stamp has Out type and end of target day as time.
            var expectedStamp = new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(23).AddMinutes(59).AddSeconds(59) };

            // Remove last record to pass to method.
            stamps.RemoveAt(stamps.Count - 1);

            // Act
            dailyReporter.VerifyLastStamp(stamps, targetEmployerID, targetDay, notifications);

            // Assert
            // Check that item was added to collection.
            Assert.That(stamps[stamps.Count - 1], Is.EqualTo(expectedStamp).Using(new EmployerTimeStampComparer()));
        }

        /// <summary>
        /// When working with empty collection should not throw exceptions.
        /// </summary>
        [TestCase]
        public void VerifyLastStamp_EmptyCollection_NoExceptions()
        {
            // Arrange
            int targetEmployerID = 1;
            DateTime targetDay = new DateTime(2016, 3, 1);
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>();
            var dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = new List<Notification>();

            // Act and assert
            Assert.That(() => dailyReporter.VerifyLastStamp(stamps, targetEmployerID, targetDay, notifications), Throws.Nothing);
        }
        
        /// <summary>
        /// Checks that correct data will not changed.
        /// </summary>
        [TestCase]
        public void VerifyLastStamp_DataIsCorrect_NoChages()
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
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = new List<Notification>();
            List<EmployerTimeStamp> expectedStamps = new List<EmployerTimeStamp>()
            {
                stamps[0],
                stamps[1]
            };

            // Act
            dailyReporter.VerifyFirstStamp(stamps, targetEmployerID, targetDay, notifications);

            // Assertions
            // Check that collections contain same items in same order.
            Assert.That(stamps, Is.EqualTo(expectedStamps));
        }

        /// <summary>
        /// Checks that system throw exceptions if reference to source is null.
        /// </summary>
        [TestCase]
        public void VerifyLastStamp_CollectionOfStampsIsNull_ThrowArgumentNullException()
        {
            // Arrange
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = null;
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = new List<Notification>();

            // Act and Assert
            // Check that call method with null collection throws "ArgumentNullException".
            Assert.That(() => dailyReporter.VerifyLastStamp(stamps, targetEmployerID, targetDay, notifications), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        /// <summary>
        /// Checks that system throw exceptions if reference to protocol is null.
        /// </summary>
        [TestCase]
        public void VerifyLastStamp_ProtocolReferenceIsNull_ThrowArgumentNullException()
        {
            // Arrange
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>();
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = null;

            // Act and Assert
            // Check that call method with null protocol throws "ArgumentNullException".
            Assert.That(() => dailyReporter.VerifyLastStamp(stamps, targetEmployerID, targetDay, notifications), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        #endregion

        #region VerifyStampsSequence Tests

        /// <summary>
        /// Tested method should add missing stamps between consecutive ones with same type in the middle.
        /// </summary>
        [TestCase]
        public void VerifyStampsSequence_NoSomeStampsInSequence_AddNonexistentStampsInMiddle()
        {
            // Arrange
            // Target day is 01.03.2016
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>()
            {
                // In-stamp at 9.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(9) },

                // In-stamp at 11.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(9).AddMinutes(10) },

                // Out-stamp at 12.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(12) },

                // Out-stamp at 14.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(14) },
            };
            int originalStampsCount = stamps.Count;
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = new List<Notification>();
            List<EmployerTimeStamp> expectedStamps = new List<EmployerTimeStamp>()
            {
                stamps[0],

                // Should be added! Out-stamp at 11.00 am of target day.
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(9).AddMinutes(5) },

                stamps[1],
                stamps[2],

                // Should be added! In-stamp at 13.00 am of target day.
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(13) },

                stamps[3]
            };

            // Act
            dailyReporter.VerifyStampsSequence(stamps, targetEmployerID, targetDay, notifications);

            // Assertions
            // Check that collections contain same items in same order.
            Assert.That(stamps, Is.EqualTo(expectedStamps).Using(new EmployerTimeStampComparer()));
        }

        /// <summary>
        /// Tests that there won't be any changes if data is correct.
        /// </summary>
        [TestCase]
        public void VerifyStampsSequence_DataIsCorrect_NoChanges()
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
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = new List<Notification>();
            List<EmployerTimeStamp> expectedStamps = new List<EmployerTimeStamp>()
            {
                stamps[0],
                stamps[1]
            };

            // Act
            dailyReporter.VerifyStampsSequence(stamps, targetEmployerID, targetDay, notifications);

            // Assertions
            // Check that collections contain same items in same order.
            Assert.That(stamps, Is.EqualTo(expectedStamps));
        }

        /// <summary>
        /// Checks that system throw exceptions if reference to source is null.
        /// </summary>
        [TestCase]
        public void VerifyStampsSequence_CollectionOfStampsIsNull_ThrowArgumentNullException()
        {
            // Arrange
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = null;
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = new List<Notification>();

            // Act and Assert
            // Check that call method with null collection throws "ArgumentNullException".
            Assert.That(() => dailyReporter.VerifyStampsSequence(stamps, targetEmployerID, targetDay, notifications), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        /// <summary>
        /// Checks that system throw exceptions if reference to protocol is null.
        /// </summary>
        [TestCase]
        public void VerifyStampsSequence_ProtocolReferenceIsNull_ThrowArgumentNullException()
        {
            // Arrange
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>();
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);
            List<Notification> notifications = null;

            // Act and Assert
            // Check that call method with null protocol throws "ArgumentNullException".
            Assert.That(() => dailyReporter.VerifyStampsSequence(stamps, targetEmployerID, targetDay, notifications), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        #endregion

        #region DailyReportsManager Tests

        /// <summary>
        /// Checks that constructor of daily reporter throw exception 
        /// if passed reference to source is null.
        /// </summary>
        [TestCase]
        public void DailyReportsManager_StampsSourceIsNull_ThrowArgumentNullException()
        {
            Assert.That(() => new DailyReportsManager(null), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        #endregion

        #region TimeOfWorkForDay Tests

        /// <summary>
        /// Test method of computation total time of work with absolute correct data.
        /// </summary>
        [TestCase]
        public void TimeOfWorkForDay_DataCorrect_CorrectTimeOfWork()
        {
            // Arrange
            // Target day is 01.03.2016
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>()
            {
                // In-stamp at 9.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(9) },

                // Out-stamp at 11.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(10).AddMinutes(10) },

                // In-stamp at 12.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(12) },

                // Out-stamp at 14.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(18) },
            };
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);

            // Expected time of work.
            // It should 7 hours and 10 minutes.
            TimeSpan expectedTime = new TimeSpan(7, 10, 0);

            // Act
            var protocol = dailyReporter.TimeOfWorkForDay(targetEmployerID, targetDay);

            // Assertions
            // Check that computation was successful carried out.
            Assert.That(protocol.IsSucceed, Is.True);
            Assert.That(protocol.Result, Is.EqualTo(expectedTime));
        }

        /// <summary>
        /// Test method of computation total time of work if data for day does not exist.
        /// </summary>
        [TestCase]
        public void TimeOfWorkForDay_NoDataForDay_TimeOfWorkEqualsZero()
        {
            // Arrange
            // Target day is 01.03.2016
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>();
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);

            // Expected time of work.
            // It should 0 hours 0 minutes and 0 seconds.
            TimeSpan expectedTime = new TimeSpan(0);

            // Act
            var protocol = dailyReporter.TimeOfWorkForDay(targetEmployerID, targetDay);

            // Assertions
            // Check that computation was successful carried out.
            Assert.That(protocol.IsSucceed, Is.True);
            Assert.That(protocol.Result, Is.EqualTo(expectedTime));
        }

        /// <summary>
        /// Test method of computation total time of work if data for day does not exist.
        /// </summary>
        [TestCase]
        public void TimeOfWorkForDay_ExistAllIncorrectnesses_CorrectTimeOfWork()
        {
            // Arrange
            // Target day is 01.03.2016
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>()
            {
                // Out-stamp at 9.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(9) },

                // Same stamp as above.
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(9) },

                // Out-stamp at 10.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(10) },

                // In-stamp at 11.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(11) },

                // In-stamp at 12.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(12) },

                // In-stamp at 12.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(12) },

                // Out-stamp at 3.00 am of next day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddDays(1).AddHours(3) },
            };
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);

            // Expected time of work.
            // It should 25 hours 0 minutes and 0 seconds.
            TimeSpan expectedTime = new TimeSpan(25, 0, 0);

            // Act
            var protocol = dailyReporter.TimeOfWorkForDay(targetEmployerID, targetDay);

            // Assertions
            // Check that computation was successful carried out.
            Assert.That(protocol.IsSucceed, Is.True);
            Assert.That(protocol.Result, Is.EqualTo(expectedTime));
        }

        #endregion

        #region RespitesForDay Tests

        /// <summary>
        /// Test computation of respites for day if data in source is absolutely correct.
        /// </summary>
        [TestCase]
        public void RespitesForDay_DataCorrect_AllRespitesFound()
        {
            // Arrange
            // Target day is 01.03.2016
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            TimeSpan maxRespiteDuration = new TimeSpan(0, 15, 0);
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>()
            {
                // In-stamp at 9.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(9) },

                // Out-stamp at 10.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(10) },

                // In-stamp at 10.10 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(10).AddSeconds(10) },

                // Out-stamp at 12.00 pm of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(12) },

                // In-stamp at 12.15 pm of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(12).AddMinutes(15).AddSeconds(1) },

                // Out-stamp at 6.00 pm of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(18) }
            };
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);

            // All expected respites.
            // Between first and second stamps.
            // Between third and forth stamps.
            List<Respite> expectedRespites = new List<Respite>()
            {
                new Respite(stamps[1].Time, stamps[2].Time)
            };

            // Act
            var protocol = dailyReporter.RespitesForDay(targetEmployerID, targetDay, maxRespiteDuration);

            // Assertions
            // Check that computation was successful carried out.
            Assert.That(protocol.IsSucceed, Is.True);
            Assert.That(protocol.Result, Is.EqualTo(expectedRespites).Using(new RespitesComparer()));
        }

        /// <summary>
        /// Test that if there is no stamps for day then system should not find any respites.
        /// </summary>
        [TestCase]
        public void RespitesForDay_NoStampsForDay_NoRespitesFound()
        {
            // Arrange
            // Target day is 01.03.2016
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;
            TimeSpan maxRespiteDuration = new TimeSpan(0, 15, 0);
            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>();
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);

            // No expected respites.
            List<Respite> expectedRespites = new List<Respite>();

            // Act
            var protocol = dailyReporter.RespitesForDay(targetEmployerID, targetDay, maxRespiteDuration);

            // Assertions
            // Check that computation was successful carried out.
            Assert.That(protocol.IsSucceed, Is.True);
            Assert.That(protocol.Result, Is.EqualTo(expectedRespites).Using(new RespitesComparer()));
        }

        /// <summary>
        /// Test that if maximum duration of respite equals zero then system does not found any respites.
        /// Even if there is consecutive Out and In stamps with equal time because
        /// source of stamps sorts stamps by ID of type too and In-stamp type's ID less then Out one.
        /// </summary>
        [TestCase]
        public void RespitesForDay_MaxRespiteDurationEqualsZero_NoRepitesFound()
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
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(10) },

                // In-stamp at 10.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(10) },

                // Out-stamp at 12.00 pm of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(12) },

                // In-stamp at 12.15 pm of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(12).AddMinutes(15) },

                // Out-stamp at 6.00 pm of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(18) }
            };
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);

            // Maximum duration of respite.
            TimeSpan maxRespiteDuration = new TimeSpan(0, 0, 0);

            // Expected time of work.
            // No expected respites.
            List<Respite> expectedRespites = new List<Respite>();

            // Act
            var protocol = dailyReporter.RespitesForDay(targetEmployerID, targetDay, maxRespiteDuration);

            // Assertions
            // Check that computation was successful carried out.
            Assert.That(protocol.IsSucceed, Is.True);
            Assert.That(protocol.Result, Is.EqualTo(expectedRespites).Using(new RespitesComparer()));
        }

        /// <summary>
        /// Checks that if source data is incorrect than it will be completed and repaired based on built-in logic.
        /// So all respites will be found.
        /// </summary>
        [TestCase]
        public void RespitesForDay_IncorrectData_AllRespitesFound()
        {
            // Arrange
            // Target day is 01.03.2016
            DateTime targetDay = new DateTime(2016, 3, 1);
            int targetEmployerID = 1;

            // Maximum duration of respite.
            // It is 10 minutes.
            TimeSpan maxRespiteDuration = new TimeSpan(0, 10, 0);

            List<EmployerTimeStamp> stamps = new List<EmployerTimeStamp>()
            {
                // In-stamp at 9.00 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(9) },

                // Out-stamp at 9.10 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(9).Add(maxRespiteDuration) },

                // In-stamp at 9.10 am of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(9).Add(maxRespiteDuration) },

                // Out-stamp at 12.00 pm of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(12) },

                // In-stamp at 12:10:00.0001 pm of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.In, Time = targetDay.AddHours(12).Add(maxRespiteDuration).AddMilliseconds(1) },

                // Out-stamp at 6.00 pm of target day
                new EmployerTimeStamp() { EmployerID = targetEmployerID, Type = StampType.Out, Time = targetDay.AddHours(18) }
            };
            DailyReportsManager dailyReporter = this.СreateDailyReporter(stamps);

            // This list looks like this because system should do the following steps to repair data:
            //      1. Source data will be sorted and 2nd record(start with 0) will be 1st.
            //      2. Now we have two In-stamps 0 and 1st so system create new Out-stamp in the middle of these ones.
            //      3. So now we have 1st Out-stamp(was added at 2nd step) and its time equals 9 hours and half of maximum respite time minutes.
            // So obvious that difference between 1st(Out-stamp) and 2nd(In-stamp) is less 
            // than maximum respite duration because equals half of it.
            List<Respite> expectedRespites = new List<Respite>()
            {
                new Respite(targetDay.AddHours(9).AddMinutes(maxRespiteDuration.Minutes / 2),  targetDay.AddHours(9).Add(maxRespiteDuration))
            };

            // Act
            var protocol = dailyReporter.RespitesForDay(targetEmployerID, targetDay, maxRespiteDuration);

            // Assertions
            // Check that computation was successful carried out.
            Assert.That(protocol.IsSucceed, Is.True);
            Assert.That(protocol.Result, Is.EqualTo(expectedRespites).Using(new RespitesComparer()));
        }

        #endregion

        /// <summary>
        /// Create mock of employer time stamps source based on passed stamps.
        /// </summary>
        /// <returns>Instance of time stamps source which manipulates passed data.</returns>
        private IEmployerStampsSource СreateStampsSource(List<EmployerTimeStamp> stamps)
        {
            Mock<IEmployerStampsSource> mockStampsSource = new Mock<IEmployerStampsSource>();

            if (stamps != null && stamps.Count != 0)
            {
                mockStampsSource.Setup(m => m.GetAll()).Returns(stamps);

                // Grouped all stamps by employer ID and locate it all to one record.
                var stampsGroupsForEmployer = from stamp in stamps
                                              orderby stamp.Time, stamp.Type
                                              group stamp by stamp.EmployerID into empGroup
                                              select new { EmployerID = empGroup.Key, Stamps = empGroup.Select(s => s).ToList() };

                // Setup mock's GetByEmployerID method.
                foreach (var empStamps in stampsGroupsForEmployer)
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
            }
            else
            {
                mockStampsSource.Setup(m => m.GetAll()).Returns(new List<EmployerTimeStamp>());
                mockStampsSource.Setup(m => m.GetByEmployerID(It.IsAny<int>())).Returns(new List<EmployerTimeStamp>());
                mockStampsSource.Setup(m => m.GetByEmployerIDForDay(It.IsAny<int>(), It.IsAny<DateTime>())).Returns(new List<EmployerTimeStamp>());
            }

            return mockStampsSource.Object;
        }

        /// <summary>
        /// Create reporter with mock source of stamps.
        /// </summary>
        /// <param name="stamps">Stamps which will used for mock source of data.</param>
        /// <returns>Instance of <see cref="DailyReportsManager"/> with mock source of stamps.</returns>
        private DailyReportsManager СreateDailyReporter(List<EmployerTimeStamp> stamps)
        {
            return new DailyReportsManager(this.СreateStampsSource(stamps));
        }
    }
}
