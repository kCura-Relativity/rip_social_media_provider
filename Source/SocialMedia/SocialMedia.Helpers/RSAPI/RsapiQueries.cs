using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.API;

namespace SocialMedia.Helpers.RSAPI
{
    public class RsapiQueries
    {
        public RDO GetSocialMediaCustodian(IServicesMgr mgr, Int32 workspaceArtifactID, Int32 custodianArtifactID)
        {
            using (var proxy = mgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
            {
                proxy.APIOptions = new APIOptions() {WorkspaceID = workspaceArtifactID};
                return proxy.Repositories.RDO.ReadSingle(custodianArtifactID);
            }
        }
    }
}
