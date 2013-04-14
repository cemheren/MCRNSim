using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CRSimClassLib.TerrainModal
{
    public class MSData
    {
        public double LastDetectedPower { get; set; }
        public double LastDetectionTime { get; set; }
        public TerrainPoint Location { get; set; }

        public MSData(double power, double time, TerrainPoint location)
        {
            LastDetectedPower = power;
            LastDetectionTime = time;
            Location = location;
        }
    }

    public class BaseStation : Station
    {
        private Dictionary<int,MSData> _MSDataList;
        private int _calculatePUTimeInterval = SimParameters.BSCalculateTimeInterval;
        private int _discardPreviousReportingDataInterval = SimParameters.BSDiscardPreviousReportingDataInterval;

        private TerrainPoint _lastEstimatedLocationOfPU;
        private TerrainPoint _estimatedviamean;

        public BaseStation(double x, double y) : base(x,y)
        {
            _MSDataList = new Dictionary<int, MSData>();

            Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(_calculatePUTimeInterval + 300), CalculatePUposition));
            Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(_discardPreviousReportingDataInterval + 200), DiscardPreviousReportingData));            
        }

        public void ReceiveReporting(ReportingFrameModal reportingFrame)
        { 
            // if I receive report the ms already decided positive
            if (_MSDataList.ContainsKey(reportingFrame.MS_ID))
            {
                _MSDataList[reportingFrame.MS_ID] = new MSData(reportingFrame.DetectedPower, Time.Instance.Now, reportingFrame.Location);
                
                return;
            }
            _MSDataList.Add(reportingFrame.MS_ID, new MSData(reportingFrame.DetectedPower, Time.Instance.Now, reportingFrame.Location));
        }

        private void CalculatePUposition()
        {
            if (_MSDataList.Count != 0 && SimParameters.OperationMode == true)
            {
                if (_MSDataList.Count < 5)
                {
                    var allMS = Simulation.GetMobileStations();
                    foreach (var ms in allMS)
                    {
                        Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(10), 
                            () => ms.UpdateWhisperRadius(false, SimParameters.WhishperRadiusDropRate)));
                    }
                }

                if (_MSDataList.Count > 6)
                {
                    var allMS = Simulation.GetMobileStations();
                    foreach (var ms in allMS)
                    {
                        if (ms._whisperRadius < SimParameters.WhishperRadiusLowerThreshold)
                        {
                            //if whisperradius is too small increase it rapidly
                            Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(10),
                                () => ms.UpdateWhisperRadius(SimParameters.WhishperRadiusLowerThreshold)));
                        }
                        else
                        {
                            Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(10),
                            () => ms.UpdateWhisperRadius(true, SimParameters.WhishperRadiusRaiseRate)));

                        }    
                    }
                }
                else
                {
                    if (_MSDataList.Count > 10)
                    {
                        var allMS = Simulation.GetMobileStations();
                        foreach (var ms in allMS)
                        {
                            if (ms._whisperRadius < SimParameters.WhishperRadiusUpperThreshold)
                            {
                                //if whisperradius is too small increase it rapidly
                                Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(10),
                                    () => ms.UpdateWhisperRadius(SimParameters.WhishperRadiusUpperThreshold)));
                            }
                            else
                            {
                                Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(10),
                                () => ms.UpdateWhisperRadius(true, SimParameters.WhishperRadiusRaiseRate)));
                        
                            }
                        }
                    }
                }
            }

            if (_MSDataList.Count < 4)
            {
                _lastEstimatedLocationOfPU = null;
            }
            else
            {
                _lastEstimatedLocationOfPU = EstimatelocationByMatrixManipulation();
            }

            _estimatedviamean = EstimateLocationByTakingMean();

            if (double.IsNaN(_estimatedviamean.x) || double.IsNaN(_estimatedviamean.y))
            {
                _estimatedviamean = null;
            }

            Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(_calculatePUTimeInterval), CalculatePUposition));                
        }

        private void DiscardPreviousReportingData()
        {
            var tempList = new Dictionary<int, MSData>();

            for (int i = 0; i < _MSDataList.Count; i++)
            {
                var msData = _MSDataList.ElementAt(i);
                if (Time.Instance.Now - msData.Value.LastDetectionTime <= _discardPreviousReportingDataInterval)
                {
                    tempList.Add(msData.Key, msData.Value);
                }
            }

            _MSDataList = tempList;

            Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(_discardPreviousReportingDataInterval), DiscardPreviousReportingData));                
        }

        /// <summary>
        /// Based on The Thesis of Birkan Yılmaz, works only when there is more than 4 detectors
        /// </summary>
        /// <returns></returns>
        private TerrainPoint EstimatelocationByMatrixManipulation()
        {
            var numberOfRows = _MSDataList.Count;

            if (numberOfRows == 0)
            {
                return default(TerrainPoint);
            }

            var A = new DenseMatrix(numberOfRows, 4);

            var B = new DenseMatrix(numberOfRows, 1);

            for (int i = 0; i < numberOfRows; i++)  //construct the matrix
            {
                var elementi = _MSDataList.ElementAt(i).Value;
                
                A.At(i, 0, 2 * elementi.Location.x);
                A.At(i, 1, 2 * elementi.Location.y);
                A.At(i, 2, 10 * ((-1* elementi.LastDetectedPower) * (-1 * 38.4) / 5 * 3.5));
                A.At(i, 3, -1);

                B.At(i, 0, elementi.Location.x * elementi.Location.x + elementi.Location.y * elementi.Location.y);     
            }

            var A_T = A.Transpose();

            var theta = A_T.Multiply(A).Inverse().Multiply(A_T).Multiply(B);

            var x = theta.At(0, 0);
            var y = theta.At(1, 0);

            return new TerrainPoint(x, y);
        }

        private TerrainPoint EstimateLocationByTakingMean()
        {
            var dataLocationList = new List<TerrainPoint>();

            foreach (var MSData in _MSDataList.Values)
            {
                dataLocationList.Add(MSData.Location);
            }

            var dataCount = dataLocationList.Count;

            var meanX = dataLocationList.Sum(d => d.x) / dataCount;
            var meanY = dataLocationList.Sum(d => d.y) / dataCount;

            return new TerrainPoint(meanX, meanY);
        }

        public TerrainPoint GetLastEstimatedLocationOfPU()
        {
            return _lastEstimatedLocationOfPU;
        }

        public TerrainPoint GetLastEstimatedLocationOfPUMean()
        {
            return _estimatedviamean;
        }

        public List<MSData> GetLastReportedData()
        {
            return _MSDataList.Values.ToList();
        }

        public int GetLastReportedDataCount()
        {
            return _MSDataList.Count;
        }
    }
}
