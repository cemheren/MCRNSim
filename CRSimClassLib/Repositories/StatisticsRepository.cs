using CRSimClassLib.TerrainModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib.Repositories
{
    public class StatisticsRepository
    {
        private static double TakeAverage(double presentAverage, double currentMeasured, int timeBefore, int timeAfter)
        {
            if (timeAfter == 0)
            {
                return 0;
            }

            var newValue = presentAverage * timeBefore + (timeAfter - timeBefore) * currentMeasured;

            return newValue / timeAfter;
        }

        public void UpdateDistanceBucket(double distanceNow, int timeBefore, int timeAfter)
        {
            var index = (int)distanceNow / 10;   //ten meters per bucket

            if (double.IsNaN(distanceNow))
            {
                Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent
                    [Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent.Length - 1] += timeAfter - timeBefore;
                return;                    
            }

            if (index > Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent.Length - 1)
            {
                index = Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent.Length - 1;
            }
            Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent[index] += timeAfter - timeBefore;
        }

        public void UpdateAverageDistance(Terrain terrain, int timeBefore, int timeAfter)
        {
            var currentDistance = Statistics.AverageDistanceOfThePredictedAndActualPrimaryUserLocation;

            var primaryUser = terrain.GetAllPrimaryUsers().FirstOrDefault();
            if (primaryUser == null)
            {
                return;
            }

            Statistics.TotalTimeAPrimaryUserHaveExisted += (timeAfter - timeBefore);

            var baseStation = terrain.GetBaseStation();
            var lastLocation = baseStation.GetLastEstimatedLocationOfPU();
            if (lastLocation == null)
            {
                lastLocation = baseStation.GetLastEstimatedLocationOfPUMean();
                if (lastLocation == null)
                {
                    return;
                }
            }

            Statistics.TotalTimeAPrimaryUserHaveDetected += (timeAfter - timeBefore); 

            var distanceNow = (lastLocation.DistanceTo(primaryUser.GetLocation()));

            UpdateDistanceBucket(distanceNow, timeBefore, timeAfter);

            Statistics.CurrentDistanceOfThePredictedAndActualPrimaryUserLocation = distanceNow;

            Statistics.AverageDistanceOfThePredictedAndActualPrimaryUserLocation
                = TakeAverage(currentDistance, distanceNow, timeBefore, timeAfter);
        }

        public void UpdateMobileStationDetectionStats(Terrain terrain, int timeBefore, int timeAfter)
        {
            var primaryUser = terrain.GetAllPrimaryUsers().FirstOrDefault();
            if (primaryUser == null)
            {
                return;
            }
            var baseStation = terrain.GetBaseStation();

            var numberOfDetectedStations = terrain.GetMobileStations().Count(ms => ms.GetPreviousDetectionDecision());

            Statistics.AverageNumberOfMSThatDetectedPrimaryUserPresence
                = TakeAverage(Statistics.AverageNumberOfMSThatDetectedPrimaryUserPresence,
                numberOfDetectedStations, timeBefore, timeAfter);

            var numberOfReportedStations = baseStation.GetLastReportedDataCount();

            Statistics.AverageNumberOfMSThatReportedPrimaryUserPresence
                = TakeAverage(Statistics.AverageNumberOfMSThatReportedPrimaryUserPresence,
                numberOfReportedStations, timeBefore, timeAfter);
        }

        public void UpdateAverageWhisperRadius(Terrain terrain, int timeBefore, int timeAfter)
        {
            var currentRadius = terrain.GetMobileStations().First()._whisperRadius;

            Statistics.AverageWhisperRadius = TakeAverage(Statistics.AverageWhisperRadius, currentRadius, timeBefore, timeAfter);
        }
    }
}
