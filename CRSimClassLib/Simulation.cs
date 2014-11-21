using CRSimClassLib.RandomWaypointMobilityModel;
using CRSimClassLib.Repositories;
using CRSimClassLib.TerrainModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace CRSimClassLib
{
    public class Simulation
    {
        private static Simulation _instance;
        public static Simulation Instance
        { 
            get 
            {
                if (_instance == null)
                {
                    _instance = new Simulation();
                }
                return _instance;
            } 
        }
        private Simulation() { }

        public void NewSimulationInstance()
        {
            _instance = new Simulation();
        }

        public bool EndCondition = false;    //check this condition in the main loop
        private OrderedSet<Event> EventQueue;
        private int _simulationStopTime;
        private double _mobileStationWhisperRadius = SimParameters.WhishperRadiusLowerThreshold;
        private Terrain _terrain;

        public Action DoOnTimeTick = null;

        private StatisticsRepository _statisticsRepository = Singleton<StatisticsRepository>.Instance;
        private PrimaryUserRepository _primaryUserRepository = Singleton<PrimaryUserRepository>.Instance;

        public void InitializeSimulation(int SimulationStopTime, int numberOfMobileStations, int? numberOfWayPoints, double terrainWidth, double terrainHeigth)
        {
            Time.Instance.SetTimeToZero();
            EndCondition = false;
                        
            EventQueue = new OrderedSet<Event>();
            _simulationStopTime = SimulationStopTime;
            if (numberOfWayPoints != null)
            {
                SimParameters.NumberOfWayPoints = numberOfWayPoints.Value;                
            }

            var simStartEvent = new Event(0);
            EnqueueEvent(simStartEvent);
            
            _terrain = new Terrain(terrainHeigth, terrainWidth);
            _terrain.CreateMobileStations(numberOfMobileStations, _mobileStationWhisperRadius);
            _terrain.CreateBaseStation();
            _primaryUserRepository.CreateAndAddPrimaryUser();
            

            EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(501), TimeTick));

            var simEndEvent = new Event(_simulationStopTime, () => { Console.WriteLine("Simulation over. Thank you."); EndCondition = true; });
            EnqueueEvent(simEndEvent);
        }

        private void TimeTick()
        {
            if (DoOnTimeTick != null)
            {
                DoOnTimeTick();                
            }
            EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(500), TimeTick));
        }

        public void EnqueueEvent(Event ev)
        {
            EventQueue.Add(ev);
        }

        public void DequeueEvent()
        {
            if (EventQueue.Count > 0)
	        {
                var timeBefore = Time.Instance.Now;
                var ev = EventQueue.GetFirst();
                ev.ExecuteEvent();
                EventQueue.RemoveFirst();

                var timeAfter = Time.Instance.Now;
                UpdateStatistics(timeBefore, timeAfter);
            }
        }

        public Terrain GetTerrain()
        {
            return _terrain;
        }
        
        public List<MobileStation> GetMobileStations()
        {
            return _terrain.GetMobileStations();
        }

        public List<PrimaryUser> GetPrimaryUsers()
        {
            return _terrain.GetAllPrimaryUsers();
        }

        public BaseStation GetBaseStation()
        {
            return _terrain.GetBaseStation();
        }

        public List<WayPoint> GetWayPoints()
        {
            return _terrain.GetWayPoints();
        }

        public void UpdateStatistics(int timeBefore, int timeAfter)
        {
            _statisticsRepository.UpdateAverageDistance(_terrain, timeBefore, timeAfter);

            _statisticsRepository.UpdateMobileStationDetectionStats(_terrain, timeBefore, timeAfter);

            _statisticsRepository.UpdateAverageWhisperRadius(_terrain, timeBefore, timeAfter);

            _statisticsRepository.UpdateAverageNumberOfWhisperingAttemptsFailed(_terrain, timeBefore, timeAfter);

            _statisticsRepository.UpdateTotalPowerForReporting(_terrain); //TODO: fix here, you know what is it - NO

            _statisticsRepository.UpdateTotalPowerForWhispering(_terrain);

            _statisticsRepository.UpdateProtocolDependentPdPmPf(_terrain, timeBefore, timeAfter);
        }
    }
}
