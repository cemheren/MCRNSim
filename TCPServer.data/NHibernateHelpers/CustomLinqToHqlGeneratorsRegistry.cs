using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq.Functions;
using NHibernate.Linq;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Orastus.Data.NHibernateHelpers
{
    public class CustomLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
    {
        public CustomLinqToHqlGeneratorsRegistry()
            : base()
        {
            RegisterGenerator(ReflectionHelper.GetMethodDefinition(() => StringExtensions.IsLike(null, null)),
                              new IsLikeGenerator());
        }
    }
}
