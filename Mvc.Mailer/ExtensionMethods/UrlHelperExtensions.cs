using System.Web.Mvc;
using System;
using System.Configuration;

namespace Mvc.Mailer {
    public static class UrlHelperExtensions {
        public static readonly string BASE_URL_KEY = "MvcMailer.BaseUrl";

        /// <summary>
        /// This extension method will help generating Absolute Urls in the mailer or other views
        /// </summary>
        /// <param name="urlHelper">The object that gets the extended behavior</param>
        /// <param name="relativeOrAbsoluteUrl">A relative or absolute URL to convert to Absolute</param>
        /// <returns>An absolute Url. e.g. http://domain:port/controller/action from /controller/action</returns>
        public static string Abs(this UrlHelper urlHelper, string relativeOrAbsoluteUrl) {
            var uri = new Uri(relativeOrAbsoluteUrl, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri) {
                return relativeOrAbsoluteUrl;
            }

            Uri combinedUri;
            if (Uri.TryCreate(BaseUrl(urlHelper), relativeOrAbsoluteUrl, out combinedUri)) {
                return combinedUri.AbsoluteUri;
            }

            throw new Exception(string.Format("Could not create absolute url for {0} using baseUri{0}", relativeOrAbsoluteUrl, BaseUrl(urlHelper)));
        }


        private static Uri BaseUrl(UrlHelper urlHelper) {
            var baseUrl = ConfigurationManager.AppSettings[BASE_URL_KEY];

            //No configuration given, so use the one from the context
            if (string.IsNullOrWhiteSpace(baseUrl)) {
                baseUrl = urlHelper.RequestContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            }

            return new Uri(baseUrl);
        }
    }
}