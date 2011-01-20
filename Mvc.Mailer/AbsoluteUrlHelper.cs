using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mvc.Mailer
{
    /// <summary>
    /// This class adds AbsoluteAction methods for the missing ones in System.Web.Mvc.UrlHelper
    /// So, in a view, one can use the following:
    /// 
    /// @AbsoluteUrlHelper.AbsoluteAction("About", "Home")
    /// 
    /// To generate the following URL:
    ///
    /// http://localhost:8080/About/Home
    /// 
    /// This is particularly useful for sending out emails. Since the URLs in Email needs to be absolute to be useful. 
    /// </summary>

    public class AbsoluteUrlHelper : UrlHelper
    {
    
        public string UrlRoot
        {
            get
            {
                return RequestContext.HttpContext.Request.Url.AbsoluteUri;
            }
        }


        public AbsoluteUrlHelper(RequestContext requestContext):
            this(requestContext, RouteTable.Routes)
        {


        }

        public AbsoluteUrlHelper(RequestContext requestContext, RouteCollection routeCollection):
            base(requestContext, routeCollection)
        {

        }

        public AbsoluteUrlHelper(UrlHelper urlHelper)
            :this(urlHelper.RequestContext, urlHelper.RouteCollection)
        {

        }

        public virtual string AbsoluteAction(string actionName)
        {
            return PrependProtocolAndHost(base.Action(actionName));
        }

        public virtual string AbsoluteAction(string actionName, string controller)
        {
            return PrependProtocolAndHost(base.Action(actionName, controller));
        }

        public string AbsoluteAction(string actionName, RouteValueDictionary routeValues)
        {
            return PrependProtocolAndHost(base.Action(actionName, routeValues));
        }

        public string AbsoluteAction(string actionName, object routeValues)
        {
            return PrependProtocolAndHost(base.Action(actionName, routeValues));
        }

        public string AbsoluteAction(string actionName, string controllerName, object routeValues)
        {
            return PrependProtocolAndHost(base.Action(actionName, controllerName, routeValues));
        }

        public string AbsoluteAction(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return PrependProtocolAndHost(base.Action(actionName, controllerName, routeValues));
        }

        protected virtual string PrependProtocolAndHost(string relativeUrl)
        {

            if (new Uri(relativeUrl, UriKind.RelativeOrAbsolute).IsAbsoluteUri)
            {
                return relativeUrl;
            }
            return string.Format("{0}{1}", UrlRoot, relativeUrl);
        }
    }
}
