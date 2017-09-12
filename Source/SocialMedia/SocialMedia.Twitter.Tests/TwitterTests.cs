using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SocialMedia.Helpers;
using SocialMedia.Helpers.Interfaces;
using SocialMedia.Helpers.Models;

namespace SocialMedia.Twitter.Tests
{
    [TestFixture]
    public class TwitterTests
    {
        public Twitter SampleTweet;
	    public Mock<ISerializationHelper> MockSerializationHelper;
        public Mock<IHttpService> MockHttpService;
        public UtilityTestHelper UtilityTestHelper;

        [TestFixtureSetUp]
        public void Init()
        {
            UtilityTestHelper = new UtilityTestHelper();
	        MockHttpService = UtilityTestHelper.MockHttpService;
			MockSerializationHelper = UtilityTestHelper.MockSerializationHelper;
            SampleTweet = UtilityTestHelper.SampleTweet;
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            UtilityTestHelper = null;
	        MockSerializationHelper = null;
            SampleTweet = null;
        }

        [Test]
        public void GetDataInsertsDataInCorrectColumns()
        {
            // Arrange
            var twitter = new Twitter();
            var tweet1 = UtilityTestHelper.GenerateTweet("1");
            var tweet2 = UtilityTestHelper.GenerateTweet("2");
            var requestedIDs = new List<String> { tweet1.ID};
            var inputFeed = new Dictionary<String, SocialMediaModelBase> { { tweet1.ID, tweet1}, { tweet2.ID, tweet2} };
            

            // Act
            var result = twitter.GetData(inputFeed, requestedIDs);

            // Assert
            using (DataTable dt = new DataTable())
            {
                var expectedHashTagFormat = String.Join(Constants.MULTIPLE_CHOICE_DELIMITER, tweet1.HashTags);
                var expectedMediaURL = string.Format(Constants.MEDIA_URL_FORMAT, tweet1.MediaURL);
                dt.Load(result);
                Assert.AreEqual(1, dt.Rows.Count);
                Assert.AreEqual(tweet1.ID, dt.Rows[0][Constants.FieldNames.ID].ToString());
                Assert.AreEqual(tweet1.TwitterURL, dt.Rows[0][Constants.FieldNames.TWITTER_URL].ToString());
                Assert.AreEqual(tweet1.TwitterHandle, dt.Rows[0][Constants.FieldNames.TWITTER_HANDLE].ToString());
                Assert.AreEqual(tweet1.Text, dt.Rows[0][Constants.FieldNames.TEXT].ToString());
                Assert.AreEqual(tweet1.DateCreated.Date.Ticks, ((DateTime)dt.Rows[0][Constants.FieldNames.DATE_CREATED]).Date.Ticks);
                Assert.AreEqual(expectedHashTagFormat, dt.Rows[0][Constants.FieldNames.HASH_TAGS].ToString());
                Assert.AreEqual(tweet1.HasMedia, Boolean.Parse(dt.Rows[0][Constants.FieldNames.HAS_MEDIA].ToString()));
                Assert.AreEqual(tweet1.MediaType, dt.Rows[0][Constants.FieldNames.MEDIA_TYPE].ToString());
                Assert.AreEqual(expectedMediaURL, dt.Rows[0][Constants.FieldNames.MEDIA_URL].ToString());
                Assert.AreEqual(tweet1.IsReplyTo, Boolean.Parse(dt.Rows[0][Constants.FieldNames.IS_REPLY_TO].ToString()));
                Assert.AreEqual(tweet1.IsReTweet, Boolean.Parse(dt.Rows[0][Constants.FieldNames.IS_RETWEET].ToString()));
                Assert.AreEqual(tweet1.ReTweetCount, Int32.Parse(dt.Rows[0][Constants.FieldNames.RETWEET_COUNT].ToString()));
                Assert.AreEqual(tweet1.LikeCount, Int32.Parse(dt.Rows[0][Constants.FieldNames.LIKE_COUNT].ToString()));
            }
        }

        [Test]
        public void DownloadFeedReturnsDataAsExpected()
        {
            // Arrange
            var twitter = new Twitter();
            var accountInfo = new AccountInformation()
            {
                TwitterAccountHandle = "Microsoft",
                SinceID = String.Empty
            };

            // Act
            var result = twitter.DownloadFeed(MockHttpService.Object, accountInfo, 5);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.ContainsKey(SampleTweet.ID));
            var resultingTweet = (Twitter) result[SampleTweet.ID];
            Assert.AreEqual(SampleTweet.ID, resultingTweet.ID);
            Assert.AreEqual($"https://twitter.com/{accountInfo.TwitterAccountHandle}/status/{SampleTweet.ID}", resultingTweet.TwitterURL);
            Assert.AreEqual(SampleTweet.TwitterHandle, resultingTweet.TwitterHandle);
            Assert.AreEqual(SampleTweet.Text, resultingTweet.Text);
            Assert.AreEqual(SampleTweet.DateCreated.Date.Ticks, resultingTweet.DateCreated.Date.Ticks);
            Assert.AreEqual(SampleTweet.HashTags.Count(), resultingTweet.HashTags.Count());
            Assert.AreEqual(SampleTweet.HasMedia, resultingTweet.HasMedia);
            Assert.AreEqual(SampleTweet.MediaType, resultingTweet.MediaType);
            Assert.AreEqual(SampleTweet.MediaURL, resultingTweet.MediaURL);
            Assert.AreEqual(SampleTweet.IsReplyTo, resultingTweet.IsReplyTo);
            Assert.AreEqual(SampleTweet.IsReTweet, resultingTweet.IsReTweet);
            Assert.AreEqual(SampleTweet.ReTweetCount, resultingTweet.ReTweetCount);
            Assert.AreEqual(SampleTweet.LikeCount, resultingTweet.LikeCount);
        }

        [Test]
        public void LastPostIDUpdatedAfterFeedDownloaded()
        {
            // Arrange
            var twitter = new Twitter();
            var accountInfo = new AccountInformation()
            {
                TwitterAccountHandle = "Microsoft",
                SinceID = String.Empty
            };

            // Act
            var result = twitter.DownloadFeed(MockHttpService.Object, accountInfo, 5);

            // Assert
            Assert.AreEqual(UtilityTestHelper.SampleTweet.ID, twitter.LastDownloadedPostID);
        }




    }
}
