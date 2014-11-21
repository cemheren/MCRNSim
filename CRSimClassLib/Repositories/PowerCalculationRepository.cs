using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib.Repositories
{
    public class PowerCalculationRepository
    {
        private const double alpha_11 = 145; //nano joule
        private const double alpha_12 = 135; //nano joule
        private const double alpha_2 = 0.01; //nano joule for n = 2

        public double TransmissionPower(double distance, int numberOfBits)
        {
            return (alpha_11 + alpha_2 * (distance * distance)) * numberOfBits;
        }

        public double ReceptionPower(double distance, int numberOfBits)
        {
            return alpha_12 * numberOfBits;
        }
    }
}
