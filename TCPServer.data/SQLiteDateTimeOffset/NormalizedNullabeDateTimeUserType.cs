using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Automapping;
using TCPServer.Data.Model;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Cfg;
using NHibernate.Linq;
using System.IO;
using NHibernate.UserTypes;
using NHibernate.SqlTypes;
using System.Data;
using NHibernate;
using FluentNHibernate.Conventions;

namespace TCPServer.Data
{
    public class NormalizedNullabeDateTimeUserType : NormalizedDateTimeUserType
    {
        public override Type ReturnedType
        {
            get { return typeof(DateTimeOffset?); }
        }
    }
}
