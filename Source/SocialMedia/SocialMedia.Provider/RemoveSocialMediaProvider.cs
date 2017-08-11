using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Provider
{
    [kCura.EventHandler.CustomAttributes.Description("Social Media provider - Uninstall")]
    [kCura.EventHandler.CustomAttributes.RunOnce(false)]
    [Guid("5A5421EF-4DD8-46F0-969B-CD37B43836DC")]
    public class RemoveSocialMediaProvider : kCura.IntegrationPoints.SourceProviderInstaller.IntegrationPointSourceProviderUninstaller
    {
        //The event handler fires when your integration points event handler is being removed
    }
}
