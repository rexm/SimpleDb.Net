using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Cucumber.SimpleDb.Linq.Structure
{
    internal class DomainExpression : SimpleDbExpression
    {
        private readonly ISimpleDbDomain _domain;

        public DomainExpression(ISimpleDbDomain domain)
        {
            _domain = domain;
        }

        public ISimpleDbDomain Domain
        {
            get { return _domain; }
        }

        public override Type Type
        {
            get
            {
                return typeof(Query<ISimpleDbItem>);
            }
        }

        public override ExpressionType NodeType
        {
            get
            {
                return (ExpressionType)SimpleDbExpressionType.Domain;
            }
        }
    }
}
