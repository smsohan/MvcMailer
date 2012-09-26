using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;

namespace Mvc.Mailer {
    /// <summary>
    /// This class is a utility class for instantiating LinkedResource objects
    /// </summary>
    public class LinkedResourceProvider : ILinkedResourceProvider {
        public virtual List<LinkedResource> GetAll(Dictionary<string, string> resources) {
            return resources
                .Select(resource => Get(resource.Key, resource.Value))
                .ToList();
        }

        public virtual LinkedResource Get(string contentId, string filePath) {
            return new LinkedResource(filePath, GetContentType(filePath)) { ContentId = contentId };
        }

        public virtual ContentType GetContentType(string fileName) {
            var ext = System.IO.Path.GetExtension(fileName).ToLower();

            var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null) {
                return new ContentType(regKey.GetValue("Content Type").ToString());
            }
            return null;
        }
    }
}