using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Mvc.Mailer;
using NUnit.Framework;
using System.Web.Routing;
using Moq;
using System.Web;

namespace Mvc.Mailer.Test
{
    [TestFixture]
    public class AbsoluteUrlHelperTest
    {
        public const string AppPathModifier = "/$(SESSION)";

        [SetUp]
        public void Init()
        {
        }


        [Test]
        public void UrlRootTest()
        {
            Assert.AreEqual("http://localhost:8080/", GetMailerUrlHelper().UrlRoot);
        }


        [Test]
        public void AbsoluteAction_with_only_action_should_work()
        {
            Assert.AreEqual("http://localhost:8080/" + AppPathModifier + "/app/home/Action", GetMailerUrlHelper().AbsoluteAction("Action"));
        }

        [Test]
        public void AbsoluteAction_with_action_and_controller_should_work()
        {
            Assert.AreEqual("http://localhost:8080/" + AppPathModifier + "/app/Controller/Action", GetMailerUrlHelper().AbsoluteAction("Action", "Controller"));
        }

        [Test]
        public void AbsoluteAction_with_route_value_should_work()
        {
            var routeValues = new RouteValueDictionary();
            routeValues["p"] = "q";
            Assert.AreEqual("http://localhost:8080/" + AppPathModifier + "/app/home/Action?p=q", GetMailerUrlHelper().AbsoluteAction("Action", routeValues));
        }

        [Test]
        public void AbsoluteAction_with_object_route_value_should_work()
        {
            var routeValues = new { p = "q" };
            Assert.AreEqual("http://localhost:8080/" + AppPathModifier + "/app/home/Action?p=q", GetMailerUrlHelper().AbsoluteAction("Action", routeValues));
        }

        [Test]
        public void AbsoluteAction_with_controller_and_object_route_value_should_work()
        {
            var routeValues = new { p = "q" };
            Assert.AreEqual("http://localhost:8080/" + AppPathModifier + "/app/Controller/Action?p=q", GetMailerUrlHelper().AbsoluteAction("Action", "Controller", routeValues));
        }

        [Test]
        public void AbsoluteAction_with_controller_and_route_value_should_work()
        {
            var routeValues = new RouteValueDictionary();
            routeValues["p"] = "q";
            Assert.AreEqual("http://localhost:8080/" + AppPathModifier + "/app/Controller/Action?p=q", GetMailerUrlHelper().AbsoluteAction("Action", "Controller", routeValues));
        }


        private static RequestContext GetRequestContext()
        {
            HttpContextBase httpcontext = GetHttpContext("/app/", null, null);
            RouteData rd = new RouteData();
            return new RequestContext(httpcontext, rd);
        }

        private static AbsoluteUrlHelper GetMailerUrlHelper()
        {
            return new AbsoluteUrlHelper(GetUrlHelper());
        }

#region Copied from ASP.Net MVC

        private static UrlHelper GetUrlHelper()
        {
            HttpContextBase httpcontext = GetHttpContext("/app/", null, null, "http", 8080);
            RouteCollection rt = new RouteCollection();
            rt.Add(new Route("{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            rt.Add("namedroute", new Route("named/{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            RouteData rd = new RouteData();
            rd.Values.Add("controller", "home");
            rd.Values.Add("action", "oldaction");

            UrlHelper urlHelper = new UrlHelper(new RequestContext(httpcontext, rd), rt);
            return urlHelper;
        }


        public static HttpContextBase GetHttpContext(string appPath, string requestPath, string httpMethod)
        {
            return GetHttpContext(appPath, requestPath, httpMethod, Uri.UriSchemeHttp.ToString(), -1);
        }

        public static HttpContextBase GetHttpContext(string appPath, string requestPath, string httpMethod, string protocol, int port)
        {
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();

            if (!String.IsNullOrEmpty(appPath))
            {
                mockHttpContext.Setup(o => o.Request.ApplicationPath).Returns(appPath);
            }
            if (!String.IsNullOrEmpty(requestPath))
            {
                mockHttpContext.Setup(o => o.Request.AppRelativeCurrentExecutionFilePath).Returns(requestPath);
            }

            Uri uri;

            if (port >= 0)
            {
                uri = new Uri(protocol + "://localhost" + ":" + Convert.ToString(port));
            }
            else
            {
                uri = new Uri(protocol + "://localhost");
            }
            mockHttpContext.Setup(o => o.Request.Url).Returns(uri);

            mockHttpContext.Setup(o => o.Request.PathInfo).Returns(String.Empty);
            if (!String.IsNullOrEmpty(httpMethod))
            {
                mockHttpContext.Setup(o => o.Request.HttpMethod).Returns(httpMethod);
            }

            mockHttpContext.Setup(o => o.Session).Returns((HttpSessionStateBase)null);
            mockHttpContext.Setup(o => o.Response.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(r => AppPathModifier + r);
            return mockHttpContext.Object;
        }
#endregion
    }
}
