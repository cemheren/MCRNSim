using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRSimClassLib.TerrainModal;
using CRSimClassLib.RandomWaypointMobilityModel;
using CRSimClassLib.Helpers;

namespace CRSimClassLib.Repositories
{
    public class MobileStationRepository
    {
        public MobileStation CreateMobileStation(double x, double y, double whisperRadius)
        {
            var ms = new MobileStation(x, y, whisperRadius);
            
            Simulation.Instance.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(0), ms.StartSensingPeriod));

            return ms;
        }

        public bool SenseForPUPresence(MobileStation station, double threshold, out double lastDetectedPower)
        {
            var pus = Simulation.Instance.GetPrimaryUsers();
            if (pus.Count == 0)
            {
                double totaldBPower = 0;
                for (int i = 0; i < SimParameters.NumberOfMeasurementsInMS; i++)
                {
                    totaldBPower += ChannelModels.NoiseFloor();
                }

                lastDetectedPower = totaldBPower / SimParameters.NumberOfMeasurementsInMS;

                return lastDetectedPower > threshold;
            }

            //for now simple assumption of one pu

            var pu = pus.First();

            var distance = station.GetLocation().DistanceTo(pu.GetLocation());

            double dBPower = 0;
            for (int i = 0; i < SimParameters.NumberOfMeasurementsInMS; i++)
            {
                var linearPower = ChannelModels.LogNormalChannelFading(pu.GetTransmitingPower(), distance).ToLinearScale()
                + ChannelModels.NoiseFloor().ToLinearScale();

                dBPower += linearPower.ToDecibels();
            }

            lastDetectedPower = dBPower / SimParameters.NumberOfMeasurementsInMS;

            return lastDetectedPower > threshold;
        }

        public bool MakeReportingDecision(MobileStation station, out bool? whisperingFailed)
        {
            //var myDecisionToReport = true;
            var mobileStations = Simulation.Instance.GetMobileStations();
            
            //this operation is costly
            var mobileStationsIHear = mobileStations.Where(ms => ms._whisperRadius >= ms.DistanceTo(station) && ms.MS_ID != station.MS_ID);
            
            if (mobileStationsIHear.Any(ms => ms._whisperSlotNumber == station._whisperSlotNumber)) // collision in this case
            {
                whisperingFailed = true;
                return true;
            }

            var thereExistsAmobileStationThatIHeardHigherThanMe = mobileStationsIHear.Any(ms => ms._lastDetectedPower > station._lastDetectedPower);

            whisperingFailed = false;

            return !thereExistsAmobileStationThatIHeardHigherThanMe;


            // if I got everything right this is the fastest version (I didn't)
            //var numberOfMobileStations = mobileStations.Count;
            //for (int i = 0; i < numberOfMobileStations; i++)
            //{
            //    var ms = mobileStations[i];
            //    if (ms._previousDetectionDecision == false)
            //        continue;

            //    if (ms._lastDetectedPower <= this._lastDetectedPower)
            //        continue;

            //    if (ms.DistanceTo(this) > ms._whisperRadius)
            //        continue;

            //    myDecisionToReport = false;
            //    break;
            //}

            //return myDecisionToReport;
        }

        // up to five closest stations report since they will spend the minimal energy for reporting
        public bool MakeReportingDecisionForMinimumEnergy(MobileStation station) 
        {
            var mobileStations = Simulation.Instance.GetMobileStations();

            //this operation is costly
            var closestMobileStationsThatHeardPUPresence = mobileStations.Where(ms => ms.GetPreviousDetectionDecision() == true).OrderBy(ms => ms.DistanceTo(Simulation.Instance.GetBaseStation())).Take(5).ToList();

            var IAmAMobileStationThatIsOneOfTheClosestToTheBaseStation = closestMobileStationsThatHeardPUPresence.Any(ms => ms.MS_ID == station.MS_ID);
                        
            return IAmAMobileStationThatIsOneOfTheClosestToTheBaseStation;
        }

        public void ScheduleNextEvent(MobileStation station, int timeInMiliseconds, Action EventMethod)
        {
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(timeInMiliseconds), EventMethod);
            Simulation.Instance.EnqueueEvent(e);
        }

        public void CreateStartReportingPeriodEvent(MobileStation station)
        {
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(0), station.StartReportingPeriod);
            Simulation.Instance.EnqueueEvent(e);
        }

        public void CreateStartWhisperPeriodEvent(MobileStation station)
        {
            Simulation.Instance.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(0), station.StartWhisperPeriod));
        }

        public void CreateStartSensingPeriodEvent(MobileStation station)
        {
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(SimParameters.TransmissionPeriod), station.StartSensingPeriod);
            Simulation.Instance.EnqueueEvent(e);                                       
        }

        public void CreateStartSensingPeriodEventWithFalseDetection(MobileStation station)
        {
            var timeShouldPass = SimParameters.TransmissionPeriod + SimParameters.ReportingPeriod;
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(timeShouldPass), station.StartSensingPeriod);
            Simulation.Instance.EnqueueEvent(e);
        }

        public void CreateEndReportingPeriodEvent(MobileStation station)
        {
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(SimParameters.ReportingPeriod), station.EndReportingPeriod);
            Simulation.Instance.EnqueueEvent(e);
        }

        public void CreateEndSensingPeriodEvent(MobileStation station)
        {
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(SimParameters.SensingPeriod), station.EndSensingPeriod);
            Simulation.Instance.EnqueueEvent(e);
        }

        public void CreateStartSensingPeriodWhenDecidedNotToReport(MobileStation mS)
        {
            // wait for reporting time + transmission time with decision
            var timeToWait = SimParameters.ReportingPeriod + SimParameters.TransmissionPeriodWithDecision;
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(timeToWait), mS.StartSensingPeriod);
            Simulation.Instance.EnqueueEvent(e);
        }

        public void CreateStartSensingPeriodEventWithDecision(MobileStation station)
        {
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(SimParameters.TransmissionPeriodWithDecision), station.StartSensingPeriod);
            Simulation.Instance.EnqueueEvent(e);
        }
    }
}
