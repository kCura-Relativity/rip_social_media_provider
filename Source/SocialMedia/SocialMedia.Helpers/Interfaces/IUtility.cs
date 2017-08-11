using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Helpers.Interfaces
{
    public interface IUtility
    {
        Task<Stream> SendHttpRequest(HttpClient client, HttpRequestMessage request);
    }
}
