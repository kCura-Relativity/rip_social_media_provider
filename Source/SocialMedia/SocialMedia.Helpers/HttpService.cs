using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SocialMedia.Helpers.Interfaces;

namespace SocialMedia.Helpers
{
	public class HttpService : IHttpService
	{
		public async Task<Stream> SendHttpRequestAsync(HttpClient client, HttpRequestMessage request)
		{
			Stream retVal = Stream.Null;
			var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

			if (response.IsSuccessStatusCode)
			{
				retVal = await response.Content.ReadAsStreamAsync();
			}

			return retVal;
		}
	}
}
