using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cucumber.SimpleDb.Test.Fakes
{
	//http://stackoverflow.com/questions/10693955/stubbing-or-mocking-asp-net-web-api-httpclient
	internal class FakeHttpMessageHandler : HttpMessageHandler
	{
		private readonly HttpResponseMessage _responseMessage;
		private readonly Action<HttpRequestMessage> _requestFilter;
		

		public FakeHttpMessageHandler(HttpResponseMessage responseMessage, Action<HttpRequestMessage> requestFilter)
		{
			_responseMessage = responseMessage;
			_requestFilter = requestFilter;
		}

		public HttpRequestMessage RequestMessage { get; private set; }

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			_requestFilter(request);
			return Task.Factory.StartNew(() => _responseMessage, cancellationToken);
		}
	}
}