using System;
using System.Collections.Generic;
using System.Globalization;
using FluentNHibernate.Automapping;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;

namespace TCPServer.Data
{
    public class CustomAutoConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.Namespace == typeof(TCPServer.Data.Model.User).Namespace;
        }

        public override bool ShouldMap(FluentNHibernate.Member member)
        {
            if (member.MemberInfo is PropertyInfo)
            {
                var pi = member.MemberInfo as PropertyInfo;
                if (pi.CanWrite != true) return false;
            }
            return base.ShouldMap(member);
        }
    }
}
