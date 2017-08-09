using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialMedia.Helpers;
using SocialMedia.Helpers.Attributes;

namespace SocialMedia.Twitter
{
    public class Twitter : SocialMediaModelBase
    {
        [IsIdentifier]
        [MappedField(Constants.FieldNames.ID, Constants.FieldNames.ID)]
        public String ID { get; set; }

        [MappedField(Constants.FieldNames.TwitterURL, Constants.FieldNames.TwitterURL)]
        public String TwitterURL { get; set; }

        [MappedField(Constants.FieldNames.TwitterHandle, Constants.FieldNames.TwitterHandle)]
        public String TwitterHandle { get; set; }

        [MappedField(Constants.FieldNames.Text, Constants.FieldNames.Text)]
        public String Text { get; set; }

        [MappedField(Constants.FieldNames.DateCreated, Constants.FieldNames.DateCreated)]
        public DateTime DateCreated { get; set; }

        [MappedField(Constants.FieldNames.HashTags, Constants.FieldNames.HashTags)]
        public IEnumerable<String> HashTags { get; set; }

        [MappedField(Constants.FieldNames.HasMedia, Constants.FieldNames.HasMedia)]
        public Boolean HasMedia { get; set; }

        [MappedField(Constants.FieldNames.MediaType, Constants.FieldNames.MediaType)]
        public String MediaType { get; set; }

        [MappedField(Constants.FieldNames.MediaURL, Constants.FieldNames.MediaURL)]
        public String MediaURL { get; set; }

        [MappedField(Constants.FieldNames.IsReplyTo, Constants.FieldNames.IsReplyTo)]
        public Boolean IsReplyTo { get; set; }

        [MappedField(Constants.FieldNames.IsReTweet, Constants.FieldNames.IsReTweet)]
        public Boolean IsReTweet { get; set; }

        [MappedField(Constants.FieldNames.ReTweetCount, Constants.FieldNames.ReTweetCount)]
        public Int32 ReTweetCount { get; set; }

        [MappedField(Constants.FieldNames.LikeCount, Constants.FieldNames.LikeCount)]
        public Int32 LikeCount { get; set; }

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
