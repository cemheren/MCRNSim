using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRSimClassLib.TerrainModal
{
    public class PrimaryUser : Station
    {
        private double _transmittingPower;

        public PrimaryUser(double x, double y, double TransmittingPower) : base(x,y)
        {
            _transmittingPower = TransmittingPower;
        }

        public double GetTransmitingPower()
        {
            return _transmittingPower;
        }
    }
}
