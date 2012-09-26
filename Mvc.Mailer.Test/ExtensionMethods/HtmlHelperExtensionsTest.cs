using NUnit.Framework;
using Moq;
using System.Web.Mvc;

namespace Mvc.Mailer.Test.ExtensionMethods {
    [TestFixture]
    public class HtmlHelperExtensionsTest {
        [Test]
        public void InlineAttachment_should_produce_the_right_tag() {
            var htmlHelper = new HtmlHelper(new ViewContext(), new Mock<IViewDataContainer>().Object);
            Assert.AreEqual("<img src=\"cid:logo\" alt=\"\"/>", htmlHelper.InlineImage("logo").ToString());
        }

        [Test]
        public void InlineAttachment_should_produce_the_right_tag_with_alt() {
            var htmlHelper = new HtmlHelper(new ViewContext(), new Mock<IViewDataContainer>().Object);
            Assert.AreEqual("<img src=\"cid:logo\" alt=\"Company Logo\"/>", htmlHelper.InlineImage("logo", "Company Logo").ToString());
        }
    }
}
