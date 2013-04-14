﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib
{
    public static class Statistics
    {
        static Statistics()
        {
            AverageDistanceOfThePredictedAndActualPrimaryUserLocation = 0;
            CurrentDistanceOfThePredictedAndActualPrimaryUserLocation = 0;
            AverageNumberOfMSThatDetectedPrimaryUserPresence = 0;
            AverageNumberOfMSThatReportedPrimaryUserPresence = 0;
            DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent = new long[20]; //ten buckets
            TotalTimeAPrimaryUserHaveExisted = 0;
            AverageWhisperRadius = 0;
        }

        public static double AverageDistanceOfThePredictedAndActualPrimaryUserLocation { get; internal set; }

        public static double CurrentDistanceOfThePredictedAndActualPrimaryUserLocation { get; internal set; }

        public static double AverageNumberOfMSThatDetectedPrimaryUserPresence { get; internal set; }

        public static double AverageNumberOfMSThatReportedPrimaryUserPresence { get; internal set; }

        public static long[] DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent { get; internal set; }

        public static long TotalTimeAPrimaryUserHaveExisted { get; internal set; }

        public static long TotalTimeAPrimaryUserHaveDetected { get; internal set; }

        public static double AverageWhisperRadius { get; internal set; }
    }
}
