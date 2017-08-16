using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Newtonsoft.Json;
using Relativity.API;
using SocialMedia.Helpers.Interfaces;
using SocialMedia.Helpers.Models;

namespace SocialMedia.Helpers
{
    public class Utility : IUtility
    {
        public async Task<Stream> SendHttpRequestAsync(HttpClient client, HttpRequestMessage request)
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

        public async Task<RDO> GetSocialMediaCustodianAsync(IServicesMgr mgr, Int32 workspaceArtifactID, Int32 socialMediaCustodianArtifactID)
        {
            return await Task.Run(() =>
            {
                using (var proxy = mgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
                {
                    proxy.APIOptions = new APIOptions() { WorkspaceID = workspaceArtifactID };
                    return proxy.Repositories.RDO.ReadSingle(socialMediaCustodianArtifactID);
                }
            });
        }

        public async Task<IEnumerable<RDO>> GetAllSocialMediaCustodiansAsync(IServicesMgr mgr, Int32 workspaceArtifactID)
        {
            
            var retVal = new List<RDO>();
            var query = new Query<RDO>
            {
                ArtifactTypeGuid = Constants.Guids.Objects.SOCIAL_MEDIA_CUSTODIAN,
                Fields = new List<FieldValue>
                {
                    new FieldValue(Constants.Guids.Fields.SocialMediaCustodian.NAME),
                    new FieldValue(Constants.Guids.Fields.SocialMediaCustodian.TWITTER),
                    new FieldValue(Constants.Guids.Fields.SocialMediaCustodian.FACEBOOK),
                    new FieldValue(Constants.Guids.Fields.SocialMediaCustodian.LINKEDIN),
                }
            };

            await Task.Run(() =>
            {
                using (var proxy = mgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
                {
                    proxy.APIOptions = new APIOptions() { WorkspaceID = workspaceArtifactID };
                    var queryResult = proxy.Repositories.RDO.Query(query);
                    if (queryResult.Success && queryResult.Results.Any())
                    {
                        foreach (var result in queryResult.Results)
                        {
                            retVal.Add(result.Artifact);
                        }
                    }
                }
            });

            return retVal;
        }

        public async Task<RDO> GetFeedRDOAsync(IServicesMgr mgr, Int32 workspaceArtifactID, Guid jobIdentifier)
        {
            RDO retVal = null;
            var query = new Query<RDO>
            {
                ArtifactTypeGuid = Constants.Guids.Objects.SOCIAL_MEDIA_FEED,
                Condition = new TextCondition(Constants.Guids.Fields.SocialMediaFeed.JOB_IDENTIFIER, TextConditionEnum.EqualTo, jobIdentifier.ToString()),
                Fields = new List<FieldValue>
                {
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.SINCE_ID),
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.FEED)
                }
            };

            await Task.Run(() =>
            {
                using (var proxy = mgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
                {
                    proxy.APIOptions = new APIOptions() { WorkspaceID = workspaceArtifactID };
                    var queryResults = proxy.Repositories.RDO.Query(query);

                    if (queryResults.Success && queryResults.Results.Any())
                    {
                        retVal = queryResults.Results[0].Artifact;
                    }
                }
            });
            
            return retVal;
        }

        public async Task CreateFeedRDOAsync(IServicesMgr mgr, Int32 workspaceArtifactID, Guid jobIdentifier, String serializedFeed, String sinceID)
        {
            var feedRDO = new RDO
            {
                ArtifactTypeGuids = new List<Guid> {Constants.Guids.Objects.SOCIAL_MEDIA_FEED },
                Fields = new List<FieldValue>
                {
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.JOB_IDENTIFIER) { Value = jobIdentifier.ToString()},
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.FEED) { Value = serializedFeed},
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.SINCE_ID) { Value = sinceID}
                }
            };
            await Task.Run(() =>
            {
                using (var proxy = mgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
                {
                    proxy.APIOptions = new APIOptions() {WorkspaceID = workspaceArtifactID};
                    proxy.Repositories.RDO.Create(feedRDO);
                }
            });
        }

        public async Task UpdateFeedRDOAsync(IServicesMgr mgr, Int32 workspaceArtifactID, Int32 feedArtifactID, String serializedFeed, String sinceID)
        {
            var feedRDO = new RDO(feedArtifactID)
            {
                ArtifactTypeGuids = new List<Guid> { Constants.Guids.Objects.SOCIAL_MEDIA_FEED },
                Fields = new List<FieldValue>
                {
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.FEED) { Value = serializedFeed},
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.SINCE_ID) { Value = sinceID}
                }
            };
            await Task.Run(() =>
            {
                using (var proxy = mgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
                {
                    proxy.APIOptions = new APIOptions() { WorkspaceID = workspaceArtifactID };
                    proxy.Repositories.RDO.Update(feedRDO);
                }
            });
        }

        public T DeserializeObjectAsync<T>(String metaData)
        {
            return JsonConvert.DeserializeObject<T>(
                value: metaData,
                settings: new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );
        }

        public String SerializeObjectAsync<T>(T obj)
        {
            return JsonConvert.SerializeObject(
                value: obj,
                formatting: Formatting.None,
                settings: new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                }
            );
        }
    }
}
