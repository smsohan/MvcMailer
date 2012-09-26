using NUnit.Framework;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using System.Configuration;

namespace Mvc.Mailer.Test.ExtensionMethods {
    [TestFixture]
    public class UrlHelperExtensionsTest {
        UrlHelper _urlHelper;

        [SetUp]
        public void Init() {
            var httpContext = HttpContext.Current;

            if (httpContext == null) {
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
        public void Abs_with_abs_should_return_itself() {
            Assert.AreEqual("http://hello.com/any/thing", _urlHelper.Abs("http://hello.com/any/thing"));
        }

        [Test]
        public void Abs_with_relative_should_absolutilze() {
            Assert.AreEqual("http://example.com:8080/any/thing", _urlHelper.Abs("/any/thing"));
        }

        [Test]
        public void Abs_with_root_should_absolutilze() {
            Assert.AreEqual("http://example.com:8080/", _urlHelper.Abs("/"));
        }

        [Test]
        public void Abs_with_encoded_params_should_keep_encoding() {
            Assert.AreEqual("http://example.com:8080/?param=encoded%20value", _urlHelper.Abs("/?param=encoded%20value"));
        }

        [Test]
        public void Abs_with_config_should_use_the_base_url_from_the_config() {
            ConfigurationManager.AppSettings[UrlHelperExtensions.BASE_URL_KEY] = "http://my:666";
            Assert.AreEqual("http://my:666/hello/there", _urlHelper.Abs("/hello/there"));
        }

        [Test]
        public void Abs_with_config_should_not_double_slash_when_using_the_base_url_from_the_config() {
            ConfigurationManager.AppSettings[UrlHelperExtensions.BASE_URL_KEY] = "http://my:666/";
            Assert.AreEqual("http://my:666/hello/there", _urlHelper.Abs("/hello/there"));
        }

        [Test]
        public void Abs_with_config_should_put_a_slash_when_using_the_base_url_from_the_config() {
            ConfigurationManager.AppSettings[UrlHelperExtensions.BASE_URL_KEY] = "http://my:666";
            Assert.AreEqual("http://my:666/hello/there", _urlHelper.Abs("hello/there"));
        }

        [TearDown]
        public void TearDown() {
            ConfigurationManager.AppSettings[UrlHelperExtensions.BASE_URL_KEY] = string.Empty;
        }
    }
}