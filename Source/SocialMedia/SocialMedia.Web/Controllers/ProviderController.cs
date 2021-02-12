using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialMedia.Helpers;
using SocialMedia.Helpers.Interfaces;
using SocialMedia.Helpers.Models;
using SocialMedia.Web.Models;

namespace SocialMedia.Web.Controllers
{
    public class ProviderController : Controller
    {
		        
        private ISocialMediaCustodianService SocialMediaCudCustodianService { get; } = new SocialMediaCustodianService();

        public async Task<ActionResult> Index()
        {
            var helper = Relativity.CustomPages.ConnectionHelper.Helper();
            var socialMediaSources = Enum.GetNames(typeof(Helpers.Constants.SocialMediaSources))
                .Select(x => new SelectListItem { Text = x, Value = x }).ToList();

            var custodians = (await SocialMediaCudCustodianService.GetAllSocialMediaCustodiansAsync(helper.GetServicesManager(), helper.GetActiveCaseID()))
                .Select(
                    x => new SelectListItem()
                    {
                        Text = x[Helpers.Constants.Guids.Fields.SocialMediaCustodian.NAME].ValueAsFixedLengthText,
                        Value = x.ArtifactID.ToString()
                    });

            var model = new ProviderViewModel
            {
                WorkspaceArtifactID = helper.GetActiveCaseID(),
                JobIdentifier = Guid.NewGuid(),
                Custodians = custodians,
                SocialMediaSources = socialMediaSources
            };

            return View(model);
        }

        [System.Web.Http.HttpPost]
        public ActionResult GetViewFields()
        {
            var logger = Relativity.CustomPages.ConnectionHelper.Helper().GetLoggerFactory().GetLogger();
            var settings = new List<KeyValuePair<string, string>>();
            try
            {
                if (Request.InputStream != null && Request.InputStream.Length > 0)
                {
                    var data = new StreamReader(Request.InputStream).ReadToEnd();
                    var token = JToken.Parse(data);
                    var jobConfiguration = JsonConvert.DeserializeObject<JobConfiguration>(token.ToString());
                    settings.Add(new KeyValuePair<string, string>("Social media type", jobConfiguration.SocialMediaType));
                    settings.Add(new KeyValuePair<string, string>("Number of posts to retrieve", jobConfiguration.NumberOfPostsToRetrieve.ToString()));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to parse JSON Input");
            }

            return Json(settings);
        }

    }
}