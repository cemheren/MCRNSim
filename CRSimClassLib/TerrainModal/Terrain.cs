using CRSimClassLib.RandomWaypointMobilityModel;
using CRSimClassLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib.TerrainModal
{
    public class Terrain
    {
        private List<MobileStation> _mobileStations;
        private List<PrimaryUser> _primaryUsers;
        private BaseStation _baseStation;
        private List<WayPoint> _wayPoints;

        internal TerrainPoint _leftUpCorner { get; private set; }
        internal TerrainPoint _leftDownCorner { get; private set; }
        internal TerrainPoint _rightUpCorner { get; private set; }
        internal TerrainPoint _rightDownCorner { get; private set; }

        private RandomNumberRepository _rand = RandomNumberRepository.Instance;
        private RandomWaypointRepository _randomWaypointRepo = Singleton<RandomWaypointRepository>.Instance;
        private MobileStationRepository _mobileStationsRepository = Singleton<MobileStationRepository>.Instance;

        public Terrain(double height, double width)
        {
            _leftUpCorner = new TerrainPoint(0, 0);
            _leftDownCorner = new TerrainPoint(0, height);
            _rightUpCorner = new TerrainPoint(width, 0);
            _rightDownCorner = new TerrainPoint(width, height);

            _mobileStations = new List<MobileStation>();
            _primaryUsers = new List<PrimaryUser>();

            _wayPoints = _randomWaypointRepo.CreateRandomWaypoints(this, SimParameters.NumberOfWayPoints);
        }

        public List<MobileStation> GetMobileStations()
        {
            return _mobileStations.ToList();
        }

        public void CreateMobileStations(int numberOfStations, double whisperRadius)
        {
            for (int i = 0; i < numberOfStations; i++)
            {
                var ms = _mobileStationsRepository.CreateMobileStation(
                    _rand.GetNextDouble(_leftUpCorner.x, _rightUpCorner.x), 
                    _rand.GetNextDouble(_leftUpCorner.y, _leftDownCorner.y),
                    whisperRadius);

                _mobileStations.Add(ms);
            }
        }

        public void CreatePrimaryUser(double transmittingPower) 
        {
            var pu = new PrimaryUser(_rand.GetNextDouble(_leftUpCorner.x + 10, _rightUpCorner.x - 10),
                    _rand.GetNextDouble(_leftUpCorner.y + 10, _leftDownCorner.y - 10), transmittingPower);
            _primaryUsers.Add(pu);

            var nextEventTime = RandomNumberRepository.Instance.NextInt(0, SimParameters.MaximalRemovePUAfter);

            Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(nextEventTime), () => RemovePrimaryUser(pu)));
        }

        public void RemovePrimaryUser(PrimaryUser primaryUser)
        {
            _primaryUsers.Remove(primaryUser);

            var nextEventTime = RandomNumberRepository.Instance.NextInt(0, SimParameters.MaximalCreatePUAfter);

            Simulation.EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(nextEventTime), () => CreatePrimaryUser(primaryUser.GetTransmitingPower())));
        }

        public List<PrimaryUser> GetAllPrimaryUsers()
        {
            return _primaryUsers;
        }

        public void CreateBaseStation()
        {
            _baseStation = new BaseStation((_leftUpCorner.x + _rightUpCorner.x) / 2, (_leftUpCorner.y + _leftDownCorner.y) / 2);
        }

        public BaseStation GetBaseStation()
        {
            return _baseStation;
        }

        public List<WayPoint> GetWayPoints()
        {
            return _wayPoints;
        }
    }
}
