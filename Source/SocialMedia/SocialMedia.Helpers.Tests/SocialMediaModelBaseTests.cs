using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SocialMedia.Helpers.Tests
{
    [TestFixture]
    public class SocialMediaModelBaseTests
    {
        [Test]
        public void ModelBaseReturnsOnlyMappedFieldProperties()
        {
            // Arrange
            var testModel = new TestModelBase();

            // Act
            var mappedFields = testModel.GetFields();

            // Assert
            Assert.AreEqual(3, mappedFields.Count());
        }

        [Test]
        public void ModelBaseReturnsOnlySingleFieldDecoratedAsIdentifier()
        {
            // Arrange
            var testModel = new TestModelBase();

            // Act
            var mappedFields = testModel.GetFields();

            // Assert
            var numberOfIdentifiers = mappedFields.Count(x => x.IsIdentifier == true);
            Assert.AreEqual(1, numberOfIdentifiers);
        }
    }
}
