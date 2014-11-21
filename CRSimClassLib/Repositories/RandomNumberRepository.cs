using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRSimClassLib.Repositories
{
    public class RandomNumberRepository
    {   
        private static Random _random;

        private static RandomNumberRepository instance;

        private RandomNumberRepository() 
        {
            _random = new Random();
        }

        public static RandomNumberRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RandomNumberRepository();
                }
                return instance;
            }
            set 
            {
                instance = null;
            }
        }

        public double ExponantialRV(double lambda)
        {
            var u = _random.NextDouble();

            if (u == 0)
            {
                u = 0.01;
            }

            return Math.Log(1-u) * lambda * (-1);
        }

        public double NextDouble
        {
            get
            {
                return _random.NextDouble();                
            }
        }

        public double GetNextDouble(double min, double max)
        {
            var rand = this.NextDouble;
            return ((max - min) * rand + min);
        }

        public int NextInt(int min, int max)
        {
            var rand = this.NextDouble;
            return (int)((max - min) * rand + min);
        }
    }
}
