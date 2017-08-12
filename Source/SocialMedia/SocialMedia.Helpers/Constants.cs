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

        public enum SocialMediaSources
        {
            TWITTER = 0,
            /* Not Implemented
            FACEBOOK = 1,
            LINKEDIN = 2 */
        }

        public class Guids
        {
            public class Provider
            {
                public const String SOCIAL_MEDIA_PROVIDER = "62F28461-22BF-46B7-A10A-3D158FA4CAEC";
            }

            public class Application
            {
                public const String SMP_RELATIVITY_APPLICATION = "B8D1DDE9-7616-48B9-9ABB-66BED76CE540";
            }

            public class Objects
            {
                public const String SOCIAL_MEDIA_CUSTODIAN = "	8580C8A9-2D8C-43A9-A4DC-429CF8C5DD5E";
            }

            public class Fields
            {
                public class SocialMediaCustodian
                {
                    public const String NAME = "333E1CCB-FB49-438C-831F-404F0FE3055C";
                    public const String TWITTER = "63849AA5-691B-4307-9F57-332A72E9719D";
                    public const String FACEBOOK = "2DB12D2A-1AFA-43F6-B52A-8AA1D60F4048";
                    public const String LINKEDIN = "D392AE07-4829-4928-8132-239189790BF0";
                    public const String ARTIFACT_ID = "F6DC0A9C-044D-4BC0-9839-00D0EAC4D61B";
                    public const String SYSTEM_CREATED_ON = "738A6C8E-CE1A-4F26-8417-E339062376F7";
                    public const String SYSTEM_CREATED_BY= "ABED52F0-B074-4B01-A94B-33A9513DDC6B";
                    public const String SYSTEM_LAST_MODIFIED_ON = "CAFEB119-4EF6-4139-A753-2B717A47E649";
                    public const String SYSTEM_LAST_MODIFIED_BY = "4721B31D-DB3E-456F-8F01-E8212AAB448D";
                }
            }
        }
    }
}
