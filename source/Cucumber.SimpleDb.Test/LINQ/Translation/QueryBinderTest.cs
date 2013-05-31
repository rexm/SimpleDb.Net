using System;
using System.Linq;
using NUnit.Framework;
using Cucumber.SimpleDb.Utilities;
using Cucumber.SimpleDb.Linq.Translation;
using System.Linq.Expressions;
using Cucumber.SimpleDb.Linq.Structure;
using Moq;
using System.Collections.Generic;

namespace Cucumber.SimpleDb.Test
{
	public abstract class QueryBinderTest
	{
        protected class QueryBinderAccessor : QueryBinder
        {
            public Expression AccessVisitMethodCall(MethodCallExpression m)
            {
                return this.VisitMethodCall(m);
            }
        }
	}

}

