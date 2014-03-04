using NUnit.Framework;
using System.Net.Mail;
using System.Threading;

namespace Mvc.Mailer.Test {
    [TestFixture]
    public class TestSmtpClientTest {
        TestSmtpClient _testSmtpClient;

        [SetUp]
        public void Init() {
            _testSmtpClient = new TestSmtpClient();
        }

        [Test]
        public void SentMails_should_be_not_null() {
            Assert.IsNotNull(TestSmtpClient.SentMails);
        }

        [Test]
        public void Send_should_add_to_sent_mails() {
            var messageA = new MailMessage { From = new MailAddress("hello@example.com"), Subject = "Hello", Body = "There" };
            messageA.To.Add("hi@example.com");
            var messageB = new MailMessage { From = new MailAddress("hi@example.com"), Subject = "There", Body = "Hello" };
            messageB.To.Add("hello@example.com");

            _testSmtpClient.Send(messageA);
            _testSmtpClient.Send(messageB);

            Assert.AreEqual(2, TestSmtpClient.SentMails.Count);
            Assert.AreSame(messageA, TestSmtpClient.SentMails[0]);
            Assert.AreSame(messageB, TestSmtpClient.SentMails[1]);
        }

        [Test]
        public void Send_should_set_async_to_false() {
            var messageA = new MailMessage { From = new MailAddress("hello@example.com"), Subject = "Hello", Body = "There" };
            messageA.To.Add("hi@example.com");

            _testSmtpClient.Send(messageA);

            Assert.AreEqual(1, TestSmtpClient.SentMails.Count);
            Assert.IsFalse(TestSmtpClient.WasLastCallAsync);
        }

        [Test]
        public void SendAsync_should_add_to_sent_mails() {
            var messageA = new MailMessage { From = new MailAddress("hello@example.com"), Subject = "Hello", Body = "There" };
            messageA.To.Add("hi@example.com");
            var messageB = new MailMessage { From = new MailAddress("hi@example.com"), Subject = "There", Body = "Hello" };
            messageB.To.Add("hello@example.com");

            _testSmtpClient.SendAsync(messageA);
            _testSmtpClient.SendAsync(messageB, "hi");

            Assert.AreEqual(2, TestSmtpClient.SentMails.Count);
            Assert.AreSame(messageA, TestSmtpClient.SentMails[0]);
            Assert.AreSame(messageB, TestSmtpClient.SentMails[1]);
        }

        [Test]
        public void SendAsync_from_background_thread_should_add_to_sent_mails() {
            var messageA = new MailMessage { From = new MailAddress("hello@example.com"), Subject = "Hello", Body = "There" };
            messageA.To.Add("hi@example.com");

            // Use actual thread for deterministic repro; this also works with async/await as long as
            // task scheduler performs work in separate thread.
            Thread backgroundThread = new Thread(() => _testSmtpClient.SendAsync(messageA));
            backgroundThread.Start();
            backgroundThread.Join();

            Assert.AreEqual(1, TestSmtpClient.SentMails.Count);
        }

        [Test]
        public void SendAsync_should_set_async_to_false() {
            var messageA = new MailMessage { From = new MailAddress("hello@example.com"), Subject = "Hello", Body = "There" };
            messageA.To.Add("hi@example.com");

            _testSmtpClient.SendAsync(messageA);

            Assert.AreEqual(1, TestSmtpClient.SentMails.Count);
            Assert.IsTrue(TestSmtpClient.WasLastCallAsync);
        }

        [Test]
        public void SendAsync_should_fire_call_back_if_registered() {
            var eventFired = false;
            _testSmtpClient.SendCompleted += (sender, e) => eventFired = true;
            var messageA = new MailMessage { From = new MailAddress("hello@example.com"), Subject = "Hello", Body = "There" };
            messageA.To.Add("hi@example.com");
            _testSmtpClient.SendAsync(messageA);
            Assert.IsTrue(eventFired);
        }

        [TearDown]
        public void TearDown() {
            MailerBase.IsTestModeEnabled = false;
            TestSmtpClient.SentMails.Clear();
            TestSmtpClient.WasLastCallAsync = false;
        }
    }
}