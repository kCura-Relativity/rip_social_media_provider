using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Relativity.API;
using SocialMedia.Helpers.Models;
using SocialMedia.Twitter;

namespace SocialMedia.Provider.Tests
{

    [TestFixture]
    public class SocialMediaProviderTests
    {
        Mock<IHelper> MockHelper;
        Mock<ILogFactory> MockLogFactory;
        Mock<IAPILog> MockLogger;

        [TestFixtureSetUp]
        public void Init()
        {
            MockHelper = new Mock<IHelper>();
            MockLogFactory = new Mock<ILogFactory>();
            MockLogger = new Mock<IAPILog>();

            MockLogFactory.Setup(x => x.GetLogger()).Returns(MockLogger.Object);
            MockHelper.Setup(x => x.GetLoggerFactory()).Returns(MockLogFactory.Object);

        }


        [Test]
        public void FieldsAreReturnedForTwitterSource()
        {
            // Arrange
            var provider = new SocialMediaProvider(MockHelper.Object);
            var config = new JobConfiguration()
            {
                JobArtifactID = 123456,
                NumberOfPostsToReveive = 50,
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
    }
}
