using System.Web.Mvc;
using System;
using System.Web;
using System.Text;

namespace Mvc.Mailer
{
    public static class HtmlHelperExtension
    {
        /// <summary>
        /// This extension method will help generating Absolute Urls in the mailer or other views
        /// </summary>
        /// <param name="urlHelper">The object that gets the extended behavior</param>
        /// <param name="relativeOrAbsoluteUrl">A relative or absolute URL to convert to Absolute</param>
        /// <returns>An absolute Url. e.g. http://domain:port/controller/action from /controller/action</returns>

        public static IHtmlString InlineImage(this HtmlHelper htmlHelper, string contentId, string alt = "")
        {
            return htmlHelper.Raw(string.Format("<img src=\"cid:{0}\" alt=\"{1}\">", contentId, alt));
        }

    }

}
