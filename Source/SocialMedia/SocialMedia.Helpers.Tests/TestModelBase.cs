using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialMedia.Helpers;
using SocialMedia.Helpers.Attributes;

namespace SocialMedia.Helpers.Tests
{
    public class TestModelBase : SocialMediaModelBase
    {
        [IsIdentifier]
        [MappedField("ID","ID")]
        public String ID { get; set; }

        [IsIdentifier]
        [MappedField("ID2", "ID2")]
        public String ID2 { get; set; }

        [MappedField("Name", "Name")]
        public String Name { get; set; }

        public String Test { get; set; }

        public override IDataReader GetData(IEnumerable<SocialMediaModelBase> inputFeed, IEnumerable<String> IDs)
        {
            var dt = new DataTable();
            return dt.CreateDataReader();
        }

        public override IEnumerable<SocialMediaModelBase> DownloadFeed(String options)
        {
            var retVal = new List<SocialMediaModelBase>();
            return retVal;
        }
    }
}
