using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib
{
    public static class Statistics
    {
        public static void InitStatistics()
        {
            AverageDistanceOfThePredictedAndActualPrimaryUserLocation = 0;
            CurrentDistanceOfThePredictedAndActualPrimaryUserLocation = 0;
            AverageNumberOfMSThatDetectedPrimaryUserPresence = 0;
            AverageNumberOfMSThatReportedPrimaryUserPresence = 0;
            DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent = new long[20]; //ten buckets
            TotalTimeAPrimaryUserHaveDetected = 0;
            TotalTimeAPrimaryUserHaveExisted = 0;
            AverageWhisperRadius = 0;
            TotalPowerWhispering = 0;
            TotalPowerReporting = 0;
            ProbabilityOfFalseAlarm = 0;
            ProbabilityOfDetection = 0;
            ProbabilityOfMissDetection = 0;
            LastTimeAPUCreated = 0;
            LastTimeAPURemoved = 0;
            TruePositiveDetectionCountForPUPresence = 0;
            FalsePositiveDetectionCountForPUPresence = 0;
            TrueNegativeDetectionCountForPUPresence = 0;
            FalseNegativeDetectionCountForPUPresence = 0;
            TotalDetectionCountForPUPresence = 0;
            TotalTimeSpentCorrectlyInProtocol_h0_h0 = 0;
            TotalTimeSpentCorrectlyInProtocol_h1_h1 = 0;
            TotalTimeSpentMistakenlylyInProtocol_h1_h0 = 0;
            TotalTimeSpentMistakenlylyInProtocol_h0_h1 = 0;
            AverageDistanceToBaseStation = 0;
            AverageNumberOfWhisperingAttemptsFailed = 0;
        }

        static Statistics()
        {
            InitStatistics();
        }

        public static double AverageNumberOfWhisperingAttemptsFailed { get; set; }

        public static double AverageDistanceOfThePredictedAndActualPrimaryUserLocation { get; internal set; }

        public static double CurrentDistanceOfThePredictedAndActualPrimaryUserLocation { get; internal set; }

        public static double AverageNumberOfMSThatDetectedPrimaryUserPresence { get; internal set; }

        public static double AverageNumberOfMSThatReportedPrimaryUserPresence { get; internal set; }

        public static long[] DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent { get; internal set; }

        public static long TotalTimeAPrimaryUserHaveExisted { get; internal set; }

        public static long TotalTimeAPrimaryUserHaveDetected { get; internal set; }

        public static double AverageWhisperRadius { get; internal set; }

        public static double TotalPowerWhispering { get; internal set; }

        public static double TotalPowerReporting { get; internal set; }

        public static double ProbabilityOfFalseAlarm { get; internal set; }

        public static double ProbabilityOfDetection { get; internal set; }

        public static double ProbabilityOfMissDetection { get; internal set; }

        public static long LastTimeAPURemoved { get; internal set; }

        public static long LastTimeAPUCreated { get; internal set; }

        public static int TruePositiveDetectionCountForPUPresence { get; internal set; }

        public static int FalsePositiveDetectionCountForPUPresence { get; internal set; }

        public static int TrueNegativeDetectionCountForPUPresence { get; internal set; }

        public static int FalseNegativeDetectionCountForPUPresence { get; internal set; }
        
        public static int TotalDetectionCountForPUPresence { get; internal set; }

        public static long TotalTimeSpentCorrectlyInProtocol_h0_h0 { get; internal set; }

        public static long TotalTimeSpentCorrectlyInProtocol_h1_h1 { get; internal set; }

        public static long TotalTimeSpentMistakenlylyInProtocol_h0_h1 { get; internal set; }

        public static long TotalTimeSpentMistakenlylyInProtocol_h1_h0 { get; internal set; }

        public static double AverageDistanceToBaseStation { get; internal set; }

    }
}
