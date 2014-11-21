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
        private long _lastPUPresenceTime;

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

            var distanceNow = (lastLocation.DistanceTo(primaryUser.GetLocation()));

            var avgDistanceToBaseStation = primaryUser.GetLocation().DistanceTo(baseStation.GetLocation());

            if (double.IsNaN(distanceNow))  //if distance is NaN assume it is the largest possible distance
            {
                distanceNow = terrain._leftDownCorner.DistanceTo(terrain._rightUpCorner);
            }

            UpdateDistanceBucket(distanceNow, timeBefore, timeAfter);

            Statistics.CurrentDistanceOfThePredictedAndActualPrimaryUserLocation = distanceNow;

            Statistics.AverageDistanceOfThePredictedAndActualPrimaryUserLocation
                = TakeAverage(currentDistance, distanceNow, timeBefore, timeAfter);

            Statistics.AverageDistanceToBaseStation = TakeAverage(Statistics.AverageDistanceToBaseStation, avgDistanceToBaseStation,
                timeBefore, timeAfter);
            
        }

        public void UpdateProtocolDependentPdPmPf(Terrain terrain, int timeBefore, int timeAfter)
        {
            var pu = terrain.GetAllPrimaryUsers().FirstOrDefault();
            var bs = terrain.GetBaseStation();
            
            if (pu != null)
            {
                Statistics.TotalTimeAPrimaryUserHaveExisted += (timeAfter - timeBefore);
            }

            if (bs._lastDetectionDecision == true)
            {
                Statistics.TotalTimeAPrimaryUserHaveDetected += (timeAfter - timeBefore); 
            }

            if (pu == null && bs._lastDetectionDecision == false)
            {
                Statistics.TotalTimeSpentCorrectlyInProtocol_h1_h1 += timeAfter - timeBefore;
            }
            if (pu != null && bs._lastDetectionDecision == true)
            {
                Statistics.TotalTimeSpentCorrectlyInProtocol_h0_h0 += timeAfter - timeBefore;                
            }
            if (pu == null && bs._lastDetectionDecision == true)
            {
                Statistics.TotalTimeSpentMistakenlylyInProtocol_h1_h0 += timeAfter - timeBefore;                
            }
            if (pu != null && bs._lastDetectionDecision == false)
            {
                Statistics.TotalTimeSpentMistakenlylyInProtocol_h0_h1 += timeAfter - timeBefore;
            }
        }

        //call this method from base station upon decision - do not change before saving
        public void UpdateProtocolIndependentPdPmPf(Terrain terrain, bool isPUDetected)
        {
            var pu = terrain.GetAllPrimaryUsers().FirstOrDefault();

            if (isPUDetected)
            {
                var goBackInTime = SimParameters.OperationMode == true ? 
                    SimParameters.WhisperPeriod + SimParameters.DecisionPeriod + SimParameters.ReportingPeriod :
                    SimParameters.ReportingPeriod;
                // how much time I want to look back in order to understand real value of PU presence 

                //if a pu is present and a pu is created before I go back in time
                if (pu != null)
                {
                    if ((Time.Instance.Now - goBackInTime) >= Statistics.LastTimeAPUCreated)
                    { //pu is here and not created after I sensed
                        Statistics.TruePositiveDetectionCountForPUPresence++;
                    }
                    else
                    { //pu is here but it is created after I sensed
                        Statistics.FalsePositiveDetectionCountForPUPresence++;
                    }
                }
                if (pu == null)
                {
                    if ((Time.Instance.Now - goBackInTime) >= Statistics.LastTimeAPURemoved)
                    { // pu is not here and it was not here when I sensed
                        Statistics.TruePositiveDetectionCountForPUPresence++;
                    }
                    else
                    {  // pu is not here however it is removed after I sensed
                        Statistics.FalsePositiveDetectionCountForPUPresence++;
                    }
                }
            }
            else //pu is not detected
            {
                var goBackInTime = SimParameters.OperationMode == true ?
                    SimParameters.WhisperPeriod + SimParameters.DecisionPeriod + SimParameters.ReportingPeriod :
                    SimParameters.ReportingPeriod;
                // how much time I want to look back in order to understand real value of PU presence 

                //if a pu is present and a pu is created before I go back in time
                if (pu != null)
                {
                    if ((Time.Instance.Now - goBackInTime) >= Statistics.LastTimeAPUCreated)
                    { //pu is here and not created after I sensed
                        Statistics.TrueNegativeDetectionCountForPUPresence++;
                    }
                    else
                    { //pu is here but it is created after I sensed
                        Statistics.FalseNegativeDetectionCountForPUPresence++;
                    }
                }
                if (pu == null)
                {
                    if ((Time.Instance.Now - goBackInTime) >= Statistics.LastTimeAPURemoved)
                    { // pu is not here and it was not here when I sensed
                        Statistics.TrueNegativeDetectionCountForPUPresence++;
                    }
                    else
                    {  // pu is not here however it is removed after I sensed
                        Statistics.FalseNegativeDetectionCountForPUPresence++;
                    }
                }
            }

            Statistics.TotalDetectionCountForPUPresence++;
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

        public void UpdateTotalPowerForWhispering(Terrain terrain)
        {
            Statistics.TotalPowerWhispering = terrain.GetMobileStations().Sum(m => m._totalPowerWhispering);
        }

        public void UpdateTotalPowerForReporting(Terrain terrain)
        {
            Statistics.TotalPowerReporting = terrain.GetMobileStations().Sum(m => m._totalPowerReporting);
        }

        public void UpdateAverageNumberOfWhisperingAttemptsFailed(Terrain terrain, int timeBefore, int timeAfter)
        {
            var currentCount = terrain.GetMobileStations().Count(ms=>ms._whisperingFailed == true);

            Statistics.AverageNumberOfWhisperingAttemptsFailed = TakeAverage(Statistics.AverageNumberOfWhisperingAttemptsFailed, currentCount, timeBefore, timeAfter);
        }

        public void UpdateAverageWhisperRadius(Terrain terrain, int timeBefore, int timeAfter)
        {
            var currentRadius = terrain.GetMobileStations().First()._whisperRadius;

            Statistics.AverageWhisperRadius = TakeAverage(Statistics.AverageWhisperRadius, currentRadius, timeBefore, timeAfter);
        }

        public void UpdateProbabilityOfFalseDetection(Terrain terrain, int timeBefore, int timeAfter)
        {
            var currentFalseDetection = timeAfter - timeBefore;

            Statistics.ProbabilityOfFalseAlarm = TakeAverage(Statistics.ProbabilityOfFalseAlarm,
                currentFalseDetection, timeBefore, timeAfter);
        }
    }
}
