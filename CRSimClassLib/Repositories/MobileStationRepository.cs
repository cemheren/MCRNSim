using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRSimClassLib.TerrainModal;
using CRSimClassLib.RandomWaypointMobilityModel;

namespace CRSimClassLib.Repositories
{
    public class MobileStationRepository
    {
        public MobileStation CreateMobileStation(double x, double y, double whisperRadius)
        {
            var ms = new MobileStation(x, y, whisperRadius);
            
            Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(0), ms.StartSensingPeriod));

            return ms;
        }

        public bool SenseForPUPresence(MobileStation station, double threshold, out double lastDetectedPower)
        {
            double totalReceivedPowerInLinearScale = 0;

            var pus = Simulation.GetPrimaryUsers();
            if (pus.Count == 0)
            {
                lastDetectedPower = ChannelModels.LogNormalChannelFading(-1000, 500); ;
                return false;
            }

            foreach (var pu in pus)
            {
                var distance = station.GetLocation().DistanceTo(pu.GetLocation());
                var receivedPower = ChannelModels.LogNormalChannelFading(pu.GetTransmitingPower(), distance);

                totalReceivedPowerInLinearScale += receivedPower.ToLinearScale();
            }

            lastDetectedPower = totalReceivedPowerInLinearScale.ToDecibels();

            return lastDetectedPower > threshold;
        }

        public bool MakeReportingDecision(MobileStation station)
        {
            //var myDecisionToReport = true;
            var mobileStations = Simulation.GetMobileStations();

            //this operation is costly
            var mobileStationsIHear = mobileStations.Where(ms => ms._whisperRadius >= ms.DistanceTo(station));

            var thereExistsAmobileStationThatIHeardHigherThanMe = mobileStationsIHear.Any(ms => ms._lastDetectedPower > station._lastDetectedPower);

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

        public void ScheduleNextEvent(MobileStation station, int timeInMiliseconds, Action EventMethod)
        {
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(timeInMiliseconds), EventMethod);
            Simulation.EnqueueEvent(e);
        }

        public void CreateStartReportingPeriodEvent(MobileStation station)
        {
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(0), station.StartReportingPeriod);
            Simulation.EnqueueEvent(e);
        }

        public void CreateStartWhisperPeriodEvent(MobileStation station)
        {
            Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(0), station.StartWhisperPeriod));
        }

        public void CreateStartSensingPeriodEvent(MobileStation station)
        {
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(SimParameters.TransmissionPeriod), station.StartSensingPeriod);
            Simulation.EnqueueEvent(e);                                       
        }

        public void CreateStartSensingPeriodEventWithFalseDetection(MobileStation station)
        {
            var timeShouldPass = SimParameters.TransmissionPeriod + SimParameters.ReportingPeriod;
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(timeShouldPass), station.StartSensingPeriod);
            Simulation.EnqueueEvent(e);
        }

        public void CreateEndReportingPeriodEvent(MobileStation station)
        {
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(SimParameters.ReportingPeriod), station.EndReportingPeriod);
            Simulation.EnqueueEvent(e);
        }

        public void CreateEndSensingPeriodEvent(MobileStation station)
        {
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(SimParameters.SensingPeriod), station.EndSensingPeriod);
            Simulation.EnqueueEvent(e);
        }

        public void CreateStartSensingPeriodWhenDecidedNotToReport(MobileStation mS)
        {
            // wait for reporting time + transmission time with decision
            var timeToWait = SimParameters.ReportingPeriod + SimParameters.TransmissionPeriodWithDecision;
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(timeToWait), mS.StartSensingPeriod);
            Simulation.EnqueueEvent(e);
        }

        public void CreateStartSensingPeriodEventWithDecision(MobileStation station)
        {
            var e = new Event(Time.Instance.GetTimeAfterMiliSeconds(SimParameters.TransmissionPeriodWithDecision), station.StartSensingPeriod);
            Simulation.EnqueueEvent(e);
        }
    }
}
