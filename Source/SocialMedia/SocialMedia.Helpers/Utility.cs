using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SocialMedia.Helpers.Interfaces;

namespace SocialMedia.Helpers
{
    public class Utility : IUtility
    {
        public async Task<Stream> SendHttpRequest(HttpClient client, HttpRequestMessage request)
        {
            Stream retVal = Stream.Null;
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.IsSuccessStatusCode)
            {
                /*
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var streamReader = new StreamReader(stream))
                using (var reader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();
                    tokenResponse = serializer.Deserialize<T>(reader);
                }
                */
                retVal =  await response.Content.ReadAsStreamAsync();
            }
  
            return retVal;
        }
    }
}
