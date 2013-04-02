using CRSimClassLib.TerrainModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib.RandomWaypointMobilityModel
{
    public class MobilityStateModal
    {
        public bool IsMoving { get; private set; }

        public TerrainPoint StartingPoint { get; private set; }

        public TerrainPoint EndingPoint { get; private set; }

        public double Distance { get; private set; }

        public double Speed { get; private set; }

        public int TimeCreated { get; private set; }

        public int TimeEnded { get; private set; }
        
        /// <summary>
        /// Considered Not Moving
        /// </summary>
        public MobilityStateModal(int timeEnded, TerrainPoint point)
        {
            IsMoving = false;
            Speed = -1;
            TimeCreated = Time.Instance.Now;
            TimeEnded = timeEnded;
            StartingPoint = point;
            EndingPoint = point;
            if (TimeCreated > timeEnded)
            {
                throw new InvalidOperationException("TimeCreated cannot be later than TimeEnded." 
                + " This could have been left out for user to decide, but it did not.");
            }
        }

        public MobilityStateModal(WayPoint startingPoint, WayPoint endingPoint, double speed)
        {
            IsMoving = true;
            StartingPoint = startingPoint.GetLocation();
            EndingPoint = endingPoint.GetLocation();
            TimeCreated = Time.Instance.Now;
            Distance = StartingPoint.DistanceTo(EndingPoint);

            var timePassed = (int)(Distance / speed);
            if (timePassed == 0)
	        {
		        timePassed = 1;
	        }

            TimeEnded = TimeCreated + timePassed;

            Speed = speed;

            if (TimeCreated > TimeEnded)
            {
                throw new InvalidOperationException("TimeCreated cannot be later than TimeEnded."
                + " This could have been left out for user to decide, but it did not.");
            }
        }

        public MobilityStateModal(TerrainPoint startingPoint, TerrainPoint endingPoint, double speed)
        {
            IsMoving = true;
            StartingPoint = startingPoint;
            EndingPoint = endingPoint;
            TimeCreated = Time.Instance.Now;
            Distance = StartingPoint.DistanceTo(EndingPoint);

            var timePassed = (int)(Distance / speed);
            if (timePassed == 0)
            {
                timePassed = 1;
            }

            TimeEnded = TimeCreated + timePassed;

            Speed = speed;

            if (TimeCreated > TimeEnded)
            {
                throw new InvalidOperationException("TimeCreated cannot be later than TimeEnded."
                + " This could have been left out for user to decide, but it did not.");
            }
        }
    }
}
