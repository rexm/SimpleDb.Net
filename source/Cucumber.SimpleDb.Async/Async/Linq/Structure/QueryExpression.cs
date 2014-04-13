using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Async.Linq.Structure
{
    internal sealed class QueryExpression : SimpleDbExpression
    {
        private readonly Expression _limit;
        private readonly IEnumerable<OrderExpression> _orderBy;
        private readonly SelectExpression _select;
        private readonly Expression _source;
        private readonly bool _useConsistency;
        private readonly Expression _where;

        public QueryExpression(SelectExpression select, Expression source, Expression where, IEnumerable<OrderExpression> orderBy, Expression limit, bool useConsistency)
        {
            _select = select;
            _source = source;
            _where = where;
            _orderBy = orderBy ?? new List<OrderExpression>();
            _limit = limit;
            _useConsistency = useConsistency;
        }

        public SelectExpression Select
        {
            get { return _select; }
        }

        public Expression Where
        {
            get { return _where; }
        }

        public Expression Source
        {
            get { return _source; }
        }

        public IEnumerable<OrderExpression> OrderBy
        {
            get { return _orderBy; }
        }

        public Expression Limit
        {
            get { return _limit; }
        }

        public bool UseConsistency
        {
            get { return _useConsistency; }
        }

        public override Type Type
        {
            get { return typeof (IEnumerable<ISimpleDbItem>); }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType) SimpleDbExpressionType.Query; }
        }
    }
}