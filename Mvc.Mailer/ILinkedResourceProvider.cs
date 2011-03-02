using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.IO;

namespace Mvc.Mailer
{
    public interface ILinkedResourceProvider
    {
        List<LinkedResource> GetAll(Dictionary<string, string> resources);
        LinkedResource Get(string contentId, string filePath);
    }
}
