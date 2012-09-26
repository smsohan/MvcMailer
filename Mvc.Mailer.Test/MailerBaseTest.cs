using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Net.Mail;
using System.Web.Mvc;
using Moq;
using System.IO;
using System.Web.Routing;

namespace Mvc.Mailer.Test {
    [TestFixture]
    public class MailerBaseTest {
        private MailerBase _mailerBase;
        private Mock<MailerBase> _mockMailer;
        private MailMessage _mailMessage;

        [SetUp]
        public void Setup() {
            MailerBase.IsTestModeEnabled = true;
            _mailerBase = new MailerBase();
            _mailMessage = new MailMessage();

            _mockMailer = new Mock<MailerBase> { CallBase = true };
        }

        #region Properties Related tests

        [Test]
        public void TestMasterName([Values(null, "", "_Layout")] string masterName) {
            _mailerBase.MasterName = masterName;
            Assert.AreEqual(masterName, _mailerBase.MasterName);
        }

        [Test]
        public void Test_LinkedResourceProvider() {
            var mailer = new MailerBase();
            var linkResourceProvider = new Mock<ILinkedResourceProvider>();

            mailer.LinkedResourceProvider = linkResourceProvider.Object;

            Assert.AreEqual(linkResourceProvider.Object, mailer.LinkedResourceProvider);
        }

        [Test]
        public void Test_IsTestModeEnabled() {
            MailerBase.IsTestModeEnabled = true;
            Assert.IsTrue(MailerBase.IsTestModeEnabled);
            MailerBase.IsTestModeEnabled = false;
            Assert.IsFalse(MailerBase.IsTestModeEnabled);
        }

        #endregion

        #region Text related tests

        [Test]
        public void PopulateTextBody_should_unmark_as_is_body_html() {
            _mockMailer.Setup(m => m.EmailBody("Welcome.text", "Layout.text")).Returns("Hello");

            _mockMailer.Object.PopulateTextBody(_mailMessage, "Welcome", "Layout");
            _mockMailer.VerifyAll();

            Assert.AreEqual("Hello", _mailMessage.Body);
            Assert.IsFalse(_mailMessage.IsBodyHtml);
        }

        [Test]
        public void TextViewExists_should_call_view_exists_with_text_names() {
            _mockMailer.Setup(m => m.ViewExists("Welcome.text", "Layout.text")).Returns(true);
            Assert.IsTrue(_mockMailer.Object.TextViewExists("Welcome", "Layout"));
            _mockMailer.VerifyAll();
        }

        [Test]
        public void TextViewName_should_append_dot_text() {
            Assert.AreEqual("Welcome.text", _mailerBase.TextViewName("Welcome"));
        }

        [Test]
        public void TextMasterName_should_append_dot_text() {
            Assert.AreEqual("Welcome.text", _mailerBase.TextMasterName("Welcome"));
        }

        [Test, Sequential]
        public void TextMasterName_should_return_nil_when_not_set([Values(null, "")] string masterName) {
            Assert.AreEqual(null, _mailerBase.TextMasterName(masterName));
        }

        [Test]
        public void PopulateTextPart_should_use_right_view_name_and_mime() {
            _mockMailer.Setup(m => m.PopulatePart(_mailMessage, "Welcome.text", "text/plain", "Mail.text"));
            _mockMailer.Object.PopulateTextPart(_mailMessage, "Welcome", "Mail");

            _mockMailer.VerifyAll();
        }

        #endregion

        #region Html related tests

        [Test]
        public void PopulateHtmltBody_should_mark_as_is_body_html() {
            _mockMailer.Setup(m => m.EmailBody("Welcome", "Layout")).Returns("<h1>Hello</h1>");

            _mockMailer.Object.PopulateHtmlBody(_mailMessage, "Welcome", "Layout");
            _mockMailer.VerifyAll();

            Assert.AreEqual("<h1>Hello</h1>", _mailMessage.Body);
            Assert.IsTrue(_mailMessage.IsBodyHtml);
        }

        [Test]
        public void HtmlViewExists_should_call_view_exists() {
            _mockMailer.Setup(m => m.ViewExists("Welcome", "Layout")).Returns(true);
            Assert.IsTrue(_mockMailer.Object.HtmlViewExists("Welcome", "Layout"));
            _mockMailer.VerifyAll();
        }

        [Test]
        public void PopulateHtmlPart_should_use_right_view_name_and_mime() {
            var resources = new Dictionary<string, string>();

            _mockMailer.Setup(m => m.PopulatePart(_mailMessage, "Welcome", "text/html", "Mail")).Returns(AlternateView.CreateAlternateViewFromString(""));
            _mockMailer.Setup(m => m.PopulateLinkedResources(It.IsAny<AlternateView>(), resources));

            _mockMailer.Object.PopulateHtmlPart(_mailMessage, "Welcome", "Mail", resources);
            _mockMailer.VerifyAll();
        }

        #endregion

        #region Multi-part related tests

        [Test]
        public void Populate_should_create_a_mail_message_and_invoke_action() {
            var linkedResources = new Dictionary<string, string>();
            _mockMailer.Setup(x => x.PopulateBody(It.IsAny<MailMessage>(), "welcome", "master", linkedResources));
            var mailMessage = _mockMailer.Object.Populate(x => {
                x.Subject = "expected";
                x.ViewName = "welcome";
                x.MasterName = "master";
                x.LinkedResources = linkedResources;
            });
            Assert.That(mailMessage.Subject, Is.EqualTo("expected"));
            _mockMailer.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PopulateBody_should_throw_exception_if_mailMessage_is_null() {
            MailMessage mailMessage = null;
            _mailerBase.PopulateBody(mailMessage, "Welcome");
        }

        [Test]
        public void PopulateBody_should_populate_html_alternate_view_when_both_parts_present() {
            _mockMailer.Setup(m => m.TextViewExists("welcome", "Mail")).Returns(true);
            _mockMailer.Setup(m => m.HtmlViewExists("welcome", "Mail")).Returns(true);


            _mockMailer.Setup(m => m.PopulateTextBody(_mailMessage, "welcome", "Mail"));
            _mockMailer.Setup(m => m.PopulateHtmlPart(_mailMessage, "welcome", "Mail", null));

            _mockMailer.Object.PopulateBody(_mailMessage, "welcome", "Mail");
            _mockMailer.VerifyAll();
        }

        [Test]
        public void PopulateBody_should_populate_body_with_text_when_only_text_present() {
            _mockMailer.Setup(m => m.HtmlViewExists("welcome", "Mail")).Returns(false);
            _mockMailer.Setup(m => m.TextViewExists("welcome", "Mail")).Returns(true);

            _mockMailer.Setup(m => m.PopulateTextBody(_mailMessage, "welcome", "Mail"));

            _mockMailer.Object.PopulateBody(_mailMessage, "welcome", "Mail");
            _mockMailer.VerifyAll();
        }

        [Test]
        public void PopulateBody_should_populate_body_with_html_when_only_html_present() {
            var resourcesToTry = new List<Dictionary<string, string>>();
            resourcesToTry.Add(null);
            resourcesToTry.Add(new Dictionary<string, string>());

            foreach (var resources in resourcesToTry) {
                _mockMailer.Setup(m => m.TextViewExists("welcome", "Mail")).Returns(false);
                _mockMailer.Setup(m => m.HtmlViewExists("welcome", "Mail")).Returns(true);
                _mockMailer.Setup(m => m.PopulateHtmlBody(_mailMessage, "welcome", "Mail"));

                _mockMailer.Object.PopulateBody(_mailMessage, "welcome", "Mail", resources);
                _mockMailer.VerifyAll();
            }
        }

        [Test]
        public void PopuateBody_should_populate_with_alternate_view_when_html_present_with_linked_resources() {
            _mockMailer.Setup(m => m.TextViewExists("welcome", "Mail")).Returns(false);
            _mockMailer.Setup(m => m.HtmlViewExists("welcome", "Mail")).Returns(true);

            var resources = new Dictionary<string, string> { { "logo", "logo.png" } };
            _mockMailer.Setup(m => m.PopulateHtmlPart(_mailMessage, "welcome", "Mail", resources));

            _mockMailer.Object.PopulateBody(_mailMessage, "welcome", "Mail", resources);
            _mockMailer.VerifyAll();
        }

        [Test, Combinatorial]
        public void IsMultiPart_should_check_html_and_text_exists([Values(true, false)] bool textExists, [Values(true, false)] bool htmlExists) {
            _mockMailer.Setup(m => m.TextViewExists("Welcome", "Layout")).Returns(textExists).Verifiable();
            _mockMailer.Setup(m => m.HtmlViewExists("Welcome", "Layout")).Returns(htmlExists);

            Assert.AreEqual(textExists && htmlExists, _mockMailer.Object.IsMultiPart("Welcome", "Layout"));
        }

        #endregion

        #region Utility related tests
        [Test]
        public void PopulatePart_should_populate_the_specified_part() {
            _mockMailer.Setup(m => m.ViewExists("welcome.text", "Mail.text")).Returns(true);
            _mockMailer.Setup(m => m.EmailBody("welcome.text", "Mail.text")).Returns("text part");

            var mailPart = _mockMailer.Object.PopulatePart(_mailMessage, "welcome.text", "text/plain", "Mail.text");

            _mockMailer.VerifyAll();
            Assert.AreEqual(1, _mailMessage.AlternateViews.Count);
            Assert.AreEqual("text/plain", _mailMessage.AlternateViews[0].ContentType.MediaType);
            Assert.AreEqual("text part", GetContent(_mailMessage.AlternateViews[0]));
            Assert.IsNotNull(mailPart);
        }

        [Test]
        public void ViewExists_should_call_view_engines_to_to_find_view() {
            var engines = ViewEngines.Engines;
            var engine = new Mock<IViewEngine>();
            var viewEngineResult = new ViewEngineResult(new Mock<IView>().Object, new Mock<IViewEngine>().Object);
            try {
                var mailer = _mailerBase;
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
            } finally {
                ViewEngines.Engines.Remove(engine.Object);
                ViewEngines.Engines.Union(engines);
            }
        }

        [Test]
        public void Test_PopulateLinkedResources_should_populate_each_resource() {
            var linkedResourceProviderMock = new Mock<ILinkedResourceProvider>();

            _mockMailer.Object.LinkedResourceProvider = linkedResourceProviderMock.Object;


            var resources = new Dictionary<string, string>{
                                {"logo", "logo.png"},
                                {"button", "button.png"}
                            };

            var linkResources = new List<LinkedResource>();

            linkedResourceProviderMock.Setup(p => p.GetAll(resources)).Returns(linkResources);


            var htmlView = AlternateView.CreateAlternateViewFromString("");
            var actualResources = _mockMailer.Object.PopulateLinkedResources(htmlView, resources);

            linkedResourceProviderMock.VerifyAll();
            Assert.AreEqual(linkResources, htmlView.LinkedResources);
            Assert.AreEqual(linkResources, actualResources);
        }

        [Test]
        public void Test_PopulateLinkedResource_should_populate_the_resource() {
            var linkedResourceProviderMock = new Mock<ILinkedResourceProvider>();

            _mockMailer.Object.LinkedResourceProvider = linkedResourceProviderMock.Object;

            var linkResource = new LinkedResource(new MemoryStream());

            linkedResourceProviderMock.Setup(p => p.Get("logo", "logo.png")).Returns(linkResource);

            var htmlView = AlternateView.CreateAlternateViewFromString("");
            var actualResource = _mockMailer.Object.PopulateLinkedResource(htmlView, "logo", "logo.png");

            linkedResourceProviderMock.VerifyAll();
            Assert.AreEqual(1, htmlView.LinkedResources.Count);
            Assert.AreEqual(linkResource, htmlView.LinkedResources.First());
            Assert.AreEqual(linkResource, actualResource);
        }
        #endregion

        private string GetContent(AlternateView alternateView) {
            var dataStream = alternateView.ContentStream;
            var byteBuffer = new byte[dataStream.Length];
            return System.Text.Encoding.ASCII.GetString(byteBuffer, 0, dataStream.Read(byteBuffer, 0, byteBuffer.Length));
        }
    }
}