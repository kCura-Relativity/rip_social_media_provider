using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Helpers.Models
{
    public class JobConfiguration
    {
        public String SocialMediaType { get; set; }
        public Int32 NumberOfPostsToReveive { get; set; }
        public Int32 JobArtifactID { get; set; }
        public Int32 SocialMediaCustodianArtifactID { get; set; }
        public Int32 WorkspaceArtifactID { get; set; }
    }
}
