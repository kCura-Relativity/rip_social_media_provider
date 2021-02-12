﻿using System;
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
    public class FeedRdoService : IFeedRDOService
    {
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
            if (sinceID != null)
            {
                feedRDO.Fields.Add(new FieldValue(Constants.Guids.Fields.SocialMediaFeed.SINCE_ID) { Value = sinceID });
            }
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
    }
}
