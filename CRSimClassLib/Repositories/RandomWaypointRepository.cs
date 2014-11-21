using CRSimClassLib.RandomWaypointMobilityModel;
using CRSimClassLib.TerrainModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib.Repositories
{
    public class RandomWaypointRepository
    {
        public List<WayPoint> CreateRandomWaypoints(Terrain terrain, int numberOfWaypoints)
        {
            return WayPoint.NewWayPointList(terrain, numberOfWaypoints);
        }

        public WayPoint SelectRandomWayPoint()
        {
            var wps = Simulation.Instance.GetWayPoints();

            return wps[RandomNumberRepository.Instance.NextInt(0,wps.Count)];
        }

        public MobilityStateModal GetRandomMobileState(Station station, WayPoint nextWaypoint)
        {
            var speed = RandomNumberRepository.Instance.GetNextDouble(SimParameters.MinSpeed, SimParameters.MaxSpeed);

            var mobilityState = new MobilityStateModal(station.GetLocation(), nextWaypoint.GetLocation(), speed);

            return mobilityState;
        }

        public MobilityStateModal GetInitialStationaryState(Station station)
        {
            var mobilityState = new MobilityStateModal(Time.Instance.Now + 1000,
                station.GetLocation());

            return mobilityState;
        }

        public MobilityStateModal GetRandomStationaryState(Station station)
        {
            var mobilityState = new MobilityStateModal(Time.Instance.Now + 
                RandomNumberRepository.Instance.NextInt(SimParameters.MinStationaryTime, SimParameters.MaxStationaryTime),
                station.GetLocation());

            return mobilityState;
        }

        public TerrainPoint CalculateMobileCoordinates(MobilityStateModal stateModal)
        {
            var now = Time.Instance.Now;
            var totalTime = stateModal.TimeEnded - stateModal.TimeCreated;
            var passedTime = now - stateModal.TimeCreated;

            var distanceX = stateModal.EndingPoint.x - stateModal.StartingPoint.x; 
            var distanceY = stateModal.EndingPoint.y - stateModal.StartingPoint.y;

            var newX = stateModal.StartingPoint.x + distanceX * ((double)passedTime / totalTime);
            var newY = stateModal.StartingPoint.y + distanceY * ((double)passedTime / totalTime);

            return new TerrainPoint(newX, newY);
        }
    }
}