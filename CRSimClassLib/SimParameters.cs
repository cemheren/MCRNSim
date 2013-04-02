using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib
{
    public static class SimParameters
    {
        public const double PUTransmissionPower = -20;

        public const double MSDetectionThreshold = -120;
        
        public const double NumberOfMeasurementsInMS = 50;
        
        public const int BSCalculateTimeInterval = 1000; //milisec

        public const int BSDiscardPreviousReportingDataInterval = 1000; //milisec

        public const int SensingPeriod = 10;    //milisec

        public const int ReportingPeriod = 10;    //milisec

        public const int TransmissionPeriod = 980;    //milisec

        public const int TransmissionPeriodWithDecision = TransmissionPeriod - DecisionPeriod; //milisec

        public const int DecisionPeriod = 20;

        public const int BSDiscardPreviousReportingDataPeriodLength = 980;

        public const int MaximalRemovePUAfter = 20000; //milisec

        public const int MaximalCreatePUAfter = 5000;    //milisec        

        public const double WhishperRadiusRaiseRate = 1.1;

        public const double MinPossibleWhisperRadius = 1;
        
        public const double WhishperRadiusDropRate = 0.5;

        public const double WhishperRadiusFastRaiseRate = 1.5;

        public const double WhishperRadiusUpperThreshold = 25;

        public const double WhishperRadiusLowerThreshold = 5;

        public const bool OperationMode = true;

        public const int NumberOfWayPoints = 40;

        public const double MaxSpeed = 0.01;  // meter/milisecond = 10 m/s (Husein Bolt)

        public const double MinSpeed = 0.001; // meter/milisecond = 1 m/s

        public const int MaxStationaryTime = 10000; //10 secs

        public const int MinStationaryTime = 1000; //1 secs
                       
    }
}
