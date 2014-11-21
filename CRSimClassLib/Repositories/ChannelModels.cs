using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib.Repositories
{
    public static class ChannelModels
    {
        private const double Plo = 38.4;    //dB
        private const double SigmaS = 8;    //dB
        private const double PathLossExponent = 1.6;

        private const double meanNoiseFloor = 10;
        private const double stdDevNoiseFloor = 8;

        private static GaussianRandom _grLogNormal = new GaussianRandom(0, SigmaS);
        private static GaussianRandom _grNoiseFloor = new GaussianRandom(meanNoiseFloor, stdDevNoiseFloor);

        private const double NumberOfMeasurements = SimParameters.NumberOfMeasurementsInMS;

        public static double LogNormalChannelFading(double TransmitterPowerdB, double distance)
        {            
            var si = _grLogNormal.NextDouble();

            var receiverPowerdB = TransmitterPowerdB - Plo - PathLossExponent * 10 * Math.Log10(distance + 0.000000001) + si;

            return receiverPowerdB;
        }

        public static double NoiseFloor()
        {
            return _grNoiseFloor.NextDouble();
        }

        public static double ToLinearScale(this double powerInDecibels)
        {
            return Math.Pow(10, powerInDecibels / 10); 
        }

        public static double ToDecibels(this double powerInLinearScale)
        {
            return Math.Log10(powerInLinearScale) * 10;
        }
    }
}
