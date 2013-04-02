using CRSimClassLib;
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
        static void Main(string[] args)
        {
            var numberOfStations = 300;

            var simulationTime = 80000 * 1000; //10,000 secs

            Simulation.InitializeSimulation(simulationTime, numberOfStations, 5, 500, 500);

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
        }

        public static void DoOnTimeTick()
        {
            if (Time.Instance.Now > 75000)
            {
                var a = Simulation.EndCondition;
            }
        }

        private static void PrintStatistics()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("Average distance diffence = " + Statistics.AverageDistanceOfThePredictedAndActualPrimaryUserLocation
                + " meters \nCurrent time = " + Time.Instance.GetTimeInSeconds().ToString("#,##0") + " seconds\n"
                + "Average # of Detected Mobile Stations : " + Statistics.AverageNumberOfMSThatDetectedPrimaryUserPresence
                + "\nAverage # of Reported Mobile Stations : " + Statistics.AverageNumberOfMSThatReportedPrimaryUserPresence
                + "\nTotal time primary user existed: " + ((double)Statistics.TotalTimeAPrimaryUserHaveExisted / 1000).ToString()
                + "\nTotal time primary user detected: " + ((double)Statistics.TotalTimeAPrimaryUserHaveDetected / 1000).ToString()
                + "\nAverage whisper radius: " + Statistics.AverageWhisperRadius.ToString()
                + "\n");

            stringBuilder.AppendLine("Distance bucket in terms of time (seconds and percentage):");
            for (int i = 0; i < Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent.Length; i++)
            {
                stringBuilder.Append(((i)*10).ToString() + "-" + (Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent[i] / 1000).ToString()
                    + " ~ " + (Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent[i] * 100 / Time.Instance.Now + "%")
                    + "\n");
            }

            Console.WriteLine(stringBuilder.ToString());
        }
    }
}
