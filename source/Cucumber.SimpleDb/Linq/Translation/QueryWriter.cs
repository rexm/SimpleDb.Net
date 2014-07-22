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
            Visit(qex.Select);
            Visit((DomainExpression)qex.Source);
            VisitWhere(qex.Where);
            VisitOrder(qex.OrderBy);
            VisitLimit(qex.Limit);
            return qex;
        }

        protected override Expression VisitSimpleDbCount (CountExpression cex)
        {
            _qsb.Append ("SELECT COUNT(*)");
            return cex;
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
            _qsb.AppendFormat(nameFormat, CreateSystemNameString(dex.Domain.Name));
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
            if (IsAttributeValueMethod(m))
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
                    case "In":
                        WriteIn(m);
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

        private bool IsAttributeValueMethod(Expression exp)
        {
            var m = exp as MethodCallExpression;
            return m != null && m.Method.DeclaringType == typeof(SimpleDbAttributeValue);
        }

        private void WriteBetween(string lower, string upper)
        {
            _qsb.AppendFormat("BETWEEN {0} AND {1}",
                string.Format(valueFormat, CreateUserValueString(lower)),
                string.Format(valueFormat, CreateUserValueString(upper)));
        }

        private void WriteLike(string term, string prepend, string append)
        {
            HandleCarriedUnary();
            _qsb.Append("LIKE");
            _qsb.AppendFormat(valueFormat,
                string.Format("{0}{1}{2}",
                    prepend,
                    CreateUserValueString(term),
                    append));
        }

        private void WriteIn (MethodCallExpression m)
        {
            VisitSimpleDbAttribute((AttributeExpression)m.Object);
            _qsb.Append("IN(");
            var sequenceArgument = GetValueArray(m.Arguments[0]);
            if(sequenceArgument.Type != typeof(object[]))
            {
                throw new NotSupportedException(string.Format(
                    "Cannot use value type '{0}' with IN operator",
                    sequenceArgument.Type.Name));
            }
            WriteCommaSeparatedSequence(
                (IEnumerable<object>)sequenceArgument.Value, value => {
                _qsb.AppendFormat(valueFormat, CreateUserValueString(value));
            });
            _qsb.Append(")");
        }

        private ConstantExpression GetValueArray (Expression exp)
        {
            if(exp is NewArrayExpression)
            {
                var values = new List<object>();
                foreach(var valueExpression in ((NewArrayExpression)exp).Expressions)
                {
                    var visitedValueExpression = Visit (valueExpression);
                    if((visitedValueExpression is ConstantExpression) == false)
                    {
                        throw new NotSupportedException(string.Format(
                            "Cannot use expression type '{0}' in literal value array",
                            visitedValueExpression.NodeType));
                    }
                    values.Add(((ConstantExpression)visitedValueExpression).Value);
                }
                return Expression.Constant(values.ToArray());
            }
            else if (exp is ConstantExpression)
            {
                return (ConstantExpression)exp;
            }
            else
            {
                throw new NotSupportedException(string.Format(
                    "Cannot convert expression type '{0}' to a literal value array",
                    exp.NodeType));
            }
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
            if(where == null)
            {
                return;
            }
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
            _qsb.AppendFormat(nameFormat, CreateSystemNameString(nex.Name));
        }

        private void VisitLimit(Expression expression)
        {
            if(expression == null)
            {
                return;
            }
            var limitConstant = expression as ConstantExpression;
            if(limitConstant != null && limitConstant.Value != null && limitConstant.Type == typeof(int))
            {
                _qsb.AppendFormat("LIMIT {0}", limitConstant.Value);
            }
            else
            {
                throw new NotSupportedException("LIMIT must be a literal integer value");
            }
        }

        private void VisitOrder(IEnumerable<OrderExpression> orderBys)
        {
			if(orderBys.Any() == false)
			{
				return;
			}
			_qsb.Append("ORDERBY");
			WriteCommaSeparatedSequence(orderBys, orderBy => {
				WriteSimpleDbAttribute(orderBy.Attribute);
				_qsb.Append(orderBy.Direction == SortDirection.Ascending ? "ASC" : "DESC");
			});
        }

		private void WriteCommaSeparatedSequence<T>(IEnumerable<T> sequence, Action<T> writeItem)
		{
			bool first = true;
			foreach(var item in sequence)
			{
				if(!first)
				{
					_qsb.Append(",");
				}
				writeItem(item);
                if(first)
                {
                    first = false;
                }
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
            if (IsNegatedValueMethod(node))
            {
                Visit (ConvertToNotUnary(node));
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

        private bool IsNegatedValueMethod (BinaryExpression node)
        {
            var oneIsAttributeMethod = IsAttributeValueMethod(node.Left) || IsAttributeValueMethod(node.Right);
            return oneIsAttributeMethod && CanTreatAsNegatedUnary(node);
        }

        private bool CanTreatAsNegatedUnary(BinaryExpression node)
        {
            var booleanConstant = node.Left as ConstantExpression ?? node.Right as ConstantExpression;
            if(booleanConstant == null || booleanConstant.Value == null || booleanConstant.Value.GetType () != typeof(bool))
            {
                return false;
            }
            var value = (bool)booleanConstant.Value;
            return (value == true && node.NodeType == ExpressionType.NotEqual)
                || (value == false && node.NodeType == ExpressionType.Equal);
        }

        private UnaryExpression ConvertToNotUnary(BinaryExpression node)
        {
            var operandNode = node.Left.NodeType == ExpressionType.Constant ? node.Right : node.Left;
            return Expression.Not(operandNode);
        }

        private bool IsBooleanConstant(Expression exp)
        {
            var constant = exp as ConstantExpression;
            return constant != null && constant.Value.GetType() == typeof(bool);
        }

        private void VisitBinaryMember(Expression expr)
        {
            if (expr is AttributeExpression)
            {
                WriteSimpleDbAttribute((AttributeExpression)expr);
            }
            else if (expr is ConstantExpression)
            {
                _qsb.AppendFormat(valueFormat, CreateUserValueString(((ConstantExpression)expr).Value));
            }
            else
            {
                Visit(expr);
            }
        }

        private string CreateUserValueString(object value)
        {
            var stringValue = value.ToSafeString();
            if(value.GetType() == typeof(DateTime))
            {
                stringValue = ((DateTime)value).ToString("o");
            }
			return stringValue
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
				.Replace("%", "\\%");
        }

        private string CreateSystemNameString(string name)
        {
            return name.ToSafeString()
                .Replace("`", "``");
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
                if(value.StartsWith(",") == false)
                {
                   _sb.Append(" ");
                }
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
