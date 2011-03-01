using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mvc.Mailer;
using System.Net.Mail;
using System.Web.Mvc;
using Moq;
using System.IO;
using System.Web.Routing;
using System.Web;

namespace Mvc.Mailer.Test
{
    [TestFixture]
    public class MailerBaseTest
    {
        private MailerBase _mailerBase;

        [SetUp]
        public void Setup()
        {
            MailerBase.IsTestModeEnabled = true;
            _mailerBase = new MailerBase();
        }

        [Test]
        public void TestMasterName([Values(null, "", "_Layout")] string masterName)
        {
            _mailerBase.MasterName = masterName;
            Assert.AreEqual(masterName, _mailerBase.MasterName);
        }

        [Test]
        public void TestIsBodyHtml([Values(true, false)] bool isBodyHtml)
        {
            _mailerBase.IsBodyHtml = isBodyHtml;
            Assert.AreEqual(isBodyHtml, _mailerBase.IsBodyHtml);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PopulateBodyWithNullMailMessage()
        {
            MailMessage mailMessage = null;
            _mailerBase.PopulateBody(mailMessage, "Welcome");
        }

        [Test]
        public void PopulateBodyWithObjectShouldPutTheViewText()
        {
            MailMessage mailMessage = new MailMessage();
            var mailer = new Mock<MailerBase>();
            mailer.CallBase = true;

            mailer.Object.IsBodyHtml = true;

            mailer.Setup(m => m.IsMultiPart("welcome", "Mail")).Returns(false);

            mailer.Setup(m => m.EmailBody("welcome", "Mail")).Returns("Hello");

            mailer.Object.PopulateBody(mailMessage, "welcome", "Mail");
            mailer.VerifyAll();
            Assert.AreEqual("Hello", mailMessage.Body);
            Assert.IsTrue(mailMessage.IsBodyHtml);
        }

        [Test]
        public void PopulateBody_should_populate_alternate_parts_if_present()
        {
            MailMessage mailMessage = new MailMessage();
            var mailer = new Mock<MailerBase>();
            mailer.CallBase = true;

            mailer.Object.IsBodyHtml = true;

            mailer.Setup(m => m.IsMultiPart("welcome", "Mail")).Returns(true);

            mailer.Setup(m => m.PopulatePart(mailMessage, "welcome.text", "text/plain", "Mail.text"));
            mailer.Setup(m => m.PopulatePart(mailMessage, "welcome", "text/html", "Mail"));

            mailer.Object.PopulateBody(mailMessage, "welcome", "Mail");
            mailer.VerifyAll();
        }

        [Test]
        public void PopulatePart_should_populate_the_specified_part()
        {
            var mailMessage = new MailMessage();
            var mailerMock = new Mock<MailerBase>();
            mailerMock.CallBase = true;

            mailerMock.Setup(m => m.EmailBody("welcome.text", "Mail.text")).Returns("text part");

            var mailPart = mailerMock.Object.PopulatePart(mailMessage, "welcome.text", "text/plain", "Mail.text");

            mailerMock.VerifyAll();
            Assert.AreEqual(1, mailMessage.AlternateViews.Count);
            Assert.AreEqual("text/plain", mailMessage.AlternateViews[0].ContentType.MediaType);
            Assert.AreEqual("text part", GetContent(mailMessage.AlternateViews[0]));
            Assert.IsNotNull(mailPart);
        }

        [Test]
        public void ViewExists_should_call_view_engines_to_to_find_view()
        {
            var engines = ViewEngines.Engines;
            var engine = new Mock<IViewEngine>();
            var viewEngineResult = new ViewEngineResult(new Mock<IView>().Object, new Mock<IViewEngine>().Object);
            try
            {
                var mailer = new MailerBase();
                var mockControllerContext = new Mock<ControllerContext>();
                var routeData = new RouteData();
                routeData.Values["controller"] = "Mail";
                mockControllerContext.Setup(m => m.RouteData).Returns(routeData);

                mailer.ControllerContext = mockControllerContext.Object;
                ViewEngines.Engines.Clear();
                ViewEngines.Engines.Add(engine.Object);
                engine.Setup(e => e.FindView(mailer.ControllerContext, "welcome", "Mail", true)).Returns(viewEngineResult);
                Assert.IsTrue(mailer.ViewExists("welcome", "Mail"));
                engine.VerifyAll();
            }
            finally
            {
                ViewEngines.Engines.Remove(engine.Object);
                ViewEngines.Engines.Union(engines);
            }
        }


        [Test, Sequential]
        public void IsMultiPart_should_check_for_both_view_exists(
            [Values(true, true, false, false)]bool isText, 
            [Values(true, false, true, false)]bool isHtml)
        {
            var mailer = new Mock<MailerBase>();
            mailer.CallBase = true;

            mailer.Setup(m => m.ViewExists("welcome.text", "Mail.text")).Returns(isText);
            mailer.Setup(m => m.ViewExists("welcome", "Mail")).Returns(isHtml);

            Assert.AreEqual(isText && isHtml, mailer.Object.IsMultiPart("welcome", "Mail"));
        }

        [Test, Sequential]
        public void IsMultiPart_should_pass_null_to_master_name([Values(null, "")] string masterName)
        {
            var mailer = new Mock<MailerBase>();
            mailer.CallBase = true;

            mailer.Setup(m => m.ViewExists("welcome.text", null)).Returns(true);
            mailer.Setup(m => m.ViewExists("welcome", null)).Returns(true);

            Assert.IsTrue(mailer.Object.IsMultiPart("welcome", masterName));
        }

        [Test]
        public void Test_PopulateLinkedResources_should_populate_each_resource()
        {
            var linkedResourceProviderMock = new Mock<ILinkedResourceProvider>();
            var mailerMock = new Mock<MailerBase>();
            mailerMock.CallBase = true;

            mailerMock.Object.LinkedResourceProvider = linkedResourceProviderMock.Object;

            
            var resources = new Dictionary<string, string>{
                                {"logo", "logo.png"},
                                {"button", "button.png"}
                            };

            var linkResources = new List<LinkedResource>();

            linkedResourceProviderMock.Setup(p => p.GetAll(resources)).Returns(linkResources);


            var htmlView = AlternateView.CreateAlternateViewFromString("");
            mailerMock.Object.PopulateLinkedResources(htmlView, resources);

            linkedResourceProviderMock.VerifyAll();
            Assert.AreEqual(linkResources, htmlView.LinkedResources);

        }

        [Test]
        public void Test_PopulateLinkedResource_should_populate_the_resource()
        {
            var linkedResourceProviderMock = new Mock<ILinkedResourceProvider>();
            var mailerMock = new Mock<MailerBase>();
            mailerMock.CallBase = true;

            mailerMock.Object.LinkedResourceProvider = linkedResourceProviderMock.Object;

            LinkedResource linkResource = new LinkedResource(new MemoryStream());

            linkedResourceProviderMock.Setup(p => p.Get("logo", "logo.png")).Returns(linkResource);

            var htmlView = AlternateView.CreateAlternateViewFromString("");
            mailerMock.Object.PopulateLinkedResource(htmlView, "logo", "logo.png");

            linkedResourceProviderMock.VerifyAll();
            Assert.AreEqual(1, htmlView.LinkedResources.Count);
            Assert.AreEqual(linkResource, htmlView.LinkedResources.First());

        }

        [Test]
        public void Test_LinkedResourceProvider()
        {
            var mailer = new MailerBase();
            var linkResourceProvider = new Mock<ILinkedResourceProvider>();

            mailer.LinkedResourceProvider = linkResourceProvider.Object;

            Assert.AreEqual(linkResourceProvider.Object, mailer.LinkedResourceProvider);
        }

 
        private string GetContent(AlternateView alternateView)
        {
            var dataStream = alternateView.ContentStream;
            byte[] byteBuffer = new byte[dataStream.Length];
            return System.Text.Encoding.ASCII.GetString(byteBuffer, 0, dataStream.Read(byteBuffer, 0, byteBuffer.Length));
        }

       

    }
}
