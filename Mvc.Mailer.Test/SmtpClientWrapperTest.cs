using NUnit.Framework;
using System.Net.Mail;

namespace Mvc.Mailer.Test {
    [TestFixture]
    public class SmtpClientWrapperTest {
        [Test]
        public void SmtpClientWrapper_should_be_implicitly_created_from_smtp_client() {
            SmtpClientWrapper smtpClientWrapper = new SmtpClient();
        }

        [Test]
        public void Empty_constructor_should_initiaize_InnerSmtpClient() {
            Assert.IsNotNull(new SmtpClientWrapper().InnerSmtpClient);
        }


        [Test]
        public void Constructor_with_SmtpClient_should_initiaize_InnerSmtpClient() {
            var client = new SmtpClient();
            Assert.AreSame(client, new SmtpClientWrapper(client).InnerSmtpClient);
        }

        [Test]
        public void Test_InnerSmtpClient() {
            var wrapper = new SmtpClientWrapper();
            var client = new SmtpClient();
            wrapper.InnerSmtpClient = client;
            Assert.AreSame(client, wrapper.InnerSmtpClient);
        }
    }
}