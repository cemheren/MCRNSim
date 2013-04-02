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
        private const double PathLossExponent = 3.5;

        private const double NumberOfMeasurements = SimParameters.NumberOfMeasurementsInMS;

        //public static double LogNormalChannelFading(double TransmitterPowerdB, double distance)
        //{
        //    var gr = new GaussianRandom(0, SigmaS);

        //    var si = gr.NextDouble();

        //    var receiverPowerdB = 0.0;

        //    for (int i = 0; i < NumberOfMeasurements; i++)
        //    {
        //        receiverPowerdB += TransmitterPowerdB - Plo - PathLossExponent * 10 * Math.Log10(distance) + si;
        //    }

        //    return receiverPowerdB / NumberOfMeasurements;
        //}

        public static double LogNormalChannelFading(double TransmitterPowerdB, double distance)
        {
            // bu gaussian property'si
            var sigmaCalculatedAccordingToNumberOfMeasurements = SigmaS / Math.Sqrt(NumberOfMeasurements);

            var gr = new GaussianRandom(0, sigmaCalculatedAccordingToNumberOfMeasurements);

            var si = gr.NextDouble();

            var receiverPowerdB = TransmitterPowerdB - Plo - PathLossExponent * 10 * Math.Log10(distance + 0.000000001) + si;

            return receiverPowerdB;
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
