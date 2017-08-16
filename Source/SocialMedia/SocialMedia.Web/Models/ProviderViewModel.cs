using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SocialMedia.Web.Models
{
    public class ProviderViewModel
    {
        public Int32 WorkspaceArtifactID { get; set; }
        public Guid JobIdentifier { get; set; }
        public IEnumerable<SelectListItem> Custodians { get; set; }
        public IEnumerable<SelectListItem> SocialMediaSources { get; set; }

    }
}