using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using kCura.Relativity.Client;
using Relativity.API;

namespace SocialMedia.Web.Controllers
{
    public class ProviderController : Controller
    {
        // GET: Provider
        public ActionResult Index()
        {
            var logger = Relativity.CustomPages.ConnectionHelper.Helper().GetLoggerFactory().GetLogger();
            try
            {
                using (var rsapiClient = Relativity.CustomPages.ConnectionHelper.Helper().GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
                {
                    var randomRDOArtifactID = 123123;
                    rsapiClient.APIOptions = new APIOptions()
                    {
                        WorkspaceID = Relativity.CustomPages.ConnectionHelper.Helper().GetActiveCaseID()
                    };
                    var result = rsapiClient.Repositories.RDO.Read(randomRDOArtifactID);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured while loaded the custom page");
            }
            
                return View();
        }
    }
}