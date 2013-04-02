using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRSimClassLib.Repositories
{
    public class RandomNumberRepository
    {   
        private static Random random;

        private static RandomNumberRepository instance;

        private RandomNumberRepository() 
        {
            random = new Random();
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


        public double NextDouble
        {
            get
            {
                return random.NextDouble();                
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
