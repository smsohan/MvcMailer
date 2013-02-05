using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mvc.Mailer.PreMailerPostProcessor
{
    public class PreMailerPostProcessor : IPostProcessor
    {
        private bool RemoveStyleTag {get; set;}
        
        public PreMailerPostProcessor() : this(true) {}

        public PreMailerPostProcessor(bool removeStyleTag)
        { 
            this.RemoveStyleTag = removeStyleTag;
        }

        public string Process(string body)
        {
            var pm = new PreMailer.Net.PreMailer();
            return pm.MoveCssInline(body, this.RemoveStyleTag);
        }
    }
}
