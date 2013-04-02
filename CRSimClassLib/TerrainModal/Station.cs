using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRSimClassLib.TerrainModal
{
    public abstract class Station
    {
        protected TerrainPoint _location;

        public Station(double x, double y)
        {
            _location = new TerrainPoint(x, y);
        }

        public double DistanceTo(Station station)
        {
            return this._location.DistanceTo(station._location);
        }

        public virtual TerrainPoint GetLocation()
        {
            return _location;
        }
    }
}
