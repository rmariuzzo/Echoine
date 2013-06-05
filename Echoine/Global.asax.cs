using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Echoine
{
    public class EchoineApp : System.Web.HttpApplication
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(EchoineApp));
        private static readonly ILog FormLogger = LogManager.GetLogger("Echoine.Form");

        protected void Application_Start()
        {
            XmlConfigurator.Configure();
            Logger.Debug("Configuring logging system...");
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Logger.Info("================================");
            Logger.Info("       ~ Echoine is up ~        ");
            Logger.Info("================================");
        }

        protected void Application_BeginRequest()
        {
            if (HttpContext.Current != null
                && HttpContext.Current.Request != null
                && HttpContext.Current.Request.Url != null)
            {
                // Log request.
                var req = HttpContext.Current.Request;
                Logger.DebugFormat("> {0}: {1}", req.HttpMethod, req.Url.PathAndQuery);
                // Log request form data.
                if (FormLogger.IsDebugEnabled) {
                    var formData = new List<String>();
                    if (req.Form!=null) {
                        foreach (string f in req.Form)
                            formData.Add(f + "=" + req.Form[f]);
                    }
                    FormLogger.DebugFormat("   form: {0}", String.Join(", ", formData));
                }
            }
        }

        protected void Application_EndRequest()
        {
            if (HttpContext.Current != null
                && HttpContext.Current.Request != null
                && HttpContext.Current.Response != null
                && HttpContext.Current.Request.Url != null)
            {
                var req = HttpContext.Current.Request;
                var res = HttpContext.Current.Response;
                if (res.StatusCode == (int)HttpStatusCode.OK)
                {
                    Logger.DebugFormat("< {0}: {1} - {2}", req.HttpMethod, req.Url.PathAndQuery, res.StatusDescription);
                }
                else
                {
                    Logger.WarnFormat("< {0}: {1} - {2}", req.HttpMethod, req.Url.PathAndQuery, res.StatusDescription);
                }

                // Echo the headers.
                res.ClearHeaders();
                foreach (string kh in req.Headers)
                    res.AddHeader(kh, req.Headers[kh]);

                res.StatusCode = (int)HttpStatusCode.OK;
                res.ClearContent();
                res.Write(String.Format("{0}: {1}",req.HttpMethod, req.Url.PathAndQuery));
            }
        }
    }


}