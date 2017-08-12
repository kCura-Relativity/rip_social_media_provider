using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.IntegrationPoints.Contracts.Models;
using kCura.IntegrationPoints.Contracts.Provider;
using Newtonsoft.Json;
using Relativity.API;
using SocialMedia.Helpers;
using SocialMedia.Helpers.Models;

namespace SocialMedia.Provider
{
    [kCura.IntegrationPoints.Contracts.DataSourceProvider(SocialMedia.Helpers.Constants.Guids.Provider.SOCIAL_MEDIA_PROVIDER)]
    public class SocialMediaProvider : IDataSourceProvider
    {
        public IHelper Helper;

        public SocialMediaProvider(IHelper helper)
        {
            Helper = helper;
        }

        public IEnumerable<FieldEntry> GetFields(string options)
        {
            var logger = Helper.GetLoggerFactory().GetLogger();
            var jobConfig = JsonConvert.DeserializeObject<JobConfiguration>(options);

            try
            {
                var socialMediaSource = GetSocialMediaSource(jobConfig);
                return socialMediaSource.GetFields();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to retrieve fields from social media source type");
                throw;
            }
        }

        public IDataReader GetData(IEnumerable<FieldEntry> fields, IEnumerable<string> entryIds, string options)
        {
            /* This method is executed on itegration points agents to import the data into Relativity
             * Query your data source from the IDs provided, format it with the columns entered in GetFields()
             * Lastly return it as a dataReader.*/

            var dataSource = new DataTable();
            return dataSource.CreateDataReader();
        }

        public IDataReader GetBatchableIds(FieldEntry identifier, string options)
        {
            /*Return all the IDs of the datasource in a datareader, The IDs will be passed to GetData
             * to allow the data to be processed in batches*/
            var dataSource = new DataTable();
            return dataSource.CreateDataReader();
            return dataSource.CreateDataReader();
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
    }
}
