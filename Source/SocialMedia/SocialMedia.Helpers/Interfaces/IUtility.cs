using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client.DTOs;
using Relativity.API;

namespace SocialMedia.Helpers.Interfaces
{
    public interface IUtility
    {
        Task<Stream> SendHttpRequestAsync(HttpClient client, HttpRequestMessage request);
        Task<RDO> GetFeedRDOAsync(IServicesMgr mgr, Int32 workspaceArtifactID, Guid jobIdentifier);

        Task CreateFeedRDOAsync(IServicesMgr mgr, Int32 workspaceArtifactID, Guid jobIdentifier, String serializedFeed, String sinceID);

        Task UpdateFeedRDOAsync(IServicesMgr mgr, Int32 workspaceArtifactID, Int32 feedArtifactID, String serializedFeed, String sinceID);
        Task<RDO> GetSocialMediaCustodianAsync(IServicesMgr mgr, Int32 workspaceArtifactID, Int32 socialMediaCustodianArtifactID);

        Task<IEnumerable<RDO>> GetAllSocialMediaCustodiansAsync(IServicesMgr mgr, Int32 workspaceArtifactID);

        T DeserializeObjectAsync<T>(String metaData);

        String SerializeObjectAsync<T>(T adminObject);
    }
}
