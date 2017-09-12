using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Helpers.Interfaces
{
	public interface IHttpService
	{
		Task<Stream> SendHttpRequestAsync(HttpClient client, HttpRequestMessage request);
	}
}
