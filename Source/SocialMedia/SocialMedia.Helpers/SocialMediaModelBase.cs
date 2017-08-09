using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using kCura.IntegrationPoints.Contracts.Models;
using SocialMedia.Helpers.Attributes;

namespace SocialMedia.Helpers
{
    public abstract class SocialMediaModelBase
    {
        public IEnumerable<FieldEntry> GetFields()
        {
            var retVal = new List<FieldEntry>();

            // Get all the properties that are decorated with the MapppedField attribute
            PropertyInfo[] mappedFieldProperties = GetType().GetProperties().Where(x => Attribute.IsDefined(x, typeof(MappedField))).ToArray();

            // Format each property as a FieldEntry and add to the list that will be returned
            foreach (PropertyInfo mappedField in mappedFieldProperties)
            {
                var mappedFieldAttribute = GetAttribute<MappedField>(mappedField);
                if (mappedFieldAttribute != null)
                {
                    // Create a new field entry based on the mapped field
                    var fieldEntry = new FieldEntry
                    {
                        DisplayName = mappedFieldAttribute.DisplayName,
                        FieldIdentifier = mappedFieldAttribute.FieldIdentifier
                    };

                    // Mark the field as an identifier if it has the attribute and there currently isn't any identifiers in the Field Entry Collection
                    var isIdentifier = GetAttribute<IsIdentifier>(mappedField);
                    var currentIdentifiers = retVal.Count(x => x.IsIdentifier == true);
                    if (isIdentifier != null && currentIdentifiers == 0)
                    {
                        fieldEntry.IsIdentifier = true;
                    }

                    retVal.Add(fieldEntry);
                }
            }
            return retVal;
        }

        private K GetAttribute<K>(PropertyInfo property) where K : Attribute
        {
            return property.GetCustomAttributes(typeof(K), false).Cast<K>().FirstOrDefault();
        }

        public abstract IDataReader GetData(Dictionary<String, SocialMediaModelBase> inputFeed, IEnumerable<String> IDs);
        public abstract Dictionary<String, SocialMediaModelBase> DownloadFeed(String options);
    }
}
