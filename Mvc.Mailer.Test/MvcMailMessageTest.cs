using System;
using NUnit.Framework;
using System.Net.Mail;
using System.IO;
using Moq;

namespace Mvc.Mailer.Test {
    [TestFixture]
    public class MvcMailMessageTest {

        private SmtpClientWrapper _smtpClient;
        private MvcMailMessage _mailMessage;
        private DirectoryInfo _mailDirectory;

        [SetUp]
        public void SetUp() {
            var smtpClient = new SmtpClient {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory
            };

            _mailDirectory = Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Mails"));
            smtpClient.PickupDirectoryLocation = _mailDirectory.FullName;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 597;
            _smtpClient = new SmtpClientWrapper { InnerSmtpClient = smtpClient };
            _mailMessage = new MvcMailMessage { From = new MailAddress("gaga@gaga.com") };
            _mailMessage.To.Add("gigi@gigi.com");
            _mailMessage.Subject = "Hello!";
            _mailMessage.Body = "Mail Body";
        }

        [Test]
        public void TestSend() {
            _mailMessage.Send(_smtpClient);
            Assert.Pass("Mail Send working since no exception wast thrown");
        }

        [Test]
        public async void SendAsync_with_userState_should_pass_that() {
            var client = new Mock<ISmtpClient>();
            client.Setup(c => c.SendAsync(_mailMessage, "something"));
            await _mailMessage.SendAsync(userState: "something", smtpClient: client.Object);
            client.VerifyAll();
        }

        [Test]
        public void In_Test_Mode_should_use_TestSmtpClient() {
            TestSmtpClient.SentMails.Clear();
            MailerBase.IsTestModeEnabled = true;
            _mailMessage.Send();
            Assert.AreEqual(1, TestSmtpClient.SentMails.Count);
            Assert.AreSame(_mailMessage, TestSmtpClient.SentMails[0]);
        }

        [TearDown]
        public void TearDown() {
            MailerBase.IsTestModeEnabled = false;
            TestSmtpClient.SentMails.Clear();
            _mailDirectory.Delete(true);
        }
    }
}