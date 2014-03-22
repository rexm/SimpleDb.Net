using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cucumber.SimpleDb.Linq.Structure;

namespace Cucumber.SimpleDb.Linq.Translation
{
    internal class QueryCommand
    {
        private readonly string _queryText;
        private readonly QueryExpression _query;

        internal QueryCommand(QueryExpression query)
        {
            _query = query;
            _queryText = QueryWriter.Write(query);
        }

        public string QueryText
        {
            get { return _queryText; }
        }

        public bool ExplicitSelect
        {
            get
            {
                return _query.Select.Attributes.Count() > 0;
            }
        }

        public bool UseConsistency
        {
            get
            {
                return _query.UseConsistency;
            }
        }

        public ISimpleDbDomain Domain
        {
            get
            {
                return ((DomainExpression)_query.Source).Domain;
            }
        }
    }
}
