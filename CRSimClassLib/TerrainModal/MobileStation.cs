using CRSimClassLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CRSimClassLib.TerrainModal;
using CRSimClassLib.RandomWaypointMobilityModel;

namespace CRSimClassLib.TerrainModal
{
    public class MobileStation : Station
    {
        private static int numberOfMS = 0;

        public double _whisperRadius { get; private set; }
        public double _totalPowerWhispering { get; private set; }
        public double _totalPowerReporting { get; private set; }
        public int MS_ID {get; private set; }
        public int _whisperSlotNumber { get; private set; }
        public bool? _whisperingFailed { get; private set; }

        private double _detectionThreshold = SimParameters.MSDetectionThreshold;
        private bool _previousDetectionDecision;
        public double _lastDetectedPower { get; private set; }
        private bool _operationMode; //true => new algorithm, false => traditional algorithm
        private MobilityStateModal _mobilityStateModal;

        private MobileStationRepository _mobileStationRepository =  Singleton<MobileStationRepository>.Instance;
        private RandomWaypointRepository _randomWaypointRepository = Singleton<RandomWaypointRepository>.Instance;
        private PowerCalculationRepository _powerConsumptionRepository = Singleton<PowerCalculationRepository>.Instance;

        public MobileStation(double x, double y, double whisperRadius) : base(x, y)
        {
            _whisperRadius = whisperRadius;
            _operationMode = SimParameters.OperationMode;
            _lastDetectedPower = -100000;
            MS_ID = numberOfMS; //give everyone an Id
            numberOfMS++;
            _whisperingFailed = null;

            _mobilityStateModal = _randomWaypointRepository.GetInitialStationaryState(this);
            Simulation.Instance.EnqueueEvent(new Event(_mobilityStateModal.TimeEnded,
                HandleMovement));
        }

        public MobilityStateModal GetMobilityStateModal()
        {
            return _mobilityStateModal;
        }

        private void HandleMovement()
        {
            _location = _mobilityStateModal.EndingPoint;
            if (_mobilityStateModal.IsMoving == false)
            {
                var nextWaypoint = _randomWaypointRepository.SelectRandomWayPoint();
                _mobilityStateModal = _randomWaypointRepository.GetRandomMobileState(this, nextWaypoint);
            }
            else
            {
                _mobilityStateModal = _randomWaypointRepository.GetRandomStationaryState(this);
            }

            Simulation.Instance.EnqueueEvent(new Event(_mobilityStateModal.TimeEnded, HandleMovement));            
        }

        public override TerrainPoint GetLocation()
        {
            if (_mobilityStateModal == null)
            {
                return base.GetLocation();
            }
            if (_mobilityStateModal.IsMoving == false)
            {
                return _location;
            }

            _location = _randomWaypointRepository.CalculateMobileCoordinates(_mobilityStateModal);

            return _location;
        }

        public bool GetPreviousDetectionDecision()
        {
            return _previousDetectionDecision;
        }

        /*
         * Non-cluster mode
         * Sensing period + Reporting period + Transmission period
         *      10                  10                  980
         * 
         * Cluster mode
         * Sensing period + Whisper period + Decision Period + Reporting period + Transmission period
         *      10                  20              20                 10                 940
         * 
         */

        public void StartSensingPeriod()
        {
            double lastDetectedPower;
            _previousDetectionDecision = _mobileStationRepository.SenseForPUPresence(this, _detectionThreshold, out lastDetectedPower);
            _lastDetectedPower = lastDetectedPower;
            _mobileStationRepository.CreateEndSensingPeriodEvent(this);
        }

        public void EndSensingPeriod()
        {
            double lastDetectedPower;
            _previousDetectionDecision = _mobileStationRepository.SenseForPUPresence(this, _detectionThreshold, out lastDetectedPower);
            _lastDetectedPower = lastDetectedPower;
                                    
            if (_previousDetectionDecision)
            {
                if (this._operationMode == false)
                {
                    _mobileStationRepository.CreateStartReportingPeriodEvent(this);
                }
                else
                {
                    _mobileStationRepository.CreateStartWhisperPeriodEvent(this);
                }
            }
            else
            {
                _mobileStationRepository.CreateStartSensingPeriodEventWithFalseDetection(this);
            }
        }

        public void StartWhisperPeriod()
        {
            _whisperSlotNumber = RandomNumberRepository.Instance.NextInt(0, SimParameters.NumberOfSlotsInWhispering);
            _whisperingFailed = false;
            Simulation.Instance.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(SimParameters.WhisperPeriod), () => EndWhisperPeriod()));            
        }

        public void EndWhisperPeriod()
        {
            if (SimParameters.ForceMinimumEnergExpenditure == false)
            {
                _totalPowerWhispering += _powerConsumptionRepository.TransmissionPower(_whisperRadius, SimParameters.NumberOfWhisperBits);                
            }
            Simulation.Instance.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(0), () => StartDecisionPeriod()));                        
        }

        public void StartDecisionPeriod()
        {
            // do this in order to wait for every node to finish their sensing first.
            Simulation.Instance.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(SimParameters.DecisionPeriod), () => EndDecisionPeriod()));
        }

        public void EndDecisionPeriod()
        {
            if (SimParameters.ForceMinimumEnergExpenditure) 
            {
                var myDecisionForMinimumEnergy = _mobileStationRepository.MakeReportingDecisionForMinimumEnergy(this);

                if (myDecisionForMinimumEnergy)
                {
                    _mobileStationRepository.CreateStartReportingPeriodEvent(this);
                }
                else
                {
                    _mobileStationRepository.CreateStartSensingPeriodWhenDecidedNotToReport(this);
                }
            }
            else
            {
                bool? whisperingFailed;

                var myDecisionToReport = _mobileStationRepository.MakeReportingDecision(this, out whisperingFailed);

                _whisperingFailed = whisperingFailed;

                if (_whisperingFailed.HasValue && _whisperingFailed.Value == true) // collision in this case
                {
                    _mobileStationRepository.CreateStartReportingPeriodEvent(this);
                    return;
                }

                if (myDecisionToReport)
                {
                    _mobileStationRepository.CreateStartReportingPeriodEvent(this);
                }
                else
                {
                    _mobileStationRepository.CreateStartSensingPeriodWhenDecidedNotToReport(this);
                }
            }
        }

        public void StartReportingPeriod()
        {
            var bs = Simulation.Instance.GetBaseStation();
            var reportingFrame = new ReportingFrameModal { MS_ID = this.MS_ID, Location = GetLocation(), DetectedPower = _lastDetectedPower };
            Simulation.Instance.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(SimParameters.ReportingPeriod), () => bs.ReceiveReporting(reportingFrame)));

            _mobileStationRepository.CreateEndReportingPeriodEvent(this);
        }

        public void EndReportingPeriod()
        {
            var distanceToBaseStation = Simulation.Instance.GetTerrain().GetBaseStation().DistanceTo(this);
            
            _totalPowerReporting += _powerConsumptionRepository.TransmissionPower(distanceToBaseStation, SimParameters.NumberOfReportingBits);

            if (this._operationMode == false)
            {
                _mobileStationRepository.CreateStartSensingPeriodEvent(this);
            }
            else
            {
                _mobileStationRepository.CreateStartSensingPeriodEventWithDecision(this);
            }
        }

        internal void UpdateWhisperRadius(bool up, double factor)
        {
            if (up)
	        {
                _whisperRadius = _whisperRadius * factor;
                if (_whisperRadius > SimParameters.MaxPossibleWhisperRadius)
                {
                    _whisperRadius = SimParameters.MaxPossibleWhisperRadius;
                }
            }
            else
            {
                _whisperRadius = _whisperRadius * factor;
                if (_whisperRadius < SimParameters.MinPossibleWhisperRadius)
                {
                    _whisperRadius = SimParameters.MinPossibleWhisperRadius;
                }
            }
        }

        internal void UpdateWhisperRadius(double newValue)
        {
            _whisperRadius = newValue;
        }
        
    }
}
