using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cucumber.SimpleDb.Linq.Structure;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class ClientProjectionWriter : SimpleDbExpressionVisitor
    {
        public static ProjectionExpression Rewrite(ProjectionExpression pex)
        {
            Expression projector = pex.Projector;
			if (projector is LambdaExpression)
			{
				projector = CreateProjector((LambdaExpression)projector);
			}
			else
			{
				projector = CreateDefaultProjector();
			}
            return new ProjectionExpression(
                pex.Source,
                projector
                );
        }

        private readonly ParameterExpression _parameter;

        private ClientProjectionWriter(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitSimpleDbAttribute(AttributeExpression aex)
        {
            return CreateItemAccessor(aex);
        }

        private Expression CreateItemAccessor(AttributeExpression aex)
        {
            return Expression.MakeIndex(
                _parameter,
                typeof(ISimpleDbItem).GetProperty("Item"),
                new[] { Expression.Constant(aex.Name) }
                );
        }

		private static Expression CreateProjector(LambdaExpression originalProjector)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(ISimpleDbItem));
			ClientProjectionWriter writer = new ClientProjectionWriter(parameter);
			var projector = writer.Visit(originalProjector.Body);
			projector = Expression.Lambda(projector, originalProjector.Parameters);
			return projector;
		}

		private static Expression CreateDefaultProjector()
		{
			var param = Expression.Parameter(typeof(ISimpleDbItem));
			var projector = Expression.Lambda(param, param);
			return projector;
		}
    }
}
