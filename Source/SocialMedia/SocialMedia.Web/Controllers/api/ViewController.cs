using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using SocialMedia.Helpers.Models;

namespace SocialMedia.Web.Controllers.api
{
	public class ViewController : ApiController
	{
		[HttpPost]
		public HttpResponseMessage GetViewFields([FromBody] object data)
		{
			var result = new List<KeyValuePair<string, string>>();
			if (data != null)
			{
				var jobConfiguration = JsonConvert.DeserializeObject<JobConfiguration>(data.ToString());
				result.Add(new KeyValuePair<string, string>("Social media type", jobConfiguration.SocialMediaType));
				result.Add(new KeyValuePair<string, string>("Number of posts to retrieve", jobConfiguration.NumberOfPostsToRetrieve.ToString()));
			}
			return Request.CreateResponse(HttpStatusCode.OK, result);
		}
	}
}