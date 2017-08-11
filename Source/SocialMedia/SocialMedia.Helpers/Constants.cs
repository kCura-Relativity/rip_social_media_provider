using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Helpers
{
    public class Constants
    {
        public const Int32 MAX_RETRY_COUNT = 3;
        public class Guids
        {
            public class Provider
            {
                public static Guid SOCIAL_MEDIA_PROVIDER = new Guid("522384FF-425A-40D5-B18C-4D132774A1CC");
            }

            public class Application
            {
                public static Guid SMP_RELATIVITY_APPLICATION = new Guid("7EE06267-9F5B-4B62-986F-DF1A3271FF93");
            }
        }
    }
}
