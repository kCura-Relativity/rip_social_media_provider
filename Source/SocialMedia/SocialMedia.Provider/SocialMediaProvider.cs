using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.IntegrationPoints.Contracts.Models;
using kCura.IntegrationPoints.Contracts.Provider;
using Relativity.API;

namespace SocialMedia.Provider
{
    public class SocialMediaProvider : IDataSourceProvider
    {
        public IHelper Helper;

        public SocialMediaProvider(IHelper helper)
        {
            Helper = helper;
        }

        public IEnumerable<FieldEntry> GetFields(string options)
        {
            /* This method runs on the webserver while users are configuring their integration point.
             * It return a list of fields for users to map to Relativity objects*/
            var fieldEntries = new List<FieldEntry>();

            /*
            fieldEntries.Add(new FieldEntry { DisplayName = GlobalConstants.TwitterFields.ID, FieldIdentifier = GlobalConstants.TwitterFields.ID, IsIdentifier = true });
            fieldEntries.Add(new FieldEntry { DisplayName = GlobalConstants.TwitterFields.TWEET_URL, FieldIdentifier = GlobalConstants.TwitterFields.TWEET_URL, IsIdentifier = false });
            */

            return fieldEntries;
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
    }
}
