using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Helpers
{
    public class Constants
    {
        public const Int32 DEFAULT_NUMBER_OF_POSTS_TO_DOWNLOAD = 50;

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
                public static Guid SMP_RELATIVITY_APPLICATION = new Guid("B8D1DDE9-7616-48B9-9ABB-66BED76CE540");
            }

            public class Objects
            {

                public static Guid SOCIAL_MEDIA_CUSTODIAN = new Guid("8580C8A9-2D8C-43A9-A4DC-429CF8C5DD5E");
                public static Guid SOCIAL_MEDIA_FEED = new Guid("1C805797-A5D7-40EA-A784-D3BCAD810E16");
            }

            public class Fields
            {
                public class SocialMediaCustodian
                {
                    public static Guid NAME = new Guid("333E1CCB-FB49-438C-831F-404F0FE3055C");
                    public static Guid TWITTER = new Guid("63849AA5-691B-4307-9F57-332A72E9719D");
                    public static Guid FACEBOOK = new Guid("2DB12D2A-1AFA-43F6-B52A-8AA1D60F4048");
                    public static Guid LINKEDIN = new Guid("D392AE07-4829-4928-8132-239189790BF0");
                    public static Guid ARTIFACT_ID = new Guid("F6DC0A9C-044D-4BC0-9839-00D0EAC4D61B");
                    public static Guid SYSTEM_CREATED_ON = new Guid("738A6C8E-CE1A-4F26-8417-E339062376F7");
                    public static Guid SYSTEM_CREATED_BY= new Guid("ABED52F0-B074-4B01-A94B-33A9513DDC6B");
                    public static Guid SYSTEM_LAST_MODIFIED_ON = new Guid("CAFEB119-4EF6-4139-A753-2B717A47E649");
                    public static Guid SYSTEM_LAST_MODIFIED_BY = new Guid("4721B31D-DB3E-456F-8F01-E8212AAB448D");
                }

                public class SocialMediaFeed
                {
                    public static Guid NAME = new Guid("93E0C36E-37F4-478D-B8B8-E0CC81B53DE8");
                    public static Guid JOB_IDENTIFIER = new Guid("55B8BE6C-F739-476B-AA09-67CC51CE1C9B");
                    public static Guid FEED = new Guid("F82576F4-28E5-4CC5-9A8A-95C3D7C8ED16");
                    public static Guid ARTIFACT_ID = new Guid("5C7262C5-7093-423C-A280-4214ABA425B0");
                    public static Guid SINCE_ID = new Guid("F0C8694A-387F-4789-8BF3-54C4A85E340A");
                    public static Guid SYSTEM_CREATED_ON = new Guid("6389D37C-2B6C-4788-AC24-03628DFF87BA");
                    public static Guid SYSTEM_CREATED_BY = new Guid("ABED52F0-B074-4B01-A94B-33A9513DDC6B");
                    public static Guid SYSTEM_LAST_MODIFIED_ON = new Guid("1E01863B-CF20-4268-AA80-4E69F23F398B");
                    public static Guid SYSTEM_LAST_MODIFIED_BY = new Guid("FC8010BC-AE0A-489C-837B-8D1A9499196C");
                }
            }
        }
    }
}
