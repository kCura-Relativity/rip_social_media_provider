using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using kCura.IntegrationPoints.SourceProviderInstaller;

namespace SocialMedia.Provider
{
    [kCura.EventHandler.CustomAttributes.Description("Social Media provider - Installer")]
    [kCura.EventHandler.CustomAttributes.RunOnce(false)]
    [Guid("D3390C04-4537-4219-8305-FFC1DD9B7F2C")]
    public class RegisterSocialMediaProvider : kCura.IntegrationPoints.SourceProviderInstaller.IntegrationPointSourceProviderInstaller
    {
        public override IDictionary<Guid, SourceProvider> GetSourceProviders()
        {
            var sourceProviders = new Dictionary<Guid, kCura.IntegrationPoints.SourceProviderInstaller.SourceProvider>();

            var socialMediaProvider = new kCura.IntegrationPoints.SourceProviderInstaller.SourceProvider()
            {
                Name = "Social Media Provider",
                Url = string.Format("/%applicationpath%/CustomPages/{0}/Provider/Index", Helpers.Constants.Guids.Application.SMP_RELATIVITY_APPLICATION)
            };

            sourceProviders.Add(new Guid(Helpers.Constants.Guids.Provider.SOCIAL_MEDIA_PROVIDER), socialMediaProvider);

            return sourceProviders;
        }
    }
}
