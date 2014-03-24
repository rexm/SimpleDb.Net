using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Structure;

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
            return AggregateAndReduce(pex);
        }

        protected override Expression VisitSimpleDbQuery(QueryExpression qex)
        {
            return AggregateAndReduce(qex);
        }

        private Expression AggregateAndReduce(Expression expr)
        {
            var aggregator = new Aggregator();
            aggregator.Visit(expr);
            if (aggregator.Source is DomainExpression)
            {
                return ProjectionFromAggregation(aggregator);
            }
            var source = Visit(aggregator.Source);
            var projector = aggregator.Projector;
            return Expression.Call(
                typeof (Enumerable).GetMethod("Select", new[]
                {
                    typeof (IEnumerable<>), typeof (Func<,>)
                }).MakeGenericMethod(source.Type, projector.Type),
                source,
                aggregator.Projector);
        }

        private static Expression ProjectionFromAggregation(Aggregator aggregator)
        {
            var domain = aggregator.Source;
            var where = aggregator.AggregatedWhere;
            var order = aggregator.AggregatedOrderBy;
            var select = aggregator.AggregatedSelect;
            var projector = aggregator.Projector;
            var limit = aggregator.Limit;
            var useConsistency = aggregator.UseConsistency;
            return SimpleDbExpression.Project(
                SimpleDbExpression.Query(
                    select,
                    domain,
                    where,
                    order,
                    limit,
                    useConsistency),
                projector);
        }

        private class Aggregator : SimpleDbExpressionVisitor
        {
            private readonly HashSet<AttributeExpression> _aggregatedAttributes = new HashSet<AttributeExpression>();
            private readonly List<OrderExpression> _aggregatedOrderBy = new List<OrderExpression>();
            private Expression _aggregatedWhere;
            private Expression _limit;
            private Expression _projector;
            private ScalarExpression _scalarExpression;
            private Expression _source;
            private bool _useConsistency;

            public SelectExpression AggregatedSelect
            {
                get
                {
                    return _scalarExpression ?? new SelectExpression(_aggregatedAttributes);
                }
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

            public Expression Limit
            {
                get { return _limit; }
            }

            public Expression Source
            {
                get { return _source; }
            }

            public bool UseConsistency
            {
                get { return _useConsistency; }
            }

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
                    _aggregatedWhere = _aggregatedWhere == null 
                        ? ssex.Where 
                        : Expression.AndAlso(_aggregatedWhere, ssex.Where);
                }
                if (ssex.OrderBy != null)
                {
                    _aggregatedOrderBy.InsertRange(0, ssex.OrderBy);
                }
                if (ssex.Limit != null)
                {
                    _limit = ssex.Limit;
                }
                Visit(ssex.Select);
                _source = ssex.Source;
                if (_source is QueryExpression)
                {
                    Visit(ssex.Source);
                }
                if (ssex.UseConsistency)
                {
                    _useConsistency = true;
                }
                return ssex;
            }

            protected override Expression VisitSimpleDbDomain(DomainExpression dex)
            {
                _source = dex;
                return dex;
            }

            protected override Expression VisitSimpleDbCount(CountExpression cex)
            {
                _scalarExpression = cex;
                return cex;
            }

            protected override Expression VisitSimpleDbSelect(SelectExpression sex)
            {
                foreach (var att in sex.Attributes)
                {
                    _aggregatedAttributes.Add(att);
                }
                return sex;
            }
        }
    }
}