using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mvc.Mailer;
using System.Net.Mail;
using System.IO;
using System.Threading;

namespace Mvc.Mailer.Test
{
    [TestFixture]
    public class MailMessageExtensionsTest
    {

        private SmtpClientWrapper _smtpClient;
        private MailMessage _mailMessage; 
        private DirectoryInfo _mailDirectory;

        [SetUp]
        public void SetUp()
        {
            var smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;

            _mailDirectory = Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Mails"));
            smtpClient.PickupDirectoryLocation = _mailDirectory.FullName;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 597;
            _smtpClient = new SmtpClientWrapper{InnerSmtpClient = smtpClient};
            _mailMessage = new MailMessage { From = new MailAddress("gaga@gaga.com") };
            _mailMessage.To.Add("gigi@gigi.com");
            _mailMessage.Subject = "Hello!";
            _mailMessage.Body = "Mail Body";
        }

        [Test]
        public void TestSend()
        {
            _mailMessage.Send(_smtpClient);
            Assert.Pass("Mail Send working since no exception wast thrown");
        }

        [Test]
        public void TestSendAsync()
        {
            _mailMessage.SendAsync(_smtpClient);
            Assert.Pass("Mail Send Async working since no exception wast thrown");

        }

        [Test]
        public void In_Test_Mode_should_use_TestSmtpClient()
        {
            TestSmtpClient.SentMails.Clear();
            MailerBase.IsTestModeEnabled = true;
            _mailMessage.Send();
            Assert.AreEqual(1, TestSmtpClient.SentMails.Count);
            Assert.AreSame(_mailMessage, TestSmtpClient.SentMails[0]);
        }

        [TearDown]
        public void TearDown()
        {
            MailerBase.IsTestModeEnabled = false;
            TestSmtpClient.SentMails.Clear();
            _mailDirectory.Delete(true);
        }



    }
}
