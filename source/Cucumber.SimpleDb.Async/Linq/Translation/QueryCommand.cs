﻿using System.Linq;
using Cucumber.SimpleDb.Async.Linq.Structure;

namespace Cucumber.SimpleDb.Async.Linq.Translation
{
    internal class QueryCommand
    {
        private readonly QueryExpression _query;
        private readonly string _queryText;

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
            get { return _query.Select.Attributes.Any(); }
        }

        public bool UseConsistency
        {
            get { return _query.UseConsistency; }
        }

        public ISimpleDbDomain Domain
        {
            get { return ((DomainExpression) _query.Source).Domain; }
        }
    }
}