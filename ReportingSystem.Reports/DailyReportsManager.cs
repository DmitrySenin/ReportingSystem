namespace ReportingSystem.Reports
{
    using System;
    using System.Collections.Generic;

    using ReportingSystem.Core;

    /// <summary>
    /// Makes daily reports.
    /// </summary>
    public class DailyReportsManager
    {
        /// <summary>
        /// Source of employers time stamps to get information for reports.
        /// </summary>
        private readonly IEmployerStampsSource employerStampsSource;

        /// <summary>
        /// Create instance of <see cref="DailyReportsManager"/> class.
        /// </summary>
        /// <param name="employerStampsSource">Connection between report manager and source for reporting.</param>
        /// <exception cref="ArgumentNullException">Thrown when passed stamps source is null reference.</exception>
        public DailyReportsManager(IEmployerStampsSource employerStampsSource)
        {
            if (employerStampsSource == null)
            {
                throw new ArgumentNullException(ReporterMessages.StampsSourceReferenceIsNull);
            }

            this.employerStampsSource = employerStampsSource;
        }

        /// <summary>
        /// Computes time of work for concrete day of some employer.
        /// </summary>
        /// <param name="employerID">Unique identifier of employer.</param>
        /// <param name="day">Date of day of computation.</param>
        /// <returns>Protocol that collect result of computation and notifications of system.</returns>
        public ReportProtocol<TimeSpan> TimeOfWorkForDay(int employerID, DateTime day)
        {
            ReportProtocol<TimeSpan> protocol = new ReportProtocol<TimeSpan>();

            List<EmployerTimeStamp> employerStamps = this.CollectStapmsForDailyReport(employerID, day, protocol.Notifications);

            // There is errors that can't be repaired.
            if (protocol.GetNotificationsOfType(NotificationType.Error).Count != 0)
            {
                protocol.IsSucceed = false;
                return protocol;
            }

            if (employerStamps.Count == 0)
            {
                protocol.Notifications.Add(new Notification(ReporterMessages.EmployerStampsNotFound, NotificationType.Message));
            }

            TimeSpan result = new TimeSpan(0);

            for (int i = 0; i < employerStamps.Count - 1; i += 2)
            {
                result += employerStamps[i + 1].Time - employerStamps[i].Time;
            }

            protocol.Result = result;
            protocol.IsSucceed = true;

            return protocol;
        }

        /// <summary>
        /// Find all respites of employer for day.
        /// </summary>
        /// <param name="employerID">Unique identifier of employer.</param>
        /// <param name="day">Target day.</param>
        /// <param name="maxDuration">Maximum duration of respite.</param>
        /// <returns>Collection of all respites.</returns>
        public ReportProtocol<List<Respite>> RespitesForDay(int employerID, DateTime day, TimeSpan maxDuration)
        {
            ReportProtocol<List<Respite>> protocol = new ReportProtocol<List<Respite>>();

            List<EmployerTimeStamp> employerStamps = this.CollectStapmsForDailyReport(employerID, day, protocol.Notifications);

            // There is errors that can't be repaired.
            if (protocol.GetNotificationsOfType(NotificationType.Error).Count != 0)
            {
                protocol.IsSucceed = false;
                return protocol;
            }

            if (employerStamps.Count == 0)
            {
                protocol.Notifications.Add(new Notification(ReporterMessages.EmployerStampsNotFound, NotificationType.Message));
            }

            List<Respite> result = new List<Respite>();

            // Start with i equals 1 because we interested of when
            // employer Out and when In but first stamp is In.
            for (int i = 1; i < employerStamps.Count - 1; i += 2)
            {
                var duration = employerStamps[i + 1].Time - employerStamps[i].Time;

                if (duration <= maxDuration)
                {
                    result.Add(new Respite(employerStamps[i].Time, employerStamps[i + 1].Time));
                }
            }

            protocol.Result = result;
            protocol.IsSucceed = true;

            return protocol;
        }

        /// <summary>
        /// Gathers all data for daily report, check it and complete if necessary.
        /// Add important notification to protocol.
        /// </summary>
        /// <param name="employerID">Target employer's unique identifier.</param>
        /// <param name="day">Reporting day.</param>
        /// <param name="notifications">Collection of notifications that will be filled during processing of collecting.</param>
        /// <returns>Checked stamps for daily reporting.</returns>
        internal List<EmployerTimeStamp> CollectStapmsForDailyReport(int employerID, DateTime day, List<Notification> notifications)
        {
            if(notifications == null)
            {
                throw new ArgumentNullException(ReporterMessages.ProtocolReferenceIsNull); 
            }

            List<EmployerTimeStamp> employerStamps = this.employerStampsSource.GetByEmployerIDForDay(employerID, day);

            if (employerStamps.Count == 0)
            {
                return employerStamps;
            }

            this.VerifyFirstStamp(employerStamps, employerID, day, notifications);
            this.VerifyLastStamp(employerStamps, employerID, day, notifications);
            this.VerifyStampsSequence(employerStamps, employerID, day, notifications);

            return employerStamps;
        }

        /// <summary>
        /// Check out first element of collection of stamps 
        /// to add if require non existent first In-stamp.
        /// </summary>
        /// <param name="employerStamps">Collection that should be checked.</param>
        /// <param name="employerID">Unique identifier of target employer.</param>
        /// <param name="day">Date of day of reporting.</param>
        /// <param name="notifications">Collection of notifications that will be filled during processing of collecting.</param>
        internal void VerifyFirstStamp(List<EmployerTimeStamp> employerStamps, int employerID, DateTime day, List<Notification> notifications)
        {
            if (employerStamps == null)
            {
                throw new ArgumentNullException(ReporterMessages.StampsCollectionReferenceIsNull);
            }

            if (notifications == null)
            {
                throw new ArgumentNullException(ReporterMessages.ProtocolReferenceIsNull);
            }

            // if first record is not in-stamp then insert begging of target day instead of it.
            if (employerStamps.Count == 0 || employerStamps[0].Type != StampType.In)
            {
                notifications.Add(new Notification(ReporterMessages.FirstInStampNotFound, NotificationType.Warning));
                notifications.Add(new Notification(ReporterMessages.BeginDayAsInStampAdded, NotificationType.Message));

                EmployerTimeStamp firstInStamp = new EmployerTimeStamp();
                firstInStamp.EmployerID = employerID;
                firstInStamp.Type = StampType.In;

                // set beginning of target day.
                firstInStamp.Time = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);

                employerStamps.Insert(0, firstInStamp);
            }
        }

        /// <summary>
        /// Check out last element of collection of stamps 
        /// to add if require non existent last out-stamp.
        /// </summary>
        /// <param name="employerStamps">Collection that should be checked.</param>
        /// <param name="employerID">Unique identifier of target employer.</param>
        /// <param name="day">Date of day of reporting.</param>
        /// <param name="notifications">Collection of notifications that will be filled during processing of collecting.</param>
        internal void VerifyLastStamp(List<EmployerTimeStamp> employerStamps, int employerID, DateTime day, List<Notification> notifications)
        {
            if (employerStamps == null)
            {
                throw new ArgumentNullException(ReporterMessages.StampsCollectionReferenceIsNull);
            }

            if (notifications == null)
            {
                throw new ArgumentNullException(ReporterMessages.ProtocolReferenceIsNull);
            }

            // if last record is not out-stamp.
            if (employerStamps.Count == 0 || employerStamps[employerStamps.Count - 1].Type != StampType.Out)
            {
                notifications.Add(new Notification(ReporterMessages.LastOutStampNotFound, NotificationType.Warning));

                // Date of next day.
                DateTime nextDay = day.AddDays(1);

                // Find records for next day.
                List<EmployerTimeStamp> employerStampsForNextDay = this.employerStampsSource.GetByEmployerIDForDay(employerID, nextDay);
                DateTime nextDayFindingDate = this.NextDayEarliestFindingTime(day);

                // First stamp of next day is Out-stamp and satisfies restriction time.
                if (employerStampsForNextDay != null && employerStampsForNextDay.Count > 0
                    && employerStampsForNextDay[0].Type == StampType.Out && employerStampsForNextDay[0].Time <= nextDayFindingDate)
                {
                    notifications.Add(new Notification(ReporterMessages.FirstOutNextDayAsLast, NotificationType.Message));

                    employerStamps.Add(employerStampsForNextDay[0]);
                }
                else
                {
                    notifications.Add(new Notification(ReporterMessages.EndDayAsLastOutStamp, NotificationType.Message));

                    // Added stamp.
                    EmployerTimeStamp lastOutStamp = new EmployerTimeStamp();
                    lastOutStamp.EmployerID = employerID;
                    lastOutStamp.Type = StampType.Out;

                    // End of target day.
                    lastOutStamp.Time = new DateTime(day.Year, day.Month, day.Day, 23, 59, 59);

                    employerStamps.Add(lastOutStamp);
                }
            }
        }

        /// <summary>
        /// Check out sequence of stamps in collection
        /// that each In-stamp is followed by Out-stamp.
        /// </summary>
        /// <param name="employerStamps">Collection that should be checked.</param>
        /// <param name="employerID">Unique identifier of target employer.</param>
        /// <param name="day">Date of day of reporting.</param>
        /// <param name="notifications">Collection of notifications that will be filled during processing of collecting.</param>
        internal void VerifyStampsSequence(List<EmployerTimeStamp> employerStamps, int employerID, DateTime day, List<Notification> notifications)
        {
            if (employerStamps == null)
            {
                throw new ArgumentNullException(ReporterMessages.StampsCollectionReferenceIsNull);
            }

            if (notifications == null)
            {
                throw new ArgumentNullException(ReporterMessages.ProtocolReferenceIsNull);
            }

            int i = 0;
            while(i < employerStamps.Count - 1)
            {
                if (employerStamps[i].Type == employerStamps[i + 1].Type)
                {
                    if (employerStamps[i].Time == employerStamps[i + 1].Time)
                    {
                        notifications.Add(new Notification(ReporterMessages.OneOfEqualStampsRemoved, NotificationType.Warning));
                        employerStamps.RemoveAt(i + 1);

                        // Indexer should not be changed because 
                        // new (i + 1) element can be same type and time again.
                    }
                    else
                    {
                        notifications.Add(new Notification(ReporterMessages.NewStampInMiddleBetweenSameType, NotificationType.Warning));

                        // Compute difference in milliseconds.
                        double diffInMilliseconds = (employerStamps[i + 1].Time - employerStamps[i].Time).TotalMilliseconds;

                        EmployerTimeStamp middleStamp = new EmployerTimeStamp();
                        middleStamp.EmployerID = employerID;
                        middleStamp.Type = employerStamps[i].Type == StampType.In ? StampType.Out : StampType.In;

                        // Add half of difference to earlier date.
                        middleStamp.Time = employerStamps[i].Time + TimeSpan.FromMilliseconds(diffInMilliseconds / 2);

                        employerStamps.Insert(i + 1, middleStamp);

                        // Go to element that was (i + 1) before insert.
                        i += 2;
                    }
                }
                else
                {
                    i++;
                }
            }
        }

        /// <summary>
        /// Build time of employer stamps to find records in next day.
        /// </summary>
        /// <param name="currentDay">Date of "current" day.</param>
        /// <returns>Time to restrict finding of data.</returns>
        internal DateTime NextDayEarliestFindingTime(DateTime currentDay)
        {
            // 4 am of next day.
            DateTime nextDayTime = currentDay.AddDays(1);
            return new DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 4, 0, 0);
        }
    }
}
