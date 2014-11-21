using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRSimClassLib.TerrainModal
{
    public class PrimaryUser : MobileStation
    {
        private double _transmittingPower;

        protected PrimaryUser(double x, double y, double TransmittingPower) : base(x,y,0)
        {
            _transmittingPower = TransmittingPower;
        }

        internal static PrimaryUser CreatePrimaryUser(double x, double y, double transmittingPower)
        {
            return new PrimaryUser(x, y, transmittingPower);
        }

        internal static PrimaryUser CreatePrimaryUser(TerrainPoint location, double transmittingPower)
        {
            return new PrimaryUser(location.x, location.y, transmittingPower);
        }

        public double GetTransmitingPower()
        {
            return _transmittingPower;
        }
    }
}
