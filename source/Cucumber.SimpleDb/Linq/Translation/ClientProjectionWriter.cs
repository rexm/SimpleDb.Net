using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Structure;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class ClientProjectionWriter : SimpleDbExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        private ClientProjectionWriter(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        public static ProjectionExpression Rewrite(ProjectionExpression pex)
        {
            var projector = pex.Projector;
            if (projector is LambdaExpression)
            {
                projector = CreateProjector((LambdaExpression) projector);
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

        private static Expression CreateProjector(LambdaExpression originalProjector)
        {
            var parameter = Expression.Parameter(typeof (ISimpleDbItem));
            var writer = new ClientProjectionWriter(parameter);
            var projector = writer.Visit(originalProjector.Body);
            projector = Expression.Lambda(projector, originalProjector.Parameters);
            return projector;
        }

        private static Expression CreateDefaultProjector()
        {
            var param = Expression.Parameter(typeof (ISimpleDbItem));
            var projector = Expression.Lambda(param, param);
            return projector;
        }

        protected override Expression VisitSimpleDbAttribute(AttributeExpression aex)
        {
            return CreateItemAccessor(aex);
        }

        private Expression CreateItemAccessor(AttributeExpression aex)
        {
            return Expression.MakeIndex(
                _parameter,
                typeof (ISimpleDbItem).GetProperty("Item"),
                new[]
                {
                    Expression.Constant(aex.Name)
                }
                );
        }
    }
}