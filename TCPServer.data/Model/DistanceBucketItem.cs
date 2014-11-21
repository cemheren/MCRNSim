using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Data.Model
{
    public class DistanceBucketItem
    {
        public virtual Guid Id { get; set; }

        public virtual Guid CRRunId { get; set; }

        public virtual int BucketNo { get; set; }

        public virtual double Value { get; set; }

        public virtual double Percentage { get; set; }
    }
}
