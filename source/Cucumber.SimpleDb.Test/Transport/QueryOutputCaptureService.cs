using System;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;

namespace Cucumber.SimpleDb.Test.Transport
{
    internal class QueryOutputCaptureService : StaticSimpleDbRestService
    {
        private readonly Action<string> _output;

        public QueryOutputCaptureService(Action<string> output)
        {
            _output = output;
        }

        public override XElement Select(string query, bool useConsistency, string nextPageToken)
        {
            _output(query);
            return base.Select(query, useConsistency, nextPageToken);
        }
    }
}

