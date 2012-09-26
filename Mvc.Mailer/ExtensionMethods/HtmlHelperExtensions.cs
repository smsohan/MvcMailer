using System.Web.Mvc;
using System.Web;

namespace Mvc.Mailer {
    public static class HtmlHelperExtensions {
        /// <summary>
        /// Produces the tag for inline image
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="contentId">e.g. logo</param>
        /// <param name="alt">e.g. Company Logo</param>
        /// <returns><img src="cid:logo" alt="Company Logo"/></returns>
        public static IHtmlString InlineImage(this HtmlHelper htmlHelper, string contentId, string alt = "") {
            return htmlHelper.Raw(string.Format("<img src=\"cid:{0}\" alt=\"{1}\"/>", contentId, alt));
        }
    }
}
