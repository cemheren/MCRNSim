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
using System.Linq;
using NHibernate.Linq;
using System.Globalization;

namespace DeneGor
{
    static class Definetor
    {
        public static void Print(this string p) { Console.WriteLine(p.ToString(CultureInfo.InvariantCulture)); }
        public static void Print(this double p) { Console.WriteLine(p.ToString(CultureInfo.InvariantCulture)); }
        public static void Print(this int p) { Console.WriteLine(p.ToString(CultureInfo.InvariantCulture)); }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ServerDB.Instance.InitSessionFactory(csb => csb.FromConnectionStringWithKey("CRSimDeneme"));

            //Parallel.ForEach(Enumerable.Range(100, 600).Where(i => i == 100 || i == 300 || i == 500), (x) => GetEnergyConsumptionForIndividualMobileStationsNormalMode(x));

            Parallel.ForEach(Enumerable.Range(100, 600).Where(i => i == 100 || i == 300 || i == 500), (a) => GetEnergyConsumptionForIndividualMobileStationsWhispering(a));
            
        }

        static double[,] GetEnergyConsumptionForAllModes()
        {
            using (var session = ServerDB.Instance.SessionFactory.OpenSession())
            {
                var all = session.Query<CRStat>().Distinct().ToList();

                var minimumEnergyMode = session.Query<CRStat>().Where(c => c.WhisperingEnabled && c.MinimumEnergyMode).ToList();
                var normalMode = session.Query<CRStat>().Where(c => !c.WhisperingEnabled).ToList();
                var whisperingMode = session.Query<CRStat>().Where(c => c.WhisperingEnabled && !c.MinimumEnergyMode).ToList();

                double [,] energyExpenditure = new double[3, 26];

                for (int i = 100, k = 0; i <= 600; i = i + 20, k++)
                {
                    energyExpenditure[0, k] = minimumEnergyMode.Where(c => c.NumberOfStations == i).Average(c => c.TotalPowerSpent) / (i);
                    energyExpenditure[1, k] = normalMode.Where(c => c.NumberOfStations == i).Average(c => c.TotalPowerSpent) / (i);
                    energyExpenditure[2, k] = whisperingMode.Where(c => c.NumberOfStations == i).Average(c => c.TotalPowerSpent) / (i);                    
                }

                energyExpenditure.PrintMatrixToFile("EnergyExpenditure.txt");

                return energyExpenditure;
            }
        }

        static double[,] GetEnergyConsumptionForIndividualMobileStationsWhispering(int numberOfStations)
        {
            using (var session = ServerDB.Instance.SessionFactory.OpenSession())
            {
                var stats = session.Query<CRStat>().Where(c => c.WhisperingEnabled && !c.MinimumEnergyMode).Where(c => c.NumberOfStations == numberOfStations);

                var energyExpenditure = new double[stats.Count(), numberOfStations];
                var k = 0;
                foreach (var item in stats)
                {
                    var msReportItems = session.Query<MSinReport>().Where(d => d.CRRunId == item.Id).ToList().Distinct();

                    for (int i = 0; i < msReportItems.Count(); i++)
                    {
                        energyExpenditure[k, i] = (msReportItems.ElementAt(i).PowerSpentReporting + msReportItems.ElementAt(i).PowerSpentWhispering) * 0.00000001; // from nJ to J
                    }

                    k++;
                    k.Print();
                }

                energyExpenditure.PrintMatrixToFile(string.Format("MSIndividualEnergyExpenditureWhisperingMode_{0}.txt", numberOfStations));

                return energyExpenditure;
            }
        }

        static double[,] GetEnergyConsumptionForIndividualMobileStationsNormalMode(int numberOfStations)
        {
            using (var session = ServerDB.Instance.SessionFactory.OpenSession())
            {
                var stats = session.Query<CRStat>().Where(c => !c.WhisperingEnabled).Where(c => c.NumberOfStations == numberOfStations);
                
                var energyExpenditure = new double[stats.Count(), 600];
                var k = 0;
                foreach (var item in stats)
                {
                    var msReportItems = session.Query<MSinReport>().Where(d => d.CRRunId == item.Id).ToList().Distinct();

                    for (int i = 0; i < msReportItems.Count(); i++)
                    {
                        energyExpenditure[k, i] = (msReportItems.ElementAt(i).PowerSpentReporting + msReportItems.ElementAt(i).PowerSpentWhispering) * 0.00000001; // from nJ to J
                    }

                    k++;
                    k.Print();
                }

                energyExpenditure.PrintMatrixToFile(string.Format("MSIndividualEnergyExpenditureNormalMode_{0}.txt", numberOfStations));

                return energyExpenditure;
            }
        }

        static double[,] GetLocationEstimationPerformance(int distanceBucket)
        {
            using (var session = ServerDB.Instance.SessionFactory.OpenSession())
            {
                var normalStats = session.Query<CRStat>().Where(c => !c.WhisperingEnabled);
                var whisperingStats = session.Query<CRStat>().Where(c => c.WhisperingEnabled && !c.MinimumEnergyMode);
                var minimumEnergyMode = session.Query<CRStat>().Where(c => c.WhisperingEnabled && c.MinimumEnergyMode);

                var energyExpenditure = new double[3, 26];

                energyExpenditure = energyExpenditure.SetValuesInRowOrColumn("row", 0, GetAverageDistanceBucketError(session, minimumEnergyMode, distanceBucket));
                0.Print();
                energyExpenditure = energyExpenditure.SetValuesInRowOrColumn("row", 1, GetAverageDistanceBucketError(session, normalStats, distanceBucket));
                1.Print();
                energyExpenditure = energyExpenditure.SetValuesInRowOrColumn("row", 2, GetAverageDistanceBucketError(session, whisperingStats, distanceBucket));

                energyExpenditure.PrintMatrixToFile(string.Format("LocationEstimationPerformanceDisanceBucket{0}.txt", distanceBucket));

                return energyExpenditure;
            }
        }

        private static double[] GetAverageDistanceBucketError(ISession session, IQueryable<CRStat> stats, int distanceBucket)
        {
            double[] averageArray = new double[26];

            for (int i = 100, k = 0; i <= 600; i += 20, k++)
            {
                double bucketTotal = 0;
                var numberOfStationsGroupedStats = stats.Where(s => s.NumberOfStations == i);
                foreach (var item in numberOfStationsGroupedStats)
                {
                    var distanceBucketItems = session.Query<DistanceBucketItem>().Where(d => d.CRRunId == item.Id && d.BucketNo == distanceBucket).ToList().Distinct();
                    bucketTotal += distanceBucketItems.Single().Percentage;
                }
                i.Print();
                averageArray[k] = bucketTotal / numberOfStationsGroupedStats.Count();
            }

            return averageArray;
        }

        static void PrintStatistics(List<CRStat> crStats)
        {
            "Present Detected".Print();
            for (int i = 100; i <= 600; i = i + 50)
            {
                var presentDetected = crStats.Where(c => c.NumberOfStations == i).Average(c => c.TimePUCorrectlyDetected);
                presentDetected.Print();            
            }
            "Falsely Detected".Print();
            for (int i = 100; i <= 600; i = i + 50)
            {
                var falselyDetected = crStats.Where(c => c.NumberOfStations == i).Average(c => c.TimePU_notExistButFalselyDetectedPresent);
                falselyDetected.Print();
            }
            "Total Time Existed".Print();
            for (int i = 100; i <= 600; i = i + 50)
            {
                var totalTimeExisted = crStats.Where(c => c.NumberOfStations == i).Average(c => c.TotalTimePrimaryUserExisted);
                totalTimeExisted.Print();           
            }

            double tenPercentBucket = 0;
            double twentyPercentBucket = 0;

            "Ten percent".Print();
            for (int i = 100; i <= 600; i = i + 50)
            {
                var stats = crStats.Where(c => c.NumberOfStations == i);
                tenPercentBucket = 0;
                foreach (var item in stats)
                {
                    using (var session = ServerDB.Instance.SessionFactory.OpenSession())
                    {
                        var distanceBucketItems = session.Query<DistanceBucketItem>().Where(d => d.CRRunId == item.Id);

                        tenPercentBucket += distanceBucketItems.Where(d => d.BucketNo == 0).Single().Percentage;
                    }
                }
                
                var averageTEN = tenPercentBucket / stats.Count();
                averageTEN.Print();
            }

            "Twenty percent".Print();
            for (int i = 100; i <= 600; i = i + 50)
            {
                var stats = crStats.Where(c => c.NumberOfStations == i);
                twentyPercentBucket = 0;
                foreach (var item in stats)
                {
                    using (var session = ServerDB.Instance.SessionFactory.OpenSession())
                    {
                        var distanceBucketItems = session.Query<DistanceBucketItem>().Where(d => d.CRRunId == item.Id);

                        twentyPercentBucket += distanceBucketItems.Where(d => d.BucketNo == 10).Single().Percentage;
                    }
                }

                var averageTWENTY = twentyPercentBucket / stats.Count();
                averageTWENTY.Print();
            }

            "Total energy consumption".Print();
            for (int i = 100; i <= 600; i = i + 50)
            {
                var stats = crStats.Where(c => c.NumberOfStations == i);
                var totalEnergyConsumption = stats.Average(c => c.TotalPowerSpent);
                totalEnergyConsumption.Print();            
            }
            
            Console.WriteLine();

            Console.ReadLine();
        }
    }
}
