﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;


namespace SocialMedia.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
	        WebApiConfig.Register(GlobalConfiguration.Configuration);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
