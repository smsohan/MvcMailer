using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mvc.Mailer;
using System.Net.Mail;
using System.IO;

namespace Mvc.Mailer.Test
{
    [TestFixture]
    public class MailMessageExtensionsTest
    {

        private SmtpClient _smtpClient;
        private MailMessage _mailMessage; 
        private DirectoryInfo _mailDirectory;

        [SetUp]
        public void SetUp()
        {
            _smtpClient = new SmtpClient();
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;

            _mailDirectory = Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Mails"));
            _smtpClient.PickupDirectoryLocation = _mailDirectory.FullName;
            _mailMessage = new MailMessage { From = new MailAddress("gaga@gaga.com") };
            _mailMessage.To.Add("gigi@gigi.com");
            _mailMessage.Subject = "Hello!";
            _mailMessage.Body = "Mail Body";
        }

        [Test]
        public void TestSend()
        {
            _mailMessage.Send(_smtpClient);            
        }

        [Test]
        public void TestSendAsync()
        {
            _mailMessage.Send(_smtpClient);
        }

        [TearDown]
        public void TearDown()
        {
            _mailDirectory.Delete(true);
        }

    }
}
