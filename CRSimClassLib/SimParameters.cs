using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib
{
    public static class SimParameters
    {
        public const double PUTransmissionPower =  70;

        public static double MSDetectionThreshold = 15;
        
        public const double NumberOfMeasurementsInMS = 50;
        
        public const int BSCalculateTimeInterval = 1000; //milisec

        public const int BSDiscardPreviousReportingDataInterval = 1000; //milisec

        public const int NumberOfWhisperBits = 16; //16 bit value is enough

        public const int NumberOfReportingBits = 128;

        public const int SensingPeriod = 10;    //milisec

        public const int WhisperPeriod = 20;

        public const int DecisionPeriod = 20;

        public const int ReportingPeriod = 10;    //milisec

        public const int TransmissionPeriod = 980;    //milisec

        public const int TransmissionPeriodWithDecision = TransmissionPeriod - DecisionPeriod - WhisperPeriod; //milisec

        public const int PUTalkDuration = 40000; //milisec 40 sec

        public const int PUInterArrival = 10000;    //milisec        

        public const double WhishperRadiusRaiseRate = 1.1;

        public const double MinPossibleWhisperRadius = 1;

        public static double MaxPossibleWhisperRadius = 25;
        
        public const double WhishperRadiusDropRate = 0.5;

        public const double WhishperRadiusFastRaiseRate = 1.5;

        public const double WhishperRadiusUpperThreshold = 25;

        public const double WhishperRadiusLowerThreshold = 5;

        public const int NumberOfSlotsInWhispering = 6400;

        public static bool OperationMode = false;

        public static int NumberOfWayPoints = 10;

        public const double MaxSpeed = 0.01;  // meter/milisecond = 10 m/s (Husein Bolt)

        public const double MinSpeed = 0.001; // meter/milisecond = 1 m/s

        public const int MaxStationaryTime = 10000; //10 secs

        public const int MinStationaryTime = 1000; //1 secs                               
        
        public static bool ForceMinimumEnergExpenditure = false;

    }
}
