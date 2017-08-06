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
        private string _displayName;
        private string _fieldIdentifier;

        public MappedField(String displayName, string fieldIdentifier)
        {
            _displayName = displayName;
            _fieldIdentifier = fieldIdentifier;
        }
    }
}
