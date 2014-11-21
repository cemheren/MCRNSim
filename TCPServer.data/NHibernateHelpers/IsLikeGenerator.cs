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
    public class IsLikeGenerator : BaseHqlGeneratorForMethod
    {
        public IsLikeGenerator()
        {
            SupportedMethods = new[] { ReflectionHelper.GetMethodDefinition(() => StringExtensions.IsLike(null, null)) };
        }

        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject,
            ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            return treeBuilder.Like(visitor.Visit(arguments[0]).AsExpression(),
                                    visitor.Visit(arguments[1]).AsExpression());
        }
    }
}