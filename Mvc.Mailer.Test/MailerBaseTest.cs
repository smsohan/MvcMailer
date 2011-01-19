using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mvc.Mailer;
using System.Net.Mail;
using System.Web.Mvc;

namespace Mvc.Mailer.Test
{
    [TestFixture]
    public class MailerBaseTest
    {
        private MailerBase _mailerBase;

        [SetUp]
        public void Setup()
        {
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void PopulateBodyWithShouldPutTheViewText()
        {
            MailMessage mailMessage = null;
            ViewEngines.Engines.Add(new MyViewEngine());
            _mailerBase.CurrentHttpContext = new EmptyHttpContext();
            _mailerBase.PopulateBody(mailMessage, "Welcome");
            Assert.AreEqual(MockedView.CannedResponse, mailMessage.Body);
        }

    }
}
