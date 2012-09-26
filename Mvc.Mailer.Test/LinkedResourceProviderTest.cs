using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Moq;
using System.Net.Mail;
using System.IO;
using System.Net.Mime;

namespace Mvc.Mailer.Test {
    [TestFixture]
    public class LinkedResourceProviderTest {
        [Test]
        public void Test_GetAll_should_call_get_and_add_to_list() {
            var linkedResourceProvider = new Mock<LinkedResourceProvider> { CallBase = true };

            var logo = new LinkedResource(new MemoryStream());
            var banner = new LinkedResource(new MemoryStream());

            linkedResourceProvider.Setup(p => p.Get("logo", "logo.png")).Returns(logo);
            linkedResourceProvider.Setup(p => p.Get("banner", "banner.png")).Returns(banner);

            var resourcesMap = new Dictionary<string, string>(){
                {"logo", "logo.png"},
                {"banner", "banner.png"}
            };
            var resources = linkedResourceProvider.Object.GetAll(resourcesMap);

            linkedResourceProvider.VerifyAll();
            Assert.AreEqual(logo, resources.First());
            Assert.AreEqual(banner, resources.Last());
        }

        [Test]
        public void Test_Get_should_return_a_linked_resource() {
            var linkedResourceProvider = new Mock<LinkedResourceProvider> { CallBase = true };

            const string fileName = "Chrysanthemum.jpg";
            var linkedResource = linkedResourceProvider.Object.Get("flower", fileName);

            Assert.AreEqual("flower", linkedResource.ContentId);
            Assert.AreEqual(new ContentType("image/jpeg"), linkedResource.ContentType);
        }
    }
}