using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialMedia.Helpers;
using SocialMedia.Helpers.Interfaces;
using SocialMedia.Twitter.Models;

namespace SocialMedia.Twitter.Tests
{
    public class UtilityTestHelper
    {
        public Twitter SampleTweet;
        public Mock<IUtility> MockUtility;
        public List<String> RequestURLs = new List<string>();

        public UtilityTestHelper()
        {
            SampleTweet = GenerateTweet("1");
            MockUtility = new Mock<IUtility>();

            MockUtility.Setup(x => x.SendHttpRequestAsync(It.IsAny<HttpClient>(), It.IsAny<HttpRequestMessage>()))
                .Callback<HttpClient, HttpRequestMessage>((client, msg) =>
                {
                    RequestURLs.Add(msg.RequestUri.ToString());
                })
                .Returns<HttpClient, HttpRequestMessage>((client, msg) =>
                {
                    var retVal = UtilityTestHelper.GenerateBearerTokenResponse();
                    var authType = msg.Headers.Authorization.Scheme;
                    if (authType == SocialMedia.Twitter.Constants.AuthorizationTypes.BEARER)
                    {
                        retVal = UtilityTestHelper.GenerateFeedResponse(SampleTweet);
                    }
                    return Task.FromResult(retVal);
                });

            var utility = new Utility();
            MockUtility.Setup(x => x.DeserializeObjectAsync<Dictionary<String, SocialMediaModelBase>>(It.IsAny<String>()))
                .Returns<String>((input) => { return utility.DeserializeObjectAsync<Dictionary<String, SocialMediaModelBase>>(input); });
            MockUtility.Setup(x => x.SerializeObjectAsync<Dictionary<String, SocialMediaModelBase>>(It.IsAny<Dictionary<String, SocialMediaModelBase>>()))
                .Returns<Dictionary<String, SocialMediaModelBase>>((input) => { return utility.SerializeObjectAsync<Dictionary<String, SocialMediaModelBase>>(input); });

        }

        public static Twitter GenerateTweet(String ID)
        {
            return new Twitter()
            {
                ID = ID,
                TwitterURL = "https://www.twitter.com",
                TwitterHandle = "Microsoft",
                Text = "Test Tweet!",
                DateCreated = DateTime.Now,
                HashTags = new List<String> { "hash1" },
                HasMedia = true,
                MediaType = "Photo",
                MediaURL = "https://t.co/zg0M8K59qw",
                IsReplyTo = true,
                IsReTweet = false,
                ReTweetCount = 15,
                LikeCount = 10
            };
        }

        public static Stream GenerateBearerTokenResponse()
        {
            var token = new TwitterBearerToken()
            {
                token_type = "bearer",
                access_token = "AAAAAAAAAAAAAAAAAAAAAJw%2F1gAAAAAAOnzdMMLe5CpHqno9lsPxtT8vwr4%3DCGovTzv9iP37Lvg2WRIjcRzGhbqRMfy33ksEpy1xLrefaqsHB6"
            };
            var serializedToken = JsonConvert.SerializeObject(token);
            return new MemoryStream(Encoding.UTF8.GetBytes(serializedToken));
        }

        public static Stream GenerateFeedResponse(params Twitter[] tweets)
        {
            var retVal = String.Empty;
            var sb = new StringBuilder();
            for (var i = 0; i < tweets.Length; i++)
            {
                var tweet = tweets[i];
                var rawTweet = $@"
                    {{
                    	""{Constants.TwiiterFeedFieldNames.CREATED_AT}"": ""{tweet.DateCreated.ToString(SocialMedia.Twitter.Constants.TWITTER_DATE_TEMPLATE)}"",
                    	""{Constants.TwiiterFeedFieldNames.ID}"": {tweet.ID},
                    	""{Constants.TwiiterFeedFieldNames.TEXT}"": ""{tweet.Text}"",
                    	""{Constants.TwiiterFeedFieldNames.ENTITIES}"": {{}},
                    	""{Constants.TwiiterFeedFieldNames.IS_REPLY_TO}"": {tweet.IsReplyTo.ToString().ToLower()},
                    	""{Constants.TwiiterFeedFieldNames.RETWEET_COUNT}"": {tweet.ReTweetCount},
                    	""{Constants.TwiiterFeedFieldNames.FAVORITE_COUNT}"": {tweet.LikeCount}
                    }}
                    ";
                var tweetObj = JObject.Parse(rawTweet);
                if (tweet.HasMedia)
                {
                    var mediaString = $@"
                            [{{
                    			""{Constants.TwiiterFeedFieldNames.MEDIA_URL}"": ""{tweet.MediaURL}"",
                    			""{Constants.TwiiterFeedFieldNames.TYPE}"": ""{tweet.MediaType}""
                    		}}]";
                    var entitieas = (JObject)tweetObj[Constants.TwiiterFeedFieldNames.ENTITIES];
                    entitieas.Add(Constants.TwiiterFeedFieldNames.MEDIA, JProperty.Parse(mediaString));
                }

                if (tweet.HashTags.Any())
                {
                    var formattedHashTage = String.Empty;
                    foreach (var hash in tweet.HashTags)
                    {
                        formattedHashTage += $@"""{Constants.TwiiterFeedFieldNames.TEXT}"":""hash"",";
                    }
                    //remove last comma
                    formattedHashTage = formattedHashTage.Substring(0, formattedHashTage.Length - 1);

                    var entitieas = (JObject)tweetObj[Constants.TwiiterFeedFieldNames.ENTITIES];
                    entitieas.Add(Constants.TwiiterFeedFieldNames.HASH_TAGS, JProperty.Parse($@"[{{{formattedHashTage}}}]"));
                }
                sb.Append(tweetObj.ToString());
                if (i < tweets.Length - 1)
                {
                    sb.Append(",");
                }
            }
            retVal = "[" + sb.ToString() + "]";
            return new MemoryStream(Encoding.UTF8.GetBytes(retVal));
        }
    }
}
