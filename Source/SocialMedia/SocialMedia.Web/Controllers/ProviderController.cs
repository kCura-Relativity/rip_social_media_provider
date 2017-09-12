﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SocialMedia.Helpers;
using SocialMedia.Helpers.Interfaces;
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
    }
}