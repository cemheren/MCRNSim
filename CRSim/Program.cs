using CRSimClassLib;
using CRSimClassLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CRSim
{
    class Program
    {
        private static int _numberOfStations = 300;
        private static int _width = 500;
        private static int _height = 500;

        static void Main(string[] args)
        {
            var simulationTime = 500000 * 1000; //10,000 secs

            Simulation.InitializeSimulation(simulationTime, _numberOfStations, null, _width, _height);

            //Simulation.DoOnTimeTick = DoOnTimeTick;

            Thread t = new Thread(() => MainLoop());

            t.Start();

            while (Simulation.EndCondition == false)
            {
                var a = Console.ReadLine();
                PrintStatistics();
            }
        }

        public static void MainLoop()
        {
            while (Simulation.EndCondition == false)
            {
                Simulation.DequeueEvent();
            }
            PrintStatistics();
            DumpStatisticsToFile();
        }

        public static void DoOnTimeTick()
        {
            if (Time.Instance.Now > 75000)
            {
                var a = Simulation.EndCondition;
            }
        }

        private static string GetFormattedStatistics()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(
                "Number of stations : " + _numberOfStations
                + "\n\rArea dimensions : " + _width +" x "+ _height
                + ""
                + ""
                + ""
                + "\n\rAverage distance diffence = " + Statistics.AverageDistanceOfThePredictedAndActualPrimaryUserLocation
                + " meters \n\rCurrent time = " + Time.Instance.GetTimeInSeconds().ToString("#,##0") + " seconds"
                + "\n\rAverage # of Detected Mobile Stations : " + Statistics.AverageNumberOfMSThatDetectedPrimaryUserPresence
                + "\n\rAverage # of Reported Mobile Stations : " + Statistics.AverageNumberOfMSThatReportedPrimaryUserPresence
                + "\n\rTotal time primary user existed: " + ((double)Statistics.TotalTimeAPrimaryUserHaveExisted / 1000).ToString()
                + "\n\rTotal time primary user detected: " + ((double)Statistics.TotalTimeAPrimaryUserHaveDetected / 1000).ToString()
                + "\n\rAverage whisper radius: " + Statistics.AverageWhisperRadius.ToString()
                + "\n\r");

            stringBuilder.AppendLine("Distance bucket in terms of time (seconds and percentage):");
            for (int i = 0; i < Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent.Length; i++)
            {
                stringBuilder.Append(((i) * 10).ToString() + "-" + (Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent[i] / 1000).ToString()
                    + " ~ " + (Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent[i] * 100.0 / Statistics.TotalTimeAPrimaryUserHaveDetected + "%")
                    + "\n\r");
            }

            return stringBuilder.ToString();
        }

        private static void PrintStatistics()
        {
            var statistics = GetFormattedStatistics();

            Console.WriteLine(statistics);
        }

        private static void DumpStatisticsToFile()
        {
            LoggerRepository.Instance.AppendLog(GetFormattedStatistics());
        }

    }
}
