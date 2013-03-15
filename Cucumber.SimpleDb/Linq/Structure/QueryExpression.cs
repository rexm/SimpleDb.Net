using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Cucumber.SimpleDb.Linq.Structure
{
    internal sealed class QueryExpression : SimpleDbExpression
    {
        private readonly SelectExpression _select;
        private readonly Expression _source;
        private readonly Expression _where;
        private readonly IEnumerable<OrderExpression> _orderBy;

        public QueryExpression(SelectExpression select, Expression source, Expression where, IEnumerable<OrderExpression> orderBy)
        {
            _select = select;
            _source = source;
            _where = where;
            _orderBy = orderBy ?? new List<OrderExpression>();
        }

        public SelectExpression Select
        {
            get
            {
                return _select;
            }
        }

        public Expression Where
        {
            get
            {
                return _where;
            }
        }

        public Expression Source
        {
            get
            {
                return _source;
            }
        }

        public IEnumerable<OrderExpression> OrderBy
        {
            get
            {
                return _orderBy;
            }
        }

        public override Type Type
        {
            get
            {
                return typeof(IEnumerable<ISimpleDbItem>);
            }
        }

        public override ExpressionType NodeType
        {
            get
            {
                return (ExpressionType)SimpleDbExpressionType.Query;
            }
        }
    }
}
