using CRSimClassLib.Repositories;
using CRSimClassLib.TerrainModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRSimClassLib.RandomWaypointMobilityModel
{
    public class WayPoint
    {
        protected TerrainPoint _location { get; private set; }

        protected WayPoint(double x, double y)
        {
            _location = new TerrainPoint(x, y);
        }

        public TerrainPoint GetLocation()
        {
            return _location;
        }

        internal static WayPoint NewWayPoint(Terrain terrain)
        {
            var leftUp = terrain._leftUpCorner;
            var rightDown = terrain._rightDownCorner;

            var randx = RandomNumberRepository.Instance.GetNextDouble(leftUp.x, rightDown.x);
            var randy = RandomNumberRepository.Instance.GetNextDouble(leftUp.y, rightDown.y);

            return new WayPoint(randx, randy);
        }

        internal static List<WayPoint> NewWayPointList(Terrain terrain, int numberOfWaypoints)
        {
            var list = new List<WayPoint>();

            for (int i = 0; i < numberOfWaypoints; i++)
            {
                list.Add(NewWayPoint(terrain));
            } 
            
            return list;
        }
    }
}
