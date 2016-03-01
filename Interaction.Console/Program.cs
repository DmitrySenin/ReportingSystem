namespace Interaction.Console
{
    using System;
    using System.Collections.Generic;

    using DataSource.StampsCollection;

    using ReportingSystem.Core;
    using ReportingSystem.Reports;

    class Program
    {
        static void Main(string[] args)
        {
            DailyReportsManager dailyReporter = new DailyReportsManager(buildStampsSource());

            ReportProtocol<TimeSpan> dayWork = dailyReporter.TimeOfWorkForDay(targetEmployer(), tagetDay());
            ReportProtocol<List<Respite>> dayRespites = dailyReporter.RespitesForDay(targetEmployer(), tagetDay(), maxRespiteDuration());

            Console.WriteLine("Time for day:");
            if (dayWork.IsSucceed)
            {
                Console.WriteLine("\t{0} hrs.", dayWork.Result.TotalHours);
                Console.WriteLine("\tNotifications count: {0}", dayWork.Notifications.Count);
            }
            else
            {
                Console.WriteLine("\tError was occurred. Reporting end unsuccessfully.");
            }
            foreach (var n in dayWork.Notifications)
            {
                Console.WriteLine("\tType: {0}. Message: {1}", n.Type, n.Description);
            }


            Console.WriteLine("Respites for day:");
            if (dayRespites.IsSucceed)
            {
                Console.WriteLine("\tAmount of respites: {0}", dayRespites.Result.Count);
                Console.WriteLine("\tNotifications count: {0}", dayRespites.Notifications.Count);
            }
            else
            {
                Console.WriteLine("\tError was occurred. Reporting end unsuccessfully.");
            }
            foreach (var n in dayRespites.Notifications)
            {
                Console.WriteLine("\tType: {0}. Message: {1}", n.Type, n.Description);
            }

            Console.ReadKey();
        }

        private static IEmployerStampsSource buildStampsSource()
        {
            return new StampsSource();
        }

        private static int targetEmployer()
        {
            return 1;
        }

        private static DateTime tagetDay()
        {
            return new DateTime(2016, 2, 29);
        }

        private static TimeSpan maxRespiteDuration()
        {
            // 15 min. respite duration.
            return new TimeSpan(0, 15, 0);
        }
    }
}
