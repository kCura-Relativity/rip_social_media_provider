using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using kCura.IntegrationPoints.Contracts.Models;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Relativity.API;
using SocialMedia.Helpers;
using SocialMedia.Helpers.Interfaces;
using SocialMedia.Helpers.Models;
using SocialMedia.Twitter;
using SocialMedia.Twitter.Tests;
using Constants = SocialMedia.Helpers.Constants;

namespace SocialMedia.Provider.Tests
{

    [TestFixture]
    public class SocialMediaProviderTests2
    {
        Twitter.Twitter SampleTweet;
		Mock<IHttpService> MockHttpService = new Mock<IHttpService>();
	    Mock<IFeedRDOService> MockRDOFeedService = new Mock<IFeedRDOService>();
	    Mock<ISocialMediaCustodianService> MockSocialMediaCustodianService = new Mock<ISocialMediaCustodianService>();
		Mock<ISerializationHelper> MockSerializationHelper;
	    UtilityTestHelper UtilityTestHelper;

        Mock<IHelper> MockHelper;
        Mock<ILogFactory> MockLogFactory;
        Mock<IAPILog> MockLogger;
        Mock<IServicesMgr> MockServicesMgr;
        Mock<IRSAPIClient> MockRsapiClient;

        RDO SocialMediaCustodian;
        RDO ArchivedFeed;

        [SetUp]
        public void Init()
        {
            MockHelper = new Mock<IHelper>();
            MockLogFactory = new Mock<ILogFactory>();
            MockLogger = new Mock<IAPILog>();
            MockServicesMgr = new Mock<IServicesMgr>();
            MockRsapiClient = new Mock<IRSAPIClient>();
            UtilityTestHelper = new UtilityTestHelper();
	        MockHttpService = UtilityTestHelper.MockHttpService;
			MockSerializationHelper = UtilityTestHelper.MockSerializationHelper;
			SampleTweet = UtilityTestHelper.SampleTweet;

            MockLogFactory.Setup(x => x.GetLogger()).Returns(MockLogger.Object);
            MockHelper.Setup(x => x.GetLoggerFactory()).Returns(MockLogFactory.Object);
            MockServicesMgr.Setup(x => x.CreateProxy<IRSAPIClient>(It.IsAny<ExecutionIdentity>())).Returns(MockRsapiClient.Object);
	        MockHelper.Setup(x => x.GetServicesManager()).Returns(MockServicesMgr.Object);

            SocialMediaCustodian = new RDO(12345)
            {
                Fields = new List<FieldValue>()
                {
                    new FieldValue(Constants.Guids.Fields.SocialMediaCustodian.TWITTER) { Value = "TestTwitterHandle"},
                    new FieldValue(Constants.Guids.Fields.SocialMediaCustodian.FACEBOOK) { Value = "TestFaceBookHandle"},
                    new FieldValue(Constants.Guids.Fields.SocialMediaCustodian.LINKEDIN) { Value = "TestLinkedinHandle"}
                }
            };

            MockSocialMediaCustodianService.Setup(x => x.GetSocialMediaCustodianAsync(It.IsAny<IServicesMgr>(), It.IsAny<Int32>(), It.IsAny<Int32>())).Returns(() => Task.FromResult(SocialMediaCustodian));
	        MockRDOFeedService.Setup(x => x.GetFeedRDOAsync(It.IsAny<IServicesMgr>(), It.IsAny<Int32>(), It.IsAny<Guid>())).Returns(() => Task.FromResult(ArchivedFeed));
        }

        [TearDown]
        public void Cleanup()
        {
            UtilityTestHelper = null;
            SampleTweet = null;
            ArchivedFeed = null;
            SocialMediaCustodian = null;
            MockHelper.ResetCalls();
            MockLogFactory.ResetCalls();
            MockLogger.ResetCalls();
            MockServicesMgr.ResetCalls();
            MockRsapiClient.ResetCalls();
	        MockRDOFeedService.ResetCalls();
	        MockSocialMediaCustodianService.ResetCalls();
	        MockSerializationHelper.ResetCalls();
		}


        [Test]
        public void FieldsAreReturnedForTwitterSource()
        {
            // Arrange
            var provider = new SocialMediaProvider(MockHelper.Object)
            {
                FeedRdoService = MockRDOFeedService.Object
            };
            var config = new JobConfiguration()
            {
                JobIdentifier = Guid.NewGuid(),
                NumberOfPostsToRetrieve = 50,
                SocialMediaType = "Twitter"
            };
            var configString = JsonConvert.SerializeObject(config);
            var twitterSource = new Twitter.Twitter();
            var expectedTwitterFields = twitterSource.GetFields();

            // Act
            var result = provider.GetFields(configString);

            // Assert
            foreach (var field in expectedTwitterFields)
            {
                Assert.IsTrue(result.Where(x => x.DisplayName == field.DisplayName && x.FieldIdentifier == field.FieldIdentifier) != null);
            }
        }

        [Test]
        public void FeedRDOisCreatedInsteadOfUpdatedWhenArchiveDoesntExist()
        {
            // Arrange
            var provider = new SocialMediaProvider(MockHelper.Object)
            {
				FeedRdoService = MockRDOFeedService.Object,
	            HttpService = MockHttpService.Object,
				SocialMediaCustodianService = MockSocialMediaCustodianService.Object,
				SerializationHelper = MockSerializationHelper.Object
			};
            ArchivedFeed = null;
            var config = new JobConfiguration()
            {
                JobIdentifier = Guid.NewGuid(),
                NumberOfPostsToRetrieve = 50,
                SocialMediaType = "Twitter"
            };
            var configString = JsonConvert.SerializeObject(config);

            // Act
            provider.GetBatchableIds(new FieldEntry() {FieldIdentifier = SocialMedia.Twitter.Constants.FieldNames.ID}, configString);

			// Assert
	        MockRDOFeedService.Verify(x => x.CreateFeedRDOAsync(It.IsAny<IServicesMgr>(), It.IsAny<Int32>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<String>()), Times.Once());
	        MockRDOFeedService.Verify(x => x.UpdateFeedRDOAsync(It.IsAny<IServicesMgr>(), It.IsAny<Int32>(), It.IsAny<Int32>(), It.IsAny<String>(), It.IsAny<String>()), Times.Never);
        }

        [Test]
        public void FeedRDOisUpdatedInsteadOfCreatedWhenArchiveExists()
        {
            // Arrange
            var provider = new SocialMediaProvider(MockHelper.Object)
            {
	            FeedRdoService = MockRDOFeedService.Object,
	            HttpService = MockHttpService.Object,
	            SocialMediaCustodianService = MockSocialMediaCustodianService.Object,
	            SerializationHelper = MockSerializationHelper.Object
			};
            
            ArchivedFeed = new RDO(12345)
            {
                Fields = new List<FieldValue>
                {
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.JOB_IDENTIFIER) { ValueAsFixedLengthText = Guid.NewGuid().ToString()},
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.FEED) { ValueAsLongText = "Feed Text Blah Blah Blah"},
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.SINCE_ID) { ValueAsFixedLengthText = "12345678"}
                }
            };
            var config = new JobConfiguration()
            {
                JobIdentifier = Guid.NewGuid(),
                NumberOfPostsToRetrieve = 50,
                SocialMediaType = "Twitter"
            };
            var configString = JsonConvert.SerializeObject(config);

            // Act
            provider.GetBatchableIds(new FieldEntry() { FieldIdentifier = SocialMedia.Twitter.Constants.FieldNames.ID }, configString);

			// Assert
	        MockRDOFeedService.Verify(x => x.CreateFeedRDOAsync(It.IsAny<IServicesMgr>(), It.IsAny<Int32>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<String>()), Times.Never);
	        MockRDOFeedService.Verify(x => x.UpdateFeedRDOAsync(It.IsAny<IServicesMgr>(), It.IsAny<Int32>(), It.IsAny<Int32>(), It.IsAny<String>(), It.IsAny<String>()), Times.Once);
        }

        [Test]
        public void FirstTweetIDIsSavedAsSinceIDinCreate()
        {
            // Arrange
            String recordedSinceID = null;
            var provider = new SocialMediaProvider(MockHelper.Object)
            {
	            FeedRdoService = MockRDOFeedService.Object,
	            HttpService = MockHttpService.Object,
	            SocialMediaCustodianService = MockSocialMediaCustodianService.Object,
	            SerializationHelper = MockSerializationHelper.Object
			};
            ArchivedFeed = null;
            Twitter.Twitter[] tweets = new Twitter.Twitter[] { UtilityTestHelper.GenerateTweet("1"), UtilityTestHelper.GenerateTweet("5"), UtilityTestHelper.GenerateTweet("3") };
	        MockHttpService.Setup(x => x.SendHttpRequestAsync(It.IsAny<HttpClient>(), It.IsAny<HttpRequestMessage>()))
                .Returns<HttpClient, HttpRequestMessage>((client, msg) =>
                {
                    var retVal = UtilityTestHelper.GenerateBearerTokenResponse();
                    var authType = msg.Headers.Authorization.Scheme;
                    if (authType == SocialMedia.Twitter.Constants.AuthorizationTypes.BEARER)
                    {
                        retVal = UtilityTestHelper.GenerateFeedResponse(tweets);
                    }
                    return Task.FromResult(retVal);
                });
	        MockRDOFeedService.Setup(x => x.CreateFeedRDOAsync(It.IsAny<IServicesMgr>(), It.IsAny<Int32>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<String>()))
            .Callback<IServicesMgr, Int32, Guid, String, String>((mgr, WorkspaceGroupID, JobID, Feed, sinceID) =>
            {
                recordedSinceID = sinceID;
            })
            .Returns(Task.FromResult(true));

            var config = new JobConfiguration()
            {
                JobIdentifier = Guid.NewGuid(),
                NumberOfPostsToRetrieve = 50,
                SocialMediaType = "Twitter"
            };
            var configString = JsonConvert.SerializeObject(config);

            // Act
            provider.GetBatchableIds(new FieldEntry() { FieldIdentifier = SocialMedia.Twitter.Constants.FieldNames.ID }, configString);

            // Assert
            Assert.AreEqual(tweets.First().ID, recordedSinceID);
        }

        [Test]
        public void FirstTweetIDIsSavedAsSinceIDInUpdate()
        {
            // Arrange
            String recordedSinceID = null;
            var provider = new SocialMediaProvider(MockHelper.Object)
            {
	            FeedRdoService = MockRDOFeedService.Object,
	            HttpService = MockHttpService.Object,
	            SocialMediaCustodianService = MockSocialMediaCustodianService.Object,
	            SerializationHelper = MockSerializationHelper.Object
			};
            ArchivedFeed = new RDO(12345)
            {
                Fields = new List<FieldValue>
                {
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.JOB_IDENTIFIER) { ValueAsFixedLengthText = Guid.NewGuid().ToString()},
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.FEED) { ValueAsLongText = "Feed Text Blah Blah Blah"},
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.SINCE_ID) { ValueAsFixedLengthText = "12345678"}
                }
            };
            Twitter.Twitter[] tweets = new Twitter.Twitter[] { UtilityTestHelper.GenerateTweet("1"), UtilityTestHelper.GenerateTweet("5"), UtilityTestHelper.GenerateTweet("3") };
	        MockHttpService.Setup(x => x.SendHttpRequestAsync(It.IsAny<HttpClient>(), It.IsAny<HttpRequestMessage>()))
                .Returns<HttpClient, HttpRequestMessage>((client, msg) =>
                {
                    var retVal = UtilityTestHelper.GenerateBearerTokenResponse();
                    var authType = msg.Headers.Authorization.Scheme;
                    if (authType == SocialMedia.Twitter.Constants.AuthorizationTypes.BEARER)
                    {
                        retVal = UtilityTestHelper.GenerateFeedResponse(tweets);
                    }
                    return Task.FromResult(retVal);
                });
	        MockRDOFeedService.Setup(x => x.UpdateFeedRDOAsync(It.IsAny<IServicesMgr>(), It.IsAny<Int32>(), It.IsAny<Int32>(), It.IsAny<String>(), It.IsAny<String>()))
                .Callback<IServicesMgr, Int32, Int32, String, String>((mgr, WorkspaceGroupID, JobID, Feed, sinceID) =>
                {
                    recordedSinceID = sinceID;
                })
                .Returns(Task.FromResult(true));

            var config = new JobConfiguration()
            {
                JobIdentifier = Guid.NewGuid(),
                NumberOfPostsToRetrieve = 50,
                SocialMediaType = "Twitter"
            };
            var configString = JsonConvert.SerializeObject(config);

            // Act
            provider.GetBatchableIds(new FieldEntry() { FieldIdentifier = SocialMedia.Twitter.Constants.FieldNames.ID }, configString);

            // Assert
            Assert.AreEqual(tweets.First().ID, recordedSinceID);
        }

        [Test]
        public void SinceIDUsedInDownloadURLWhenArchiveFeedExists()
        {
            // Arrange
            var archiveFeedSinceID = "12345678";
            var provider = new SocialMediaProvider(MockHelper.Object)
            {
	            FeedRdoService = MockRDOFeedService.Object,
	            HttpService = MockHttpService.Object,
	            SocialMediaCustodianService = MockSocialMediaCustodianService.Object,
	            SerializationHelper = MockSerializationHelper.Object
			};
            ArchivedFeed = new RDO(12345)
            {
                Fields = new List<FieldValue>
                {
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.JOB_IDENTIFIER) { ValueAsFixedLengthText = Guid.NewGuid().ToString()},
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.FEED) { ValueAsLongText = "Feed Text Blah Blah Blah"},
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.SINCE_ID) { ValueAsFixedLengthText = archiveFeedSinceID}
                }
            };

            var config = new JobConfiguration()
            {
                JobIdentifier = Guid.NewGuid(),
                NumberOfPostsToRetrieve = 50,
                SocialMediaType = "Twitter"
            };
            var configString = JsonConvert.SerializeObject(config);

            // Act
            provider.GetBatchableIds(new FieldEntry() { FieldIdentifier = SocialMedia.Twitter.Constants.FieldNames.ID }, configString);

            // Assert
            var expectedInsert = "&since_id=" + archiveFeedSinceID;
            var allUrls = String.Join("", UtilityTestHelper.RequestURLs);
            Assert.IsTrue(allUrls.Contains(expectedInsert));
        }

        [Test]
        public void GetDataReturnsDataCorrectly()
        {
            // Arrange
            var provider = new SocialMediaProvider(MockHelper.Object)
            {
	            FeedRdoService = MockRDOFeedService.Object,
	            HttpService = MockHttpService.Object,
	            SocialMediaCustodianService = MockSocialMediaCustodianService.Object,
	            SerializationHelper = MockSerializationHelper.Object
			};
            Twitter.Twitter[] tweets = new Twitter.Twitter[] { UtilityTestHelper.GenerateTweet("1"), UtilityTestHelper.GenerateTweet("5") };
            var testFeed = new Dictionary<String, SocialMediaModelBase> { {tweets[0].ID, tweets[0]} , { tweets[1].ID, tweets[1] } };
            var serializationHelper = new SerializationHelper();
            var testFeedString = serializationHelper.SerializeObjectAsync(testFeed);

            ArchivedFeed = new RDO(12345)
            {
                Fields = new List<FieldValue>
                {
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.JOB_IDENTIFIER) { ValueAsFixedLengthText = Guid.NewGuid().ToString()},
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.FEED) { ValueAsLongText = testFeedString},
                    new FieldValue(Constants.Guids.Fields.SocialMediaFeed.SINCE_ID) { ValueAsFixedLengthText = "12345678"}
                }
            };
            var config = new JobConfiguration()
            {
                JobIdentifier = Guid.NewGuid(),
                NumberOfPostsToRetrieve = 50,
                SocialMediaType = "Twitter"
            };
            var configString = JsonConvert.SerializeObject(config);
            

            // Act
            var result = provider.GetData(new List<FieldEntry>(), new String[] {tweets[0].ID}, configString);

            // Assert
            using (DataTable dt = new DataTable())
            {
                var expectedHashTagFormat = String.Join(Twitter.Constants.MULTIPLE_CHOICE_DELIMITER, tweets[0].HashTags);
                var expectedMediaURL = string.Format(Twitter.Constants.MEDIA_URL_FORMAT, tweets[0].MediaURL);
                dt.Load(result);
                Assert.AreEqual(1, dt.Rows.Count);
                Assert.AreEqual(tweets[0].ID, dt.Rows[0][Twitter.Constants.FieldNames.ID].ToString());
                Assert.AreEqual(tweets[0].TwitterURL, dt.Rows[0][Twitter.Constants.FieldNames.TWITTER_URL].ToString());
                Assert.AreEqual(tweets[0].TwitterHandle, dt.Rows[0][Twitter.Constants.FieldNames.TWITTER_HANDLE].ToString());
                Assert.AreEqual(tweets[0].Text, dt.Rows[0][Twitter.Constants.FieldNames.TEXT].ToString());
                Assert.AreEqual(tweets[0].DateCreated.Date.Ticks, ((DateTime)dt.Rows[0][Twitter.Constants.FieldNames.DATE_CREATED]).Date.Ticks);
                Assert.AreEqual(expectedHashTagFormat, dt.Rows[0][Twitter.Constants.FieldNames.HASH_TAGS].ToString());
                Assert.AreEqual(tweets[0].HasMedia, Boolean.Parse(dt.Rows[0][Twitter.Constants.FieldNames.HAS_MEDIA].ToString()));
                Assert.AreEqual(tweets[0].MediaType, dt.Rows[0][Twitter.Constants.FieldNames.MEDIA_TYPE].ToString());
                Assert.AreEqual(expectedMediaURL, dt.Rows[0][Twitter.Constants.FieldNames.MEDIA_URL].ToString());
                Assert.AreEqual(tweets[0].IsReplyTo, Boolean.Parse(dt.Rows[0][Twitter.Constants.FieldNames.IS_REPLY_TO].ToString()));
                Assert.AreEqual(tweets[0].IsReTweet, Boolean.Parse(dt.Rows[0][Twitter.Constants.FieldNames.IS_RETWEET].ToString()));
                Assert.AreEqual(tweets[0].ReTweetCount, Int32.Parse(dt.Rows[0][Twitter.Constants.FieldNames.RETWEET_COUNT].ToString()));
                Assert.AreEqual(tweets[0].LikeCount, Int32.Parse(dt.Rows[0][Twitter.Constants.FieldNames.LIKE_COUNT].ToString()));
            }
        }

        [Test]
        public void GetBatchableIDsReturnsAllArchiveIDs()
        {
            // Arrange
            var provider = new SocialMediaProvider(MockHelper.Object)
            {
	            FeedRdoService = MockRDOFeedService.Object,
	            HttpService = MockHttpService.Object,
	            SocialMediaCustodianService = MockSocialMediaCustodianService.Object,
	            SerializationHelper = MockSerializationHelper.Object
			};
  
            var config = new JobConfiguration()
            {
                JobIdentifier = Guid.NewGuid(),
                NumberOfPostsToRetrieve = 50,
                SocialMediaType = "Twitter"
            };
            var configString = JsonConvert.SerializeObject(config);


            // Act
            var result = provider.GetBatchableIds(new FieldEntry() {FieldIdentifier = SocialMedia.Twitter.Constants.FieldNames.ID }, configString);
            var returnedIDs = new List<String>();
            while (result.Read())
            {
                returnedIDs.Add(result["ID"].ToString());
            }

            Assert.AreEqual(1, returnedIDs.Count);
            Assert.AreEqual(SampleTweet.ID, returnedIDs.First());
        }
    }
}
