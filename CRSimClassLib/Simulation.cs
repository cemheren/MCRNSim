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
    public static class Simulation
    {
        public static bool EndCondition = false;    //check this condition in the main loop
        private static OrderedSet<Event> EventQueue;
        private static int _simulationStopTime;
        private static double _mobileStationWhisperRadius;
        private static Terrain _terrain;

        public static Action DoOnTimeTick = null;

        private static StatisticsRepository _statisticsRepository = Singleton<StatisticsRepository>.Instance;
        private static PrimaryUserRepository _primaryUserRepository = Singleton<PrimaryUserRepository>.Instance;

        public static void InitializeSimulation(int SimulationStopTime, int numberOfMobileStations, int? numberOfWayPoints, double terrainWidth, double terrainHeigth)
        {
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
            
            Time.Instance.SetTimeToZero();
            EndCondition = false;

            EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(501), TimeTick));

            var simEndEvent = new Event(_simulationStopTime, () => { Console.WriteLine("Simulation over. Thank you."); EndCondition = true; });
            EnqueueEvent(simEndEvent);
        }

        private static void TimeTick()
        {
            if (DoOnTimeTick != null)
            {
                DoOnTimeTick();                
            }
            EnqueueEvent(new Event(Time.Instance.GetTimeAfterMiliSeconds(500), TimeTick));
        }

        public static void EnqueueEvent(Event ev)
        {
            EventQueue.Add(ev);
        }

        public static void DequeueEvent()
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

        public static Terrain GetTerrain()
        {
            return _terrain;
        }
        
        public static List<MobileStation> GetMobileStations()
        {
            return _terrain.GetMobileStations();
        }

        public static List<PrimaryUser> GetPrimaryUsers()
        {
            return _terrain.GetAllPrimaryUsers();
        }

        public static BaseStation GetBaseStation()
        {
            return _terrain.GetBaseStation();
        }

        public static List<WayPoint> GetWayPoints()
        {
            return _terrain.GetWayPoints();
        }

        public static void UpdateStatistics(int timeBefore, int timeAfter)
        {
            _statisticsRepository.UpdateAverageDistance(_terrain, timeBefore, timeAfter);

            _statisticsRepository.UpdateMobileStationDetectionStats(_terrain, timeBefore, timeAfter);

            _statisticsRepository.UpdateAverageWhisperRadius(_terrain, timeBefore, timeAfter);
        }
    }
}
