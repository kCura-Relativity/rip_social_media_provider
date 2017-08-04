using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Provider
{
    [kCura.EventHandler.CustomAttributes.Description("Social Media provider - Uninstall")]
    [kCura.EventHandler.CustomAttributes.RunOnce(false)]
    public class RemoveSocialMediaProvider : kCura.IntegrationPoints.SourceProviderInstaller.IntegrationPointSourceProviderUninstaller
    {
        //The event handler fires when your integration points event handler is being removed
    }
}
