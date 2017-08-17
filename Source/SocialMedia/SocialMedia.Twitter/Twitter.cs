using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using SocialMedia.Helpers;
using SocialMedia.Helpers.Attributes;
using SocialMedia.Helpers.Interfaces;
using SocialMedia.Twitter.Models;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using Newtonsoft.Json.Linq;
using SocialMedia.Helpers.Models;

namespace SocialMedia.Twitter
{
    public class Twitter : SocialMediaModelBase
    {
        #region Fields
        [IsIdentifier]
        [MappedField(Constants.FieldNames.ID, Constants.FieldNames.ID)]
        public String ID { get; set; } = String.Empty;

        [MappedField(Constants.FieldNames.TWITTER_URL, Constants.FieldNames.TWITTER_URL)]
        public String TwitterURL { get; set; } = String.Empty;

        [MappedField(Constants.FieldNames.TWITTER_HANDLE, Constants.FieldNames.TWITTER_HANDLE)]
        public String TwitterHandle { get; set; } = String.Empty;

        [MappedField(Constants.FieldNames.TEXT, Constants.FieldNames.TEXT)]
        public String Text { get; set; } = String.Empty;

        [MappedField(Constants.FieldNames.DATE_CREATED, Constants.FieldNames.DATE_CREATED)]
        public DateTime DateCreated { get; set; }

        [MappedField(Constants.FieldNames.HASH_TAGS, Constants.FieldNames.HASH_TAGS)]
        public IEnumerable<String> HashTags { get; set; } = new List<string>();

        [MappedField(Constants.FieldNames.HAS_MEDIA, Constants.FieldNames.HAS_MEDIA)]
        public Boolean HasMedia { get; set; }

        [MappedField(Constants.FieldNames.MEDIA_TYPE, Constants.FieldNames.MEDIA_TYPE)]
        public String MediaType { get; set; } = String.Empty;

        [MappedField(Constants.FieldNames.MEDIA_URL, Constants.FieldNames.MEDIA_URL)]
        public String MediaURL { get; set; } = String.Empty;

        [MappedField(Constants.FieldNames.IS_REPLY_TO, Constants.FieldNames.IS_REPLY_TO)]
        public Boolean IsReplyTo { get; set; }

        [MappedField(Constants.FieldNames.IS_RETWEET, Constants.FieldNames.IS_RETWEET)]
        public Boolean IsReTweet { get; set; }

        [MappedField(Constants.FieldNames.RETWEET_COUNT, Constants.FieldNames.RETWEET_COUNT)]
        public Int32 ReTweetCount { get; set; }

        [MappedField(Constants.FieldNames.LIKE_COUNT, Constants.FieldNames.LIKE_COUNT)]
        public Int32 LikeCount { get; set; }
        #endregion

        #region PublicOverriddenMethods

        public override String LastDownloadedPostID { get; set; }
        public override IDataReader GetData(Dictionary<String, SocialMediaModelBase> inputFeed, IEnumerable<String> IDs)
        {
            var dt = GenerateDataTable();

            foreach (var ID in IDs)
            {
                if (inputFeed.ContainsKey(ID))
                {
                    try
                    {
                        var item = inputFeed[ID];
                        var tweet = item as Twitter;

                        if (tweet != null)
                        {
                            dt.Rows.Add(tweet.ID, tweet.TwitterURL, tweet.TwitterHandle, tweet.Text, tweet.DateCreated, FormatHashTags(tweet.HashTags), tweet.HasMedia, tweet.MediaType, FormatMediaURL(tweet.MediaURL), tweet.IsReplyTo, tweet.IsReTweet, tweet.ReTweetCount, tweet.LikeCount);
                        }
                    }
                    catch (Exception ex)
                    {
                        RaiseError(ex.Message, ex);
                        throw;
                    }
                }
            }
            return dt.CreateDataReader();
        }

        public override Dictionary<String, SocialMediaModelBase> DownloadFeed(IUtility utility, AccountInformation accountInfo, Int32 maxPosts)
        {
            var retVal = new Dictionary<String, SocialMediaModelBase>();
            try
            {
                var bearerToken = RequestBearerToken(utility, accountInfo.Key, accountInfo.Secret).Result;
                var feed = RequestFeed(utility, bearerToken.access_token, accountInfo.TwitterAccountHandle, accountInfo.SinceID, maxPosts).Result;

                if (feed.Any())
                {
                    // Twiiter feeds are sorted in Descending Order, so subsequent feed requests should start from the first ID of this present feed
                    LastDownloadedPostID = feed.First().ID;

                    // Add Tweets to Dictionary
                    foreach (var tweet in feed)
                    {
                        if (!retVal.ContainsKey(tweet.ID))
                        {
                            retVal.Add(tweet.ID, tweet);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                RaiseError(ex.Message, ex);
                throw;
            }

            return retVal;
        }
        #endregion

        #region PrivateMethods
        private String FormatHashTags(IEnumerable<String> hashTags)
        {
            var retVal = String.Empty;
            if (hashTags != null && hashTags.Any())
            {
                retVal = String.Join(Constants.MULTIPLE_CHOICE_DELIMITER, hashTags);
            }
            return retVal;
        }

        private DataTable GenerateDataTable()
        {
            var retVal = new DataTable();
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.ID) { DataType = ID.GetType() });
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.TWITTER_URL) { DataType = TwitterURL.GetType() });
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.TWITTER_HANDLE) { DataType = TwitterHandle.GetType() });
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.TEXT) { DataType = Text.GetType() });
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.DATE_CREATED) { DataType = DateCreated.GetType() });
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.HASH_TAGS) { DataType = typeof(String) });
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.HAS_MEDIA) { DataType = HasMedia.GetType() });
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.MEDIA_TYPE) { DataType = MediaType.GetType() });
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.MEDIA_URL) { DataType = MediaURL.GetType() });
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.IS_REPLY_TO) { DataType = IsReplyTo.GetType() });
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.IS_RETWEET) { DataType = IsReTweet.GetType() });
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.RETWEET_COUNT) { DataType = ReTweetCount.GetType() });
            retVal.Columns.Add(new DataColumn(Constants.FieldNames.LIKE_COUNT) { DataType = LikeCount.GetType() });
            return retVal;
        }

        private async Task<TwitterBearerToken> RequestBearerToken(IUtility utility, String consumerKey, String consumerSecret)
        {
            TwitterBearerToken retVal = null;

            using (var client = new HttpClient())
            {
                var twitterToken = String.Concat(consumerKey, ":", consumerSecret);
                var endcodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(twitterToken));
                var content = new Dictionary<String, String> { { Constants.FormValues.GRANT_TYPE, Constants.FormValues.CLIENT_CREDENTIALS } };
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(Constants.URLs.REQUEST_BEARER_TOKEN),
                    Method = HttpMethod.Post,
                    Content = new FormUrlEncodedContent(content)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue(Constants.AuthorizationTypes.BASIC, endcodedToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));

                using (var responseStream = await utility.SendHttpRequestAsync(client, request))
                using (var streamReader = new StreamReader(responseStream))
                using (var reader = new JsonTextReader(streamReader))
                {
                    if (responseStream != Stream.Null)
                    {
                        var serializer = new JsonSerializer();
                        retVal = serializer.Deserialize<TwitterBearerToken>(reader);
                    }
                }
            }

            return retVal;
        }

        private async Task<IEnumerable<Twitter>> RequestFeed(IUtility utility, String bearerToken, String twitterHandle, String sinceID, Int32 maxPosts)
        {
            var retVal = new List<Twitter>();
           
            using (var client = new HttpClient())
            {
                var url = FormatFeedURL(twitterHandle, sinceID, maxPosts);
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get,
                };
                request.Headers.Authorization = new AuthenticationHeaderValue(Constants.AuthorizationTypes.BEARER, bearerToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));

                using (var responseStream = await utility.SendHttpRequestAsync(client, request))
                using (var streamReader = new StreamReader(responseStream))
                using (var reader = new JsonTextReader(streamReader))
                {
                    if (responseStream != Stream.Null)
                    {
                        retVal.AddRange(FormatFeed(reader, twitterHandle));
                    }
                }
            }

            return retVal;
        }

        private String FormatMediaURL(String mediaURL)
        {
            return string.Format(Constants.MEDIA_URL_FORMAT, mediaURL);
        }

        private String FormatFeedURL(String twitterHandle, String sinceID, Int32 maxPosts)
        {
            var max = maxPosts < 1 ? SocialMedia.Helpers.Constants.DEFAULT_NUMBER_OF_POSTS_TO_DOWNLOAD : maxPosts;
            var retVal = String.Format(Constants.URLs.DOWNLOAD_FEED, twitterHandle, max);
            
            if (!String.IsNullOrWhiteSpace(sinceID))
            {
                retVal = retVal + "&since_id=" +sinceID.Trim();
            }
            return retVal;
        }

        private IEnumerable<Twitter> FormatFeed(JsonReader reader, string twitterHandle)
        {
            List<Twitter> retVal = new List<Twitter>();

            var rawTweets = JArray.Load(reader);

            for (var i = 0; i < rawTweets.Count; i++)
            {
                JToken rawTweet = null;
                try
                {
                    rawTweet = rawTweets[i];
                    var tweetID = rawTweet[Constants.TwiiterFeedFieldNames.ID].ToString();

                    var tweet = new Twitter()
                    {
                        ID = tweetID,
                        TwitterURL = $"https://twitter.com/{twitterHandle}/status/{tweetID}",
                        TwitterHandle = twitterHandle,
                        Text = rawTweet[Constants.TwiiterFeedFieldNames.TEXT].ToString(),
                        DateCreated = DateTime.ParseExact(rawTweet[Constants.TwiiterFeedFieldNames.CREATED_AT].ToString(), Constants.TWITTER_DATE_TEMPLATE, new System.Globalization.CultureInfo("en-us")),
                        HasMedia = rawTweet[Constants.TwiiterFeedFieldNames.ENTITIES][Constants.TwiiterFeedFieldNames.MEDIA] != null,
                        IsReplyTo = rawTweet[Constants.TwiiterFeedFieldNames.IS_REPLY_TO] != null,
                        IsReTweet = rawTweet[Constants.TwiiterFeedFieldNames.RETWEET_STATUS] != null,
                        ReTweetCount = Convert.ToInt32(rawTweet[Constants.TwiiterFeedFieldNames.RETWEET_COUNT]),
                        LikeCount = Convert.ToInt32(rawTweet[Constants.TwiiterFeedFieldNames.FAVORITE_COUNT])
                    };

                    // Hash Tags
                    var hashTags = new List<string>();
                    foreach (var hashTag in rawTweet[Constants.TwiiterFeedFieldNames.ENTITIES][Constants.TwiiterFeedFieldNames.HASH_TAGS])
                    {
                        hashTags.Add(hashTag[Constants.TwiiterFeedFieldNames.TEXT].ToString());
                    }
                    tweet.HashTags = hashTags;

                    // Media
                    if (tweet.HasMedia)
                    {
                        tweet.MediaURL = rawTweet[Constants.TwiiterFeedFieldNames.ENTITIES][Constants.TwiiterFeedFieldNames.MEDIA][0][Constants.TwiiterFeedFieldNames.MEDIA_URL].ToString();
                        tweet.MediaType = rawTweet[Constants.TwiiterFeedFieldNames.ENTITIES][Constants.TwiiterFeedFieldNames.MEDIA][0][Constants.TwiiterFeedFieldNames.TYPE].ToString();
                    }

                    retVal.Add(tweet);
                    rawTweet = null;
                }
                catch (Exception ex)
                {
                    var errorMsg = "Error parsing tweet";
                    if (rawTweet != null)
                    {
                        errorMsg += ": " + rawTweet.ToString();
                    }
                    RaiseError(errorMsg, ex);
                }
            }

            return retVal;
        }
        #endregion
    }
}
