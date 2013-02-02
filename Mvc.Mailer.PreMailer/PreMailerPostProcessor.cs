using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mvc.Mailer.PreMailerPostProcessor
{
    public class PreMailerPostProcessor : IPostProcessor
    {
        public string Process(string body)
        {
            var pm = new PreMailer.Net.PreMailer();
            return pm.MoveCssInline(body, true);
        }
    }
}
