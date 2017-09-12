using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kCura.Relativity.Client.DTOs;
using Relativity.API;

namespace SocialMedia.Helpers.Interfaces
{
	public interface ISocialMediaCustodianService
	{
		Task<RDO> GetSocialMediaCustodianAsync(IServicesMgr mgr, Int32 workspaceArtifactID, Int32 socialMediaCustodianArtifactID);
		Task<IEnumerable<RDO>> GetAllSocialMediaCustodiansAsync(IServicesMgr mgr, Int32 workspaceArtifactID);
	}
}