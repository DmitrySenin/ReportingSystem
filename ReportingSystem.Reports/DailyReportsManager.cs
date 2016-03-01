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

            List<EmployerTimeStamp> employerStamps = this.collectStapmsForDailyReport<TimeSpan>(employerID, day, protocol);

            // There is errors that can't be repaired.
            if (protocol.NotificationsByType(NotificationType.Error).Count != 0)
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

            List<EmployerTimeStamp> employerStamps = this.collectStapmsForDailyReport<List<Respite>>(employerID, day, protocol);

            // There is errors that can't be repaired.
            if (protocol.NotificationsByType(NotificationType.Error).Count != 0)
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
        /// <typeparam name="T">Type of protocol result.</typeparam>
        /// <param name="employerID">Target employer's unique identifier.</param>
        /// <param name="day">Reporting day.</param>
        /// <param name="protocol">Protocol of reporting.</param>
        /// <returns>Checked stamps for daily reporting.</returns>
        internal List<EmployerTimeStamp> collectStapmsForDailyReport<T>(int employerID, DateTime day, ReportProtocol<T> protocol)
        {
            if(protocol == null)
            {
                throw new ArgumentNullException(ReporterMessages.ProtocolReferenceIsNull); 
            }

            List<EmployerTimeStamp> employerStamps = this.employerStampsSource.GetByEmployerIDForDay(employerID, day);

            if (employerStamps.Count == 0)
            {
                return employerStamps;
            }

            this.verifyFirstStamp<T>(employerStamps, employerID, day, protocol);
            this.verifyLastStamp<T>(employerStamps, employerID, day, protocol);
            this.verifyStampsSequence<T>(employerStamps, employerID, day, protocol);

            return employerStamps;
        }

        /// <summary>
        /// Check out first element of collection of stamps 
        /// to add if require non existent first In-stamp.
        /// </summary>
        /// <typeparam name="T">Type of result of protocol.</typeparam>
        /// <param name="employerStamps">Collection that should be checked.</param>
        /// <param name="employerID">Unique identifier of target employer.</param>
        /// <param name="day">Date of day of reporting.</param>
        /// <param name="protocol">Protocol of reporting.</param>
        internal void verifyFirstStamp<T>(List<EmployerTimeStamp> employerStamps, int employerID, DateTime day, ReportProtocol<T> protocol)
        {
            if (employerStamps == null)
            {
                throw new ArgumentNullException(ReporterMessages.StampsCollectionReferenceIsNull);
            }

            if (protocol == null)
            {
                throw new ArgumentNullException(ReporterMessages.ProtocolReferenceIsNull);
            }

            // if first record is not in-stamp.
            if (employerStamps.Count == 0 || employerStamps[0].Type != StampType.In)
            {
                protocol.Notifications.Add(new Notification(ReporterMessages.FirstInStampNotFound, NotificationType.Warning));
                protocol.Notifications.Add(new Notification(ReporterMessages.BeginDayAsInStampAdded, NotificationType.Message));

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
        /// <typeparam name="T">Type of result of protocol.</typeparam>
        /// <param name="employerStamps">Collection that should be checked.</param>
        /// <param name="employerID">Unique identifier of target employer.</param>
        /// <param name="day">Date of day of reporting.</param>
        /// <param name="protocol">Protocol of reporting.</param>
        internal void verifyLastStamp<T>(List<EmployerTimeStamp> employerStamps, int employerID, DateTime day, ReportProtocol<T> protocol)
        {
            if (employerStamps == null)
            {
                throw new ArgumentNullException(ReporterMessages.StampsCollectionReferenceIsNull);
            }

            if (protocol == null)
            {
                throw new ArgumentNullException(ReporterMessages.ProtocolReferenceIsNull);
            }

            // if last record is not out-stamp.
            if (employerStamps.Count == 0 || employerStamps[employerStamps.Count - 1].Type != StampType.Out)
            {
                protocol.Notifications.Add(new Notification(ReporterMessages.LastOutStampNotFound, NotificationType.Warning));

                // Date of next day.
                DateTime nextDay = day.AddDays(1);

                // Find records for next day.
                List<EmployerTimeStamp> employerStampsForNextDay = this.employerStampsSource.GetByEmployerIDForDay(employerID, nextDay);
                DateTime nextDayFindingDate = this.nextDayEarliestFindingTime(day);

                // First stamp of next day is Out-stamp and satisfies restriction time.
                if (employerStampsForNextDay != null && employerStampsForNextDay.Count > 0
                    && employerStampsForNextDay[0].Type == StampType.Out && employerStampsForNextDay[0].Time <= nextDayFindingDate)
                {
                    protocol.Notifications.Add(new Notification(ReporterMessages.FirstOutNextDayAsLast, NotificationType.Message));

                    employerStamps.Add(employerStampsForNextDay[0]);
                }
                else
                {
                    protocol.Notifications.Add(new Notification(ReporterMessages.EndDayAsLastOutStamp, NotificationType.Message));

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
        /// <typeparam name="T">Type of result of protocol.</typeparam>
        /// <param name="employerStamps">Collection that should be checked.</param>
        /// <param name="employerID">Unique identifier of target employer.</param>
        /// <param name="day">Date of day of reporting.</param>
        /// <param name="protocol">Protocol of reporting.</param>
        internal void verifyStampsSequence<T>(List<EmployerTimeStamp> employerStamps, int employerID, DateTime day, ReportProtocol<T> protocol)
        {
            if (employerStamps == null)
            {
                throw new ArgumentNullException(ReporterMessages.StampsCollectionReferenceIsNull);
            }

            if (protocol == null)
            {
                throw new ArgumentNullException(ReporterMessages.ProtocolReferenceIsNull);
            }

            for (int i = 0; i < employerStamps.Count - 1; )
            {
                if (employerStamps[i].Type == employerStamps[i + 1].Type)
                {
                    if (employerStamps[i].Time == employerStamps[i + 1].Time)
                    {
                        protocol.Notifications.Add(new Notification(ReporterMessages.OneOfEqualStampsRemoved, NotificationType.Warning));
                        employerStamps.RemoveAt(i + 1);

                        // Indexer should not be changed because 
                        // new (i + 1) element can be same type and time again.
                    }
                    else
                    {
                        protocol.Notifications.Add(new Notification(ReporterMessages.NewStampInMiddleBetweenSameType, NotificationType.Warning));

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
        internal DateTime nextDayEarliestFindingTime(DateTime currentDay)
        {
            // 4 am of next day.
            DateTime nextDayTime = currentDay.AddDays(1);
            return new DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 4, 0, 0);
        }
    }
}
