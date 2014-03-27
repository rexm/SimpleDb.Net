using System;
using System.Collections.Specialized;

namespace Cucumber.SimpleDb
{
	public static class QueryStringRelatedExtensions
	{
		//http://stackoverflow.com/questions/68624/how-to-parse-a-query-string-into-a-namevaluecollection-in-net
		static NameValueCollection ToNameValueCollection(this string source)
		{
			var queryParameters = new NameValueCollection();
			var querySegments = source.Split('&');
			foreach (var segment in querySegments)
			{
				var parts = segment.Split('=');
				if (parts.Length <= 0) continue;
				var key = parts[0].Trim(new[] { '?', ' ' });
				var val = parts[1].Trim();

				queryParameters.Add(key, Uri.UnescapeDataString(val));
			}
			return queryParameters;
		}

		public static NameValueCollection ToNameValueCollection(this Uri source)
		{
			return source.Query.ToNameValueCollection();
		}
	}
}