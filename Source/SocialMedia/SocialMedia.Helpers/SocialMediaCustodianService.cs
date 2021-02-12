using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using SocialMedia.Helpers.Interfaces;

namespace SocialMedia.Helpers
{
	public class SocialMediaCustodianService : ISocialMediaCustodianService
	{
		public async Task<RDO> GetSocialMediaCustodianAsync(IServicesMgr mgr, Int32 workspaceArtifactID, Int32 socialMediaCustodianArtifactID)
		{
			return await Task.Run(() =>
			{
				using (var proxy = mgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					proxy.APIOptions = new APIOptions() { WorkspaceID = workspaceArtifactID };
					return proxy.Repositories.RDO.ReadSingle(socialMediaCustodianArtifactID);
				}
			});
		}

		public async Task<IEnumerable<RDO>> GetAllSocialMediaCustodiansAsync(IServicesMgr mgr, Int32 workspaceArtifactID)
		{

			var retVal = new List<RDO>();
			var query = new Query<RDO>
			{
				ArtifactTypeGuid = Constants.Guids.Objects.SOCIAL_MEDIA_CUSTODIAN,
				Fields = new List<FieldValue>
				{
					new FieldValue(Constants.Guids.Fields.SocialMediaCustodian.NAME),
					new FieldValue(Constants.Guids.Fields.SocialMediaCustodian.TWITTER),
					new FieldValue(Constants.Guids.Fields.SocialMediaCustodian.FACEBOOK),
					new FieldValue(Constants.Guids.Fields.SocialMediaCustodian.LINKEDIN),
				}
			};

			await Task.Run(() =>
			{
				using (var proxy = mgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					proxy.APIOptions = new APIOptions() { WorkspaceID = workspaceArtifactID };
					var queryResult = proxy.Repositories.RDO.Query(query);
					if (queryResult.Success && queryResult.Results.Any())
					{
						foreach (var result in queryResult.Results)
						{
							retVal.Add(result.Artifact);
						}
					}
				}
			});

			return retVal;
		}
	}
}
