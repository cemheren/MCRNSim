using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Data.Model
{
    public class MSinReport
    {
        public virtual Guid Id { get; set; }

        public virtual Guid CRRunId { get; set; }

        public virtual double PowerSpentReporting { get; set; }

        public virtual double PowerSpentWhispering { get; set; }

        public virtual double AverageDistance { get; set; }
    }
}
