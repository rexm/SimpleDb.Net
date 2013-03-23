using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cucumber.SimpleDb.Linq.Structure;
using System.Linq.Expressions;
using System.Reflection;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class QueryReducer : SimpleDbExpressionVisitor
    {
        public static Expression Reduce(Expression expr)
        {
            return new QueryReducer().Visit(expr);
        }

        protected override Expression VisitSimpleDbProjection(ProjectionExpression pex)
        {
            return this.AggregateAndReduce(pex);
        }

        protected override Expression VisitSimpleDbQuery(QueryExpression qex)
        {
            return this.AggregateAndReduce(qex);
        }

        private Expression AggregateAndReduce(Expression expr)
        {
            var aggregator = new Aggregator();
            aggregator.Visit(expr);
            if (aggregator.Source is DomainExpression)
            {
                return ProjectionFromAggregation(aggregator);
            }
            else
            {
                var source = Visit(aggregator.Source);
                var projector = aggregator.Projector;
                return Expression.Call(
                    typeof(Enumerable).GetMethod("Select", new []{typeof(IEnumerable<>), typeof(Func<,>)}).MakeGenericMethod(source.Type, projector.Type),
                    source,
                    aggregator.Projector);
            }
        }

        private Expression ProjectionFromAggregation(Aggregator aggregator)
        {
            var domain = aggregator.Source;
            var where = aggregator.AggregatedWhere;
            var order = aggregator.AggregatedOrderBy;
            var select = aggregator.AggregatedSelect;
            var projector = aggregator.Projector;
            return new ProjectionExpression(
                new QueryExpression(
                    select,
                    domain,
                    where,
                    order),
                projector);
        }

        private class Aggregator : SimpleDbExpressionVisitor
        {
            public SelectExpression AggregatedSelect
            {
                get { return new SelectExpression(_aggregatedSelect); }
            }

            public IEnumerable<OrderExpression> AggregatedOrderBy
            {
                get { return _aggregatedOrderBy.Distinct(); }
            }

            public Expression AggregatedWhere
            {
                get { return _aggregatedWhere; }
            }

            public Expression Projector
            {
                get { return _projector; }
            }

            public Expression Source
            {
                get { return _source; }
            }

            private List<AttributeExpression> _aggregatedSelect = new List<AttributeExpression>();
            private Expression _aggregatedWhere;
            private List<OrderExpression> _aggregatedOrderBy = new List<OrderExpression>();
            private Expression _projector;
            private Expression _source;

            protected override Expression VisitSimpleDbProjection(ProjectionExpression pex)
            {
                _projector = pex.Projector;
                VisitSimpleDbQuery(pex.Source);
                return pex;
            }

            protected override Expression VisitSimpleDbQuery(QueryExpression ssex)
            {
                if (ssex.Where != null)
                {
                    if (_aggregatedWhere == null)
                    {
                        _aggregatedWhere = ssex.Where;
                    }
                    else
                    {
                        _aggregatedWhere = Expression.AndAlso(_aggregatedWhere, ssex.Where);
                    }
                }
                if (ssex.OrderBy != null)
                {
                    _aggregatedOrderBy.InsertRange(0, ssex.OrderBy);
                }
                Visit(ssex.Select);
                _source = ssex.Source;
                if (_source is QueryExpression)
                {
                    Visit(ssex.Source);
                }
                return ssex;
            }

            protected override Expression VisitSimpleDbDomain(DomainExpression dex)
            {
                _source = dex;
                return dex;
            }

            protected override Expression VisitSimpleDbSelect(SelectExpression sex)
            {
                _aggregatedSelect.AddRange(sex.Attributes.Where(att => !_aggregatedSelect.Contains(att)));
                return sex;
            }
        }
    }
}

