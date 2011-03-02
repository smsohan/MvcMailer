using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using System.Web.Mvc;
using Mvc.Mailer;

namespace Mvc.Mailer.Test
{
    [TestFixture]
    public class HtmlHelperExtensionTest
    {
        [Test]
        public void InlineAttachment_should_produce_the_right_tag()
        {
            var htmlHelper = new HtmlHelper(new ViewContext(), new Mock<IViewDataContainer>().Object);
            Assert.AreEqual("<img src=\"cid:logo\" alt=\"\">", htmlHelper.InlineImage("logo"));
        }

        [Test]
        public void InlineAttachment_should_produce_the_right_tag_with_alt()
        {
            var htmlHelper = new HtmlHelper(new ViewContext(), new Mock<IViewDataContainer>().Object);
            Assert.AreEqual("<img src=\"cid:logo\" alt=\"Company Logo\">", htmlHelper.InlineImage("logo", "Company Logo"));
        }
    }
}
