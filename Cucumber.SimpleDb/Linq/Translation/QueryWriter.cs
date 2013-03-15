using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cucumber.SimpleDb.Linq.Structure;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Utilities;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class QueryWriter : SimpleDbExpressionVisitor
    {
        private const string nameFormat = "`{0}`";
        private const string valueFormat = "\"{0}\"";

        public static string Write(QueryExpression expr)
        {
            QueryStringBuilder qsb = new QueryStringBuilder();
            QueryWriter writer = new QueryWriter(qsb);
            writer.Visit(expr);
            return qsb.ToString();
        }

        private readonly QueryStringBuilder _qsb;
        private UnaryExpression _carryingUnary;

        private QueryWriter(QueryStringBuilder qsb)
        {
            _qsb = qsb;
        }

        protected override Expression VisitSimpleDbProjection(ProjectionExpression pex)
        {
            VisitSimpleDbQuery(pex.Source);
            return pex;
        }

        protected override Expression VisitSimpleDbQuery(QueryExpression qex)
        {
            VisitSimpleDbSelect(qex.Select);
            VisitSimpleDbDomain((DomainExpression)qex.Source);
            VisitWhere(qex.Where);
            VisitOrder(qex.OrderBy);
            return qex;
        }

        protected override Expression VisitSimpleDbSelect(SelectExpression sex)
        {
            _qsb.Append("SELECT");
            string select = "*";
            if(sex.Attributes.Count() > 0)
            {
                select = string.Join(",",
                    sex.Attributes
                        .DistinctBy(att => att.Name)
                        .Select(att => string.Format(nameFormat, att.Name)));
            }
            _qsb.Append(select);
            return sex;
        }

        protected override Expression VisitSimpleDbDomain(DomainExpression dex)
        {
            _qsb.Append("FROM");
            _qsb.AppendFormat(nameFormat, dex.Domain);
            return dex;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(ISimpleDbItem))
            {
                switch (m.Member.Name)
                {
                    case "Name":
                        WriteItemName();
                        return m;
                    default:
                        throw new NotSupportedException(
                            string.Format("Querying on '{0}' is not currently supported",
                            m.Member.Name));
                }
            }
            return base.VisitMember(m);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(SimpleDbAttributeValue))
            {
                switch (m.Method.Name)
                {
                    case "Every":
                        WriteEvery(m);
                        break;
                    case "StartsWith":
                        Visit(m.Object);
                        WriteLike(((ConstantExpression)m.Arguments[0]).Value.ToString(), "", "%");
                        break;
                    case "EndsWith":
                        Visit(m.Object);
                        WriteLike(((ConstantExpression)m.Arguments[0]).Value.ToString(), "%", "");
                        break;
                    case "Contains":
                        Visit(m.Object);
                        WriteLike(((ConstantExpression)m.Arguments[0]).Value.ToString(), "%", "%");
                        break;
                    case "Between":
                        Visit(m.Object);
                        WriteBetween(
                            ((ConstantExpression)m.Arguments[0]).Value.ToString(),
                            ((ConstantExpression)m.Arguments[1]).Value.ToString());
                        break;
                    default:
                        throw new NotSupportedException(
                            string.Format("Querying on '{0}' is not currently supported",
                            m.Method.Name));
                }
                return m;
            }
            return base.VisitMethodCall(m);
        }

        private void WriteBetween(string lower, string upper)
        {
            _qsb.AppendFormat("BETWEEN {0} AND {1}",
                string.Format(valueFormat, lower),
                string.Format(valueFormat, upper));
        }

        private void WriteLike(string term, string prepend, string append)
        {
            HandleCarriedUnary();
            _qsb.Append("LIKE");
            _qsb.AppendFormat(valueFormat,
                string.Format("{0}{1}{2}",
                    prepend,
                    term,
                    append));
        }

        private void HandleCarriedUnary()
        {
            if (_carryingUnary != null)
            {
                _qsb.Append(GetOperator(_carryingUnary.NodeType));
                _carryingUnary = null;
            }
        }

        private void WriteItemName()
        {
            _qsb.Append("itemName()");
        }

        private void WriteEvery(MethodCallExpression m)
        {
            _qsb.Append("every(");
            WriteSimpleDbAttribute(m.Object as AttributeExpression);
            _qsb.Append(")");
        }

        private void VisitWhere(Expression where)
        {
            _qsb.Append("WHERE");
            Visit(where);
        }

        protected override Expression VisitSimpleDbAttribute(AttributeExpression nex)
        {
            WriteSimpleDbAttribute(nex);
            return nex;
        }

        private void WriteSimpleDbAttribute(AttributeExpression nex)
        {
            _qsb.AppendFormat(nameFormat, nex.Name);
        }

        private void VisitOrder(IEnumerable<OrderExpression> orderBys)
        {
            string orderBy = string.Join(",",
                orderBys.Select(ob => string.Format("{0} {1}",
                    string.Format(nameFormat, ob.Attribute.Name),
                    ob.Direction == SortDirection.Ascending ? "ASC" : "DESC")
                    )
                );
            if (!string.IsNullOrEmpty(orderBy))
            {
                _qsb.AppendFormat("ORDERBY {0}", orderBy);
            }
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            _carryingUnary = u;
            return Visit(u.Operand);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (ShouldHandleNull(node))
            {
                HandleNull(node);
                return node;
            }
            string op = GetOperator(node.NodeType);
            if(string.IsNullOrEmpty(op))
            {
                throw new NotSupportedException(
                    string.Format("Querying with the '{0}' operator is not currently supported",
                    node.NodeType));
            }
            _carryingUnary = null;
            VisitBinaryMember(node.Left);
            _qsb.Append(op);
            VisitBinaryMember(node.Right);
            return node;
        }

        private void HandleNull(BinaryExpression node)
        {
            var attribute = node.Left as AttributeExpression ?? node.Right as AttributeExpression;
            WriteSimpleDbAttribute(attribute);
            _qsb.Append("IS");
            switch (node.NodeType)
            {
                case ExpressionType.NotEqual:
                    _qsb.Append("NOT");
                    break;
                case ExpressionType.Equal:
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format("'{0}' comparison on null is not supported",
                        node.NodeType));
            }
            _qsb.Append("NULL");
        }

        private bool ShouldHandleNull(BinaryExpression node)
        {
            return IsNullComparison(node.Left, node.Right) || IsNullComparison(node.Right, node.Left);
        }

        private bool IsNullComparison(Expression left, Expression right)
        {
            return (left is AttributeExpression && right is ConstantExpression && ((ConstantExpression)right).Value == null);
        }

        private void VisitBinaryMember(Expression expr)
        {
            if (expr is AttributeExpression)
            {
                WriteSimpleDbAttribute((AttributeExpression)expr);
            }
            else if (expr is ConstantExpression)
            {
                _qsb.AppendFormat(valueFormat, ((ConstantExpression)expr).Value.ToSafeString().Replace("\"", "\"\""));
            }
            else
            {
                Visit(expr);
            }
        }

        private string GetOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Not:
                    return "NOT";
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "!=";
                default:
                    return null;
            }
        }

        private class QueryStringBuilder
        {
            private StringBuilder _sb;

            public QueryStringBuilder()
            {
                _sb = new StringBuilder();
            }

            public QueryStringBuilder Append(string value)
            {
                _sb.Append(" ");
                _sb.Append(value);
                return this;
            }

            public QueryStringBuilder AppendFormat(string format, params object[] values)
            {
                _sb.Append(" ");
                _sb.AppendFormat(format, values);
                return this;
            }

            public override string ToString()
            {
                return _sb.ToString().Trim();
            }
        }
    }
}
