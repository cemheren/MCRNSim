using CRSimClassLib.TerrainModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRSimClassLib
{
    public class ReportingFrameModal
    {
        public int MS_ID { get; set; }

        public double DetectedPower { get; set; }

        public TerrainPoint Location { get; set; }

        public double Frequency { get; set; }
    }
}
