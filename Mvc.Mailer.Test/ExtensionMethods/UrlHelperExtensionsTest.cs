using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;

namespace Mvc.Mailer.Test
{
    [TestFixture]
    public class UrlHelperExtensionsTest
    {
        UrlHelper _urlHelper;

        [SetUp]
        public void Init()
        {
            var httpContext = HttpContext.Current;

            if (httpContext == null)
            {
                var request = new HttpRequest("/", "http://example.com:8080", "");
                var response = new HttpResponse(new StringWriter());
                httpContext = new HttpContext(request, response);
            }

            var httpContextBase = new HttpContextWrapper(httpContext);
            var routeData = new RouteData();
            var requestContext = new RequestContext(httpContextBase, routeData);

            _urlHelper = new UrlHelper(requestContext);
        }

        [Test]
        public void Abs_with_abs_should_return_itself()
        {
            Assert.AreEqual("http://hello.com/any/thing", _urlHelper.Abs("http://hello.com/any/thing"));
        }

        [Test]
        public void Abs_with_relative_should_absolutilze()
        {
            Assert.AreEqual("http://example.com:8080/any/thing", _urlHelper.Abs("/any/thing"));
        }

        [Test]
        public void Abs_with_root_should_absolutilze()
        {
            Assert.AreEqual("http://example.com:8080/", _urlHelper.Abs("/"));
        }

    }
}
