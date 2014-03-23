using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Linq.Structure
{
    internal enum SimpleDbExpressionType
    {
        Attribute = 2000,
        Query = 2001,
        Projection = 2002,
        Order = 2003,
        Domain = 2004,
        Select = 2005,
        Count = 2006
    }

    internal abstract class SimpleDbExpression : Expression
    {
        public static AttributeExpression Attribute(string name, Type type)
        {
            return new AttributeExpression(name, type);
        }

        public static DomainExpression Domain(ISimpleDbDomain domain)
        {
            return new DomainExpression(domain);
        }

        public static QueryExpression Query(
            SelectExpression select,
            Expression source,
            Expression where,
            IEnumerable<OrderExpression> orderBy,
            Expression limit,
            bool useConsistency)
        {
            return new QueryExpression(select, source, where, orderBy, limit, useConsistency);
        }

        public static SelectExpression Select(IEnumerable<AttributeExpression> attributes)
        {
            return new SelectExpression(attributes);
        }

        public static ProjectionExpression Project(QueryExpression query, Expression projector)
        {
            return new ProjectionExpression(query, projector);
        }

        public static CountExpression Count()
        {
            return new CountExpression();
        }

        public override string ToString()
        {
            return Enum.GetName(typeof (SimpleDbExpressionType), NodeType);
        }
    }
}