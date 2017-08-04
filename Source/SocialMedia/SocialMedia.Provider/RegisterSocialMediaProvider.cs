using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.IntegrationPoints.SourceProviderInstaller;

namespace SocialMedia.Provider
{
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

            sourceProviders.Add(Helpers.Constants.Guids.Provider.SOCIAL_MEDIA_PROVIDER, socialMediaProvider);

            return sourceProviders;
        }
    }
}
