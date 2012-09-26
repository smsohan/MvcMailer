using System;
using NUnit.Framework;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mvc.Mailer.Test {
    [TestFixture]
    public class StringResultTest {
        private StringResult _stringResult;

        [SetUp]
        public void SetUp() {
            _stringResult = new StringResult();
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteResultWithNullContextShouldThrowArgumentNullException() {
            _stringResult.ExecuteResult(null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteResultWithBlankViewNameShouldThrowArgumentNullException([Values(null, "")] string viewName) {
            _stringResult.ViewName = viewName;
            _stringResult.ExecuteResult(new ControllerContext());
        }

        [Test]
        public void ShouldRenderViewIntoOutputPropertyWhenViewIsSet() {
            _stringResult.View = new MockedView();
            _stringResult.ViewName = "welcome";
            _stringResult.ExecuteResult(new ControllerContext());
            Assert.AreEqual(MockedView.CannedResponse, _stringResult.Output);
        }

        [Test]
        public void ShouldRenderViewIntoOutputProperty() {
            ViewEngines.Engines.Add(new MyViewEngine());
            _stringResult.ViewName = "welcome";
            var httpContext = new EmptyHttpContext();
            var controller = new MailerBase();
            var routeData = new RouteData();
            routeData.Values["controller"] = "test";
            var contollerContext = new ControllerContext(httpContext, routeData, controller);
            _stringResult.ExecuteResult(contollerContext);
            Assert.AreEqual(MockedView.CannedResponse, _stringResult.Output);
        }
    }
}