using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Data.Model
{
    public class CRStat
    {
        public virtual Guid Id { get; set; }

        public virtual int NumberOfStations { get; set; }

        public virtual int TruePositiveDetectionCount { get; set; }

        public virtual int FalsePositiveDetectionCount { get; set; }

        public virtual int TrueNegativeDetectionCount { get; set; }

        public virtual int FalseNegativeDetectionCount { get; set; }

        public virtual int TotalDetectionCount { get; set; }

        public virtual double HardPd { get; set; }

        public virtual double HardPm { get; set; }

        public virtual double HardPf { get; set; }

        public virtual double TimePUCorrectlyDetected { get; set; }

        public virtual double TimePU_not_existandCorrectlyDetected { get; set; }

        public virtual double TimePUPresentButNotDetected { get; set; }

        public virtual double TimePU_notExistButFalselyDetectedPresent { get; set; }

        public virtual double TotalPowerSpentForWhispering { get; set; }

        public virtual double TotalPowerSpentForReporting { get; set; }

        public virtual double TotalPowerSpent { get; set; }

        public virtual double AverageDistanceDifference { get; set; }

        public virtual double CurrentTime { get; set; }

        public virtual double AverageNumberOfDetectedMobileStations { get; set; }

        public virtual double AverageNumberOfReportedMobileStations { get; set; }

        public virtual double TotalTimePrimaryUserExisted { get; set; }

        public virtual double TotalTimePrimaryUserDetected { get; set; }

        public virtual double AverageWhisperRadius { get; set; }

        public virtual double AverageDistanceToBS { get; set; }

        public virtual bool MinimumEnergyMode { get; set; }

        public virtual bool WhisperingEnabled { get; set; }

        public virtual double AverageFailedWhisperingCount { get; set; }
    }
}
