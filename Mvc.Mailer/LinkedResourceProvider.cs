using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;

namespace Mvc.Mailer
{
    public class LinkedResourceProvider : ILinkedResourceProvider
    {
        public virtual List<LinkedResource> GetAll(Dictionary<string, string> resources)
        {
            var linkedResources = new List<LinkedResource>();
            foreach (var resource in resources)
            {
                linkedResources.Add(Get(resource.Key, resource.Value));
            }
            return linkedResources;
        }

        public virtual LinkedResource Get(string contentId, string filePath)
        {
            LinkedResource resource = new LinkedResource(filePath, GetContentType(filePath));
            resource.ContentId = contentId;
            return resource;
        }

        public virtual ContentType GetContentType(string fileName)
        {
            string ext = System.IO.Path.GetExtension(fileName).ToLower();

            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);

            if (regKey != null && regKey.GetValue("Content Type") != null)
            {
                return new ContentType(regKey.GetValue("Content Type").ToString());
            }
            return null;
        }
    }
}
