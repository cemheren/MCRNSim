using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib.TerrainModal
{
    public class TerrainPoint
    {
        public double x { get; set; }

        public double y { get; set; }

        public TerrainPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double DistanceTo(TerrainPoint point)
        {
            var xDiff = this.x - point.x;
            var yDiff = this.y - point.y;
            return Math.Sqrt( xDiff * xDiff + yDiff * yDiff);
        }

        public static TerrainPoint operator +(TerrainPoint left, TerrainPoint right)
        {
            return new TerrainPoint(right.x + left.x, right.y + left.y);
        }
    }
}
