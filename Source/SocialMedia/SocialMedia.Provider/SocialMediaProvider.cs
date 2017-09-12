using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.IntegrationPoints.Contracts.Models;
using kCura.IntegrationPoints.Contracts.Provider;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Newtonsoft.Json;
using Relativity.API;
using SocialMedia.Helpers;
using SocialMedia.Helpers.Interfaces;
using SocialMedia.Helpers.Models;
using Constants = SocialMedia.Helpers.Constants;

namespace SocialMedia.Provider
{
    [kCura.IntegrationPoints.Contracts.DataSourceProvider(SocialMedia.Helpers.Constants.Guids.Provider.SOCIAL_MEDIA_PROVIDER)]
    public class SocialMediaProvider : IDataSourceProvider
    {
        private IHelper Helper;
	    public IHttpService HttpService { get; internal set; } = new HttpService();
	    public IFeedRDOService FeedRdoService { get; internal set; } = new FeedRdoService();
	    public ISerializationHelper SerializationHelper { get; internal set; } = new SerializationHelper();
	    public ISocialMediaCustodianService SocialMediaCustodianService { get; internal set; } = new SocialMediaCustodianService();

        public SocialMediaProvider(IHelper helper)
        {
            Helper = helper;
        }

        public IEnumerable<FieldEntry> GetFields(string options)
        {
            try
            {
                var jobConfig = JsonConvert.DeserializeObject<JobConfiguration>(options);
                var socialMediaSource = GetSocialMediaSource(jobConfig);
                socialMediaSource.OnRaiseMessage += LogError;
                return socialMediaSource.GetFields();
            }
            catch (Exception ex)
            {
                LogError("Error while retrieving fields from social media source type", ex);
                throw;
            }
        }

        public IDataReader GetData(IEnumerable<FieldEntry> fields, IEnumerable<string> entryIds, string options)
        {
            /* This method is executed on integration points agents to import the data into Relativity
             * Query your data source from the IDs provided, format it with the columns entered in GetFields()
             * Lastly return it as a dataReader.*/
            try
            {
                var jobConfig = JsonConvert.DeserializeObject<JobConfiguration>(options);
                var archivedFeedRDO = FeedRdoService.GetFeedRDOAsync(Helper.GetServicesManager(), jobConfig.WorkspaceArtifactID, jobConfig.JobIdentifier).Result;
                var archiveFeed = SerializationHelper.DeserializeObjectAsync<Dictionary<String,SocialMediaModelBase>>(archivedFeedRDO[Constants.Guids.Fields.SocialMediaFeed.FEED].ValueAsLongText);
                var socialMediaSource = GetSocialMediaSource(jobConfig);
                socialMediaSource.OnRaiseMessage += LogError;
                return socialMediaSource.GetData(archiveFeed, entryIds);
            }
            catch (Exception ex)
            {
                LogError("Error while retrieving data", ex);
                throw;
            }
        }

        public IDataReader GetBatchableIds(FieldEntry identifier, string options)
        {
            /*Return all the IDs of the datasource in a datareader, The IDs will be passed to GetData
             * to allow the data to be processed in batches*/
            try
            {
                // Retrieve information about the current job
                var jobConfig = JsonConvert.DeserializeObject<JobConfiguration>(options);
                var socialMediaCustodian = SocialMediaCustodianService.GetSocialMediaCustodianAsync(Helper.GetServicesManager(), jobConfig.WorkspaceArtifactID, jobConfig.SocialMediaCustodianArtifactID).Result;
                var archivedFeedRDO = FeedRdoService.GetFeedRDOAsync(Helper.GetServicesManager(), jobConfig.WorkspaceArtifactID, jobConfig.JobIdentifier).Result;
                var accountInfo = AssembleAccountInformation(socialMediaCustodian, archivedFeedRDO);
                var socialMediaSource = GetSocialMediaSource(jobConfig);
                socialMediaSource.OnRaiseMessage += LogError;
                var feed = socialMediaSource.DownloadFeed(HttpService, accountInfo, jobConfig.NumberOfPostsToRetrieve);
                SaveFeed(jobConfig, archivedFeedRDO, feed, socialMediaSource.LastDownloadedPostID);

                var dt = new DataTable();
                dt.Columns.Add(new DataColumn(identifier.FieldIdentifier, typeof(String)));
                foreach (var tweet in feed)
                {
                    dt.Rows.Add(tweet.Key);
                }

                return dt.CreateDataReader();
            }
            catch (Exception ex)
            {
                LogError("Error while retrieving Batchable IDs", ex);
                throw;
            }
        }

        private SocialMediaModelBase GetSocialMediaSource(JobConfiguration config)
        {
            SocialMediaModelBase retVal = null;
            var socialMediaType = (Constants.SocialMediaSources)Enum.Parse(typeof(Constants.SocialMediaSources), config.SocialMediaType.ToUpper());
   
            switch (socialMediaType)
            {
                case Constants.SocialMediaSources.TWITTER:
                    retVal = new Twitter.Twitter();
                    break;

                default:
                    throw new Exception("Unsupported Social Media Source");
            }

            return retVal;
        }

        private AccountInformation AssembleAccountInformation(RDO socialMediaCustodian, RDO archivedFeedRDO)
        {
            var retVal = new AccountInformation();
            retVal.TwitterAccountHandle = socialMediaCustodian[Constants.Guids.Fields.SocialMediaCustodian.TWITTER].ValueAsFixedLengthText;
            retVal.FacebookAccountHandle = socialMediaCustodian[Constants.Guids.Fields.SocialMediaCustodian.FACEBOOK].ValueAsFixedLengthText;
            retVal.LinkedinAccountHandle = socialMediaCustodian[Constants.Guids.Fields.SocialMediaCustodian.LINKEDIN].ValueAsFixedLengthText;

            if (archivedFeedRDO != null)
            {
                retVal.SinceID = archivedFeedRDO[Constants.Guids.Fields.SocialMediaFeed.SINCE_ID].ValueAsFixedLengthText;
            }

            return retVal;
        }

        private void SaveFeed(JobConfiguration config, RDO archivedFeedRDO, Dictionary<String, SocialMediaModelBase> feed, String sinceID)
        {
            try
            {
                if (feed.Any())
                {
                    var serializedFeed = SerializationHelper.SerializeObjectAsync(feed);
                    if (archivedFeedRDO == null)
                    {
                        // Create a new feedRDO for this RIP Job
                        FeedRdoService.CreateFeedRDOAsync(Helper.GetServicesManager(), config.WorkspaceArtifactID, config.JobIdentifier, serializedFeed, sinceID).Wait();
                    }
                    else
                    {
                        // Update existing feedRDO
                        FeedRdoService.UpdateFeedRDOAsync(Helper.GetServicesManager(), config.WorkspaceArtifactID, archivedFeedRDO.ArtifactID, serializedFeed, sinceID).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("Error Saving new Feed", ex);
                throw;
            }
        }

        private void LogError(String message, Exception ex)
        {
            var logger = Helper.GetLoggerFactory().GetLogger();
            logger.LogError(ex, message);
        }

  
    }
}
