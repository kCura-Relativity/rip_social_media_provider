using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MappedField : Attribute
    {
        public String DisplayName { get; set; }
        public String FieldIdentifier { get; set; }

        public MappedField(String displayName, String fieldIdentifier)
        {
            DisplayName = displayName;
            FieldIdentifier = fieldIdentifier;
        }
    }
}
