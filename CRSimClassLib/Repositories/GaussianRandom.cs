using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRSimClassLib.Repositories
{
    public class GaussianRandom
    {
        public double Mean { get; set; }
        public double StdDev { get; set; }

        private RandomNumberRepository _randomNumbersRepository;

        private double? _unusedRandomNumber;
        
        public GaussianRandom(double mean, double stdDev)
        {
            _randomNumbersRepository = RandomNumberRepository.Instance;
            Mean = mean;
            StdDev = stdDev;
            _unusedRandomNumber = null;
        }

        public double NextDouble()
        {
            if (_unusedRandomNumber.HasValue)
            {
                var temp = _unusedRandomNumber;
                _unusedRandomNumber = null;
                return temp.Value;
            }

            var u1 = _randomNumbersRepository.NextDouble;
            var u2 = _randomNumbersRepository.NextDouble;
            
            double randStdNormalSin = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)

            double randStdNormalCos = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Cos(2.0 * Math.PI * u2); //random normal(0,1)

            _unusedRandomNumber = this.Mean + this.StdDev * randStdNormalSin;
            return (this.Mean + this.StdDev * randStdNormalCos);
        }
    }
}
