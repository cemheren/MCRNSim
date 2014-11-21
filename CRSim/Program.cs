using CRSimClassLib;
using CRSimClassLib.Repositories;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCPServer.Data;
using TCPServer.Data.Model;

namespace CRSim
{
    class Program
    {
        private static int _numberOfStations = 100;
        private static int _width = 500;
        private static int _height = 500;
        //private static int simulationTime = 10000 * 1000; //10 000 secs
        private static int simulationTime = 10000 * 1000; //100 000 secs

        static void Main(string[] args)
        {

            Console.WriteLine("Drop DB?");
            var line = Console.ReadLine();

            if (line == "yes")
            {
                SimParameters.OperationMode = true;
                Console.WriteLine("Dropping old DB");

                //ServerDB.Instance.DropCreate(csb => csb.FromConnectionStringWithKey("CRSim"));                            
            }

            ServerDB.Instance.Update(csb => csb.FromConnectionStringWithKey("CRSim"));
            
            Console.WriteLine("Simulation is started, hit enter to see current statistics");

            Thread t = new Thread(() => MainLoop());

            t.Start();

            while (Simulation.Instance.EndCondition == false)
            {
                var a = Console.ReadLine();
                PrintStatistics();
            }
        }

        public static void MainLoop()
        {          

            for (int i = 0; i < 100; i++)
            {
                SimParameters.NumberOfWayPoints = 20;
            
                RandomNumberRepository.Instance = null; // new random number
                
                _numberOfStations = 100;
                SimParameters.OperationMode = true;
                SimParameters.ForceMinimumEnergExpenditure = true;

                for (int k = 0; k < 26; k++)  // 100 - 600
                {
                    Simulation.Instance.InitializeSimulation(simulationTime, _numberOfStations, null, _width, _height);

                    while (Simulation.Instance.EndCondition == false)
                    {
                        Simulation.Instance.DequeueEvent();
                    }
                    PrintStatistics();
                    //DumpStatisticsToFile();
                    SaveStatisticsToDB();

                    _numberOfStations += 20;

                    Statistics.InitStatistics();
                    Simulation.Instance.NewSimulationInstance();
                }
                
                SimParameters.OperationMode = false;
                _numberOfStations = 100;

                for (int k = 0; k < 26; k++)  // 100 - 600
                {
                    Simulation.Instance.InitializeSimulation(simulationTime, _numberOfStations, null, _width, _height);

                    while (Simulation.Instance.EndCondition == false)
                    {
                        Simulation.Instance.DequeueEvent();
                    }
                    PrintStatistics();
                    SaveStatisticsToDB();
                    //DumpStatisticsToFile();

                    _numberOfStations += 20;

                    Statistics.InitStatistics();
                    Simulation.Instance.NewSimulationInstance();
                }

                SimParameters.OperationMode = true;
                SimParameters.ForceMinimumEnergExpenditure = false;
                _numberOfStations = 100;

                for (int k = 0; k < 26; k++)  // 100 - 600
                {
                    Simulation.Instance.InitializeSimulation(simulationTime, _numberOfStations, null, _width, _height);

                    while (Simulation.Instance.EndCondition == false)
                    {
                        Simulation.Instance.DequeueEvent();
                    }
                    PrintStatistics();
                    SaveStatisticsToDB();
                    //DumpStatisticsToFile();

                    _numberOfStations += 20;

                    Statistics.InitStatistics();
                    Simulation.Instance.NewSimulationInstance();
                }

            }
            

            //for (int i = 0; i < 5; i++) // number of waypoints 10 - 50
            //{
                

            //    SimParameters.NumberOfWayPoints += 10;
            //}

            //SimParameters.NumberOfWayPoints = 40;

            //_numberOfStations = 300;

            //SimParameters.MSDetectionThreshold = -80;

            //for (int i = 0; i <= 20; i++)
            //{
            //    Simulation.Instance.InitializeSimulation(simulationTime, _numberOfStations, null, _width, _height);

            //    while (Simulation.Instance.EndCondition == false)
            //    {
            //        Simulation.Instance.DequeueEvent();
            //    }
            //    PrintStatistics();
            //    DumpStatisticsToFile();

            //    SimParameters.MSDetectionThreshold--;

            //    Statistics.InitStatistics();
            //    Simulation.Instance.NewSimulationInstance();
            //}

        }

        public static void DoOnTimeTick()
        {
            if (Time.Instance.Now > 75000)
            {
                var a = Simulation.Instance.EndCondition;
            }
        }

        private static string GetFormattedStatistics()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Number of stations : " + _numberOfStations);
            
            stringBuilder.AppendLine("Area dimensions : " + _width + " x " + _height);
            stringBuilder.AppendLine("True positive detection count for pu presence = " + Statistics.TruePositiveDetectionCountForPUPresence);
            stringBuilder.AppendLine("False positive detection count for pu presence = " + Statistics.FalsePositiveDetectionCountForPUPresence);
            stringBuilder.AppendLine("True Negative detection count for pu presence = " + Statistics.TrueNegativeDetectionCountForPUPresence);
            stringBuilder.AppendLine("False Negative detection count for pu presence = " + Statistics.FalseNegativeDetectionCountForPUPresence);
            stringBuilder.AppendLine("Total detection count for pu presence = " + Statistics.TotalDetectionCountForPUPresence);
            stringBuilder.AppendLine("Hard Pd = " + Statistics.TruePositiveDetectionCountForPUPresence / (double)(Statistics.TruePositiveDetectionCountForPUPresence + Statistics.FalsePositiveDetectionCountForPUPresence));
            stringBuilder.AppendLine("Hard Pm = " + Statistics.FalsePositiveDetectionCountForPUPresence / (double)(Statistics.TruePositiveDetectionCountForPUPresence + Statistics.FalsePositiveDetectionCountForPUPresence));
            stringBuilder.AppendLine("Hard Pf = " + Statistics.FalseNegativeDetectionCountForPUPresence / (double)(Statistics.TrueNegativeDetectionCountForPUPresence + Statistics.FalseNegativeDetectionCountForPUPresence));

            stringBuilder.AppendLine("Total time spend with PU presence and correctly detected = " 
                + Statistics.TotalTimeSpentCorrectlyInProtocol_h0_h0 / 1000.0
                + " % " + Statistics.TotalTimeSpentCorrectlyInProtocol_h0_h0 * 100 / (double)Time.Instance.Now);
            stringBuilder.AppendLine("Total time spend with PU does not exist and correctly detected = " 
                + Statistics.TotalTimeSpentCorrectlyInProtocol_h1_h1 / 1000.0
                + " % " + Statistics.TotalTimeSpentCorrectlyInProtocol_h1_h1 * 100 / (double)Time.Instance.Now);
            stringBuilder.AppendLine("Total time spend with PU present but not detected = " 
                + Statistics.TotalTimeSpentMistakenlylyInProtocol_h0_h1 / 1000.0
                + " % " + Statistics.TotalTimeSpentMistakenlylyInProtocol_h0_h1 * 100 / (double)Time.Instance.Now);
            stringBuilder.AppendLine("Total time spend with PU dont exist but falsely detected as present = " 
                + Statistics.TotalTimeSpentMistakenlylyInProtocol_h1_h0 / 1000.0
                + " % " + Statistics.TotalTimeSpentMistakenlylyInProtocol_h1_h0 * 100 / (double)Time.Instance.Now);              

            stringBuilder.AppendLine("Total power spent for whispering = " + Statistics.TotalPowerWhispering * 0.00000001 + " joules ");
            stringBuilder.AppendLine("Total power spent for reporting = " + Statistics.TotalPowerReporting * 0.00000001 + " joules ");
            stringBuilder.AppendLine("Total power spent = " + ((Statistics.TotalPowerReporting + Statistics.TotalPowerWhispering) * 0.00000001).ToString() + " joules ");
            stringBuilder.AppendLine("Average distance diffence = " + Statistics.AverageDistanceOfThePredictedAndActualPrimaryUserLocation
                + " meters");
            stringBuilder.AppendLine("Current time = " + Time.Instance.GetTimeInSeconds().ToString("#,##0") + " seconds");
            stringBuilder.AppendLine("Average # of Detected Mobile Stations : " + Statistics.AverageNumberOfMSThatDetectedPrimaryUserPresence);
            stringBuilder.AppendLine("Average # of Reported Mobile Stations : " + Statistics.AverageNumberOfMSThatReportedPrimaryUserPresence);
            stringBuilder.AppendLine("Total time primary user existed: " + ((double)Statistics.TotalTimeAPrimaryUserHaveExisted / 1000).ToString());
            stringBuilder.AppendLine("Total time primary user detected: " + ((double)Statistics.TotalTimeAPrimaryUserHaveDetected / 1000).ToString());
            stringBuilder.AppendLine("Average whisper radius: " + Statistics.AverageWhisperRadius.ToString());
            stringBuilder.AppendLine("Average distance to BS: " + Statistics.AverageDistanceToBaseStation.ToString());
            stringBuilder.AppendLine("Average failed whispers: " + Statistics.AverageNumberOfWhisperingAttemptsFailed.ToString());

            stringBuilder.AppendLine();

            stringBuilder.AppendLine("Distance bucket in terms of time (seconds and percentage):");
            for (int i = 0; i < Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent.Length; i++)
            {
                stringBuilder.AppendLine(((i) * 10).ToString() + "-" + (Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent[i] / 1000).ToString()
                    + " ~ " + (Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent[i] * 100.0 / Statistics.TotalTimeAPrimaryUserHaveDetected 
                    + "%"));
            }

            stringBuilder.AppendLine("-----------------------------------------------------------------------");


            return stringBuilder.ToString();
        }

        private static void SaveStatisticsToDB() 
        {
            var isBreak = false;

            for (int i = 0; i < 15; i++)
            {
                try
                {
                    SaveStatisticsToDBInner();
                    isBreak = true;
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Trial: " + i);
                }
            }

            if (isBreak == false)
            {
                DumpStatisticsToFile();
            }
        }

        private static void SaveStatisticsToDBInner()
        {

            using (var session = ServerDB.Instance.SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var CRStat = new CRStat
                {
                    Id = Guid.NewGuid(),
                    NumberOfStations = _numberOfStations,
                    TruePositiveDetectionCount = Statistics.TruePositiveDetectionCountForPUPresence,
                    FalsePositiveDetectionCount = Statistics.FalsePositiveDetectionCountForPUPresence,
                    TrueNegativeDetectionCount = Statistics.TrueNegativeDetectionCountForPUPresence,
                    FalseNegativeDetectionCount = Statistics.FalseNegativeDetectionCountForPUPresence,
                    TotalDetectionCount = Statistics.TotalDetectionCountForPUPresence,
                    HardPd = Statistics.TruePositiveDetectionCountForPUPresence / (double)(Statistics.TruePositiveDetectionCountForPUPresence + Statistics.FalsePositiveDetectionCountForPUPresence),
                    HardPm = Statistics.FalsePositiveDetectionCountForPUPresence / (double)(Statistics.TruePositiveDetectionCountForPUPresence + Statistics.FalsePositiveDetectionCountForPUPresence),
                    HardPf = Statistics.FalseNegativeDetectionCountForPUPresence / (double)(Statistics.TrueNegativeDetectionCountForPUPresence + Statistics.FalseNegativeDetectionCountForPUPresence),
                    TimePUCorrectlyDetected = Statistics.TotalTimeSpentCorrectlyInProtocol_h0_h0 / 1000.0,
                    TimePU_not_existandCorrectlyDetected = Statistics.TotalTimeSpentCorrectlyInProtocol_h1_h1 / 1000.0,
                    TimePUPresentButNotDetected = Statistics.TotalTimeSpentMistakenlylyInProtocol_h0_h1 / 1000.0,
                    TimePU_notExistButFalselyDetectedPresent = Statistics.TotalTimeSpentMistakenlylyInProtocol_h1_h0 / 1000.0,
                    TotalPowerSpentForWhispering = Statistics.TotalPowerWhispering * 0.00000001,
                    TotalPowerSpentForReporting = Statistics.TotalPowerReporting * 0.00000001,
                    TotalPowerSpent = (Statistics.TotalPowerReporting + Statistics.TotalPowerWhispering) * 0.00000001,
                    AverageDistanceDifference = Statistics.AverageDistanceOfThePredictedAndActualPrimaryUserLocation,
                    AverageNumberOfDetectedMobileStations = Statistics.AverageNumberOfMSThatDetectedPrimaryUserPresence,
                    AverageNumberOfReportedMobileStations = Statistics.AverageNumberOfMSThatReportedPrimaryUserPresence,
                    TotalTimePrimaryUserExisted = (double)Statistics.TotalTimeAPrimaryUserHaveExisted / 1000.0,
                    TotalTimePrimaryUserDetected = (double)Statistics.TotalTimeAPrimaryUserHaveDetected / 1000.0,
                    AverageWhisperRadius = Statistics.AverageWhisperRadius,
                    AverageDistanceToBS = Statistics.AverageDistanceToBaseStation,
                    MinimumEnergyMode = SimParameters.ForceMinimumEnergExpenditure,
                    WhisperingEnabled = SimParameters.OperationMode,
                    AverageFailedWhisperingCount = Statistics.AverageNumberOfWhisperingAttemptsFailed
                };

                session.Save(CRStat);

                List<CRSimClassLib.TerrainModal.MobileStation> mobileStations = Simulation.Instance.GetMobileStations();

                foreach (var item in mobileStations)
                {
                    var MSreport = new MSinReport
                    {
                        Id = Guid.NewGuid(),
                        CRRunId = CRStat.Id,
                        PowerSpentReporting = item._totalPowerReporting,
                        PowerSpentWhispering = item._totalPowerWhispering
                    };

                    session.Save(MSreport);
                }

                for (int i = 0; i < Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent.Length; i++)
                {
                    var distanceBucketItem = new DistanceBucketItem
                    {
                        Id = Guid.NewGuid(),
                        CRRunId = CRStat.Id,
                        BucketNo = i * 10,
                        Value = Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent[i] / 1000.0,
                        Percentage = Statistics.DetectedAndActualDistanceDifferenceBucketInMiliSecondsSpent[i] * 100.0 / Statistics.TotalTimeAPrimaryUserHaveDetected
                    };

                    session.Save(distanceBucketItem);
                }

                // individual energy consumption of mobile stations shold be saved - 

                transaction.Commit();
            }
        }

        private static string GetFormattedSimParameters()
        { 
            var stringBuilder = new StringBuilder();

            foreach (var field in typeof(SimParameters).GetFields())
            {
                stringBuilder.AppendLine(field.Name + " : " + field.GetValue(null));
            }

            return stringBuilder.ToString();
        }

        private static void PrintStatistics()
        {
            var parameters = GetFormattedSimParameters();

            var statistics = GetFormattedStatistics();

            Console.WriteLine(parameters);

            Console.WriteLine(statistics);
        }

        private static void DumpStatisticsToFile()
        {
            LoggerRepository.Instance.AppendLog(GetFormattedSimParameters());

            LoggerRepository.Instance.AppendLog(GetFormattedStatistics());
        }

    }
}
