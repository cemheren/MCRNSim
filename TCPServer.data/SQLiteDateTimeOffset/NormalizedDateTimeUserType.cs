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
    public class NormalizedDateTimeUserType : IUserType
    {
        private readonly TimeZoneInfo databaseTimeZone = TimeZoneInfo.Local;


        public virtual Type ReturnedType
        {
            get { return typeof(DateTimeOffset); }
        }

        public virtual bool IsMutable
        {
            get { return false; }
        }

        public virtual object Disassemble(object value)
        {
            return value;
        }

        public virtual SqlType[] SqlTypes
        {
            get { return new[] { new SqlType(DbType.StringFixedLength, 34) }; }
        }

        public virtual bool Equals(object x, object y)
        {
            return object.Equals(x, y);
        }

        public virtual int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public virtual object NullSafeGet(IDataReader dr, string[] names, object owner)
        {
            object r = dr[names[0]];
            if (r == DBNull.Value)
            {
                return null;
            }
        	var result = DateTimeOffset.Parse(r.ToString());
        	return result;
        }

        public virtual void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null)
            {
                NHibernateUtil.DateTime.NullSafeSet(cmd, null, index);
            }
            else
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
            	var paramVal = dateTimeOffset.ToString();

                IDataParameter parameter = (IDataParameter)cmd.Parameters[index];
                parameter.Value = paramVal;
            }
        }

        public virtual object DeepCopy(object value)
        {
            return value;
        }

        public virtual object Replace(object original, object target, object owner)
        {
            return original;
        }

        public virtual object Assemble(object cached, object owner)
        {
            return cached;
        }
    }
}
