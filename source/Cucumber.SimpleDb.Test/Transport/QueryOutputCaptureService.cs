using System;
using System.Xml.Linq;
using Cucumber.SimpleDb.Transport;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Test.Transport
{
    internal class QueryOutputCaptureService : StaticSimpleDbRestService
    {
        private readonly Action<string> _output;

        public QueryOutputCaptureService(Action<string> output)
        {
            _output = output;
        }

        public override async Task<XElement> SelectAsync(string query, bool useConsistency, string nextPageToken)
        {
            _output(query);
            return await base.SelectAsync(query, useConsistency, nextPageToken).ConfigureAwait(false);
        }
    }
}

