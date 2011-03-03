using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mvc.Mailer
{
    /// <summary>
    /// The Base class for Mailers. Your mailer should subclass MailerBase:
    /// </summary>
    public class MailerBase : ControllerBase
    {
        /// <summary>
        /// The parameterless constructor
        /// </summary>
        public MailerBase()
        {
            if (HttpContext.Current != null)
            {
                CurrentHttpContext = new HttpContextWrapper(HttpContext.Current);
            }
            else if (IsTestModeEnabled)
            {
                CurrentHttpContext = new EmptyHttpContext();
            }
            IsTestModeEnabled = false;
        }

        private ILinkedResourceProvider _LinkedResourceProvider = new LinkedResourceProvider();
        /// <summary>
        /// Uses the ILinkedResourceProvider to produce inline linked resources
        /// </summary>
        public ILinkedResourceProvider LinkedResourceProvider
        {
            get { return _LinkedResourceProvider;  }
            set { _LinkedResourceProvider = value; }
        }

        /// <summary>
        /// The Path to the MasterPage or Layout
        /// e.g. Razor: myMailer.MasterName = "_MyLayout.cshtml"
        /// e.g. Aspx: myMailer.MasterName = "_MyLayout.Master"
        /// </summary>
        public string MasterName
        {
            get;
            set;
        }

        /// <summary>
        /// Nothing to Execute at this point, left blank
        /// </summary>
        protected override void ExecuteCore()
        {
           
        }

        /// <summary>
        /// This method generates the EmailBody from the given viewName, masterName
        /// </summary>
        /// <param name="viewName">@example: "WelcomeMessage" </param>
        /// <param name="masterName">@example: "_MyLayout.cshtml" if nothing is set, then the MasterName property will be used instead</param>
        /// <returns>the raw html content of the email view and its master page</returns>
        public virtual string EmailBody(string viewName, string masterName=null)
        {

            masterName = masterName ?? MasterName;
         
            var result = new StringResult
            {
                ViewName = viewName,
                ViewData = ViewData,               
                MasterName = masterName ?? MasterName
            };
            if(ControllerContext == null)
                CreateControllerContext();
            result.ExecuteResult(ControllerContext, MailerName);
            return result.Output;
        }

        /// <summary>
        /// Populates the mailMessage with content rendered from the view using the default masterName
        /// </summary>
        /// <param name="mailMessage">a non null System.Net.Mail.MailMessage reference</param>
        /// <param name="viewName">The name of the view file, e.g. WelcomeMessage </param>
        /// <param name="linkedResources">Key: linked resource id or CID, Value:Path to the resource</param>
        public virtual void PopulateBody(MailMessage mailMessage, string viewName, Dictionary<string, string> linkedResources)
        {
            PopulateBody(mailMessage, viewName, null, linkedResources);
        }

        /// <summary>
        /// Populates the mailMessage with content rendered from the view using the provided masterName
        /// </summary>
        /// <param name="mailMessage">a non null System.Net.Mail.MailMessage reference</param>
        /// <param name="viewName">The name of the view file, e.g. WelcomeMessage </param>
        /// <param name="masterName">The name of the master file, e.g. Layout </param>
        /// <param name="linkedResources">Key: linked resource id or CID, Value:Path to the resource</param>
        public virtual void PopulateBody(MailMessage mailMessage, string viewName, string masterName = null, Dictionary<string, string> linkedResources = null)
        {
            if (mailMessage == null)
            {
                throw new ArgumentNullException("mailMessage", "mailMessage cannot be null");
            }

            masterName = masterName ?? MasterName;
            PopulateTextPart(mailMessage, viewName, masterName);         
            PopulateHtmlPart(mailMessage, viewName, masterName, linkedResources);
        }
        
        /// <summary>
        /// Populates a text/plain AlternateView inside the mailMessage
        /// </summary>
        public virtual AlternateView PopulateTextPart(MailMessage mailMessage, string viewName, string masterName)
        {
            var textMasterName = string.IsNullOrEmpty(masterName) ? null : masterName + ".text";
            var textViewName = viewName + ".text";
            return PopulatePart(mailMessage, textViewName, "text/plain", textMasterName);
        }

        /// <summary>
        /// Populates a text/html AlternateView inside the mailMessage
        /// </summary>
        public virtual AlternateView PopulateHtmlPart(MailMessage mailMessage, string viewName, string masterName, Dictionary<string, string> linkedResources)
        {
            var htmlPart = PopulatePart(mailMessage, viewName, "text/html", masterName) ?? PopulatePart(mailMessage, viewName + ".html", "text/html", masterName + ".html");
            if (htmlPart != null)
            {
                PopulateLinkedResources(htmlPart, linkedResources);
            }
            return htmlPart;
        }

        /// <summary>
        /// Populates an AlternateView inside the mailMessage
        /// </summary>
        /// <param name="mime">e.g. text/plain, text/html etc.</param>
        public virtual AlternateView PopulatePart(MailMessage mailMessage, string viewName, string mime, string masterName = null)
        {
            masterName = masterName ?? MasterName;
            if (ViewExists(viewName, masterName))
            {
                var part = EmailBody(viewName, masterName);
                var alternateView = AlternateView.CreateAlternateViewFromString(part, new ContentType(mime));
                mailMessage.AlternateViews.Add(alternateView);
                return alternateView;
            }
            return null;
        }

        /// <summary>
        /// Adds LinkedResources to the mailPart
        /// </summary>
        public virtual List<LinkedResource> PopulateLinkedResources(AlternateView mailPart, Dictionary<string, string> resources)
        {
            if(resources == null || resources.Count == 0)    
                return new List<LinkedResource>();

            var linkedResources = LinkedResourceProvider.GetAll(resources);
            linkedResources.ForEach(resource => mailPart.LinkedResources.Add(resource));
            return linkedResources;
        }

        /// <summary>
        /// Adds a single LinkedResource to a mailPart
        /// </summary>
        public virtual LinkedResource PopulateLinkedResource(AlternateView mailPart, string contentId, string fileName)
        {
            var linkedResource = LinkedResourceProvider.Get(contentId, fileName);
            mailPart.LinkedResources.Add(linkedResource);
            return linkedResource;
        }


        private ControllerContext CreateControllerContext()
        {
            var routeData = RouteTable.Routes.GetRouteData(CurrentHttpContext);
            ControllerContext = new ControllerContext(CurrentHttpContext, routeData, this);
            return ControllerContext;
        }

        /// <summary>
        /// The MailerName determines the folder that contains the views for this mailer
        /// </summary>
        protected virtual string MailerName
        {
            get
            {
                return this.GetType().Name;;
            }
        }


        public virtual HttpContextBase CurrentHttpContext
        {
            get;
            set;
        }

        /// <summary>
        /// If set to true, it will use TestSmtpClient instead of SmtpClient. Used solely for testing purpose
        /// </summary>
        public static bool IsTestModeEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Determines if a View exists given its name and masterName
        /// </summary>
        public virtual bool ViewExists(string viewName, string masterName)
        {
            if (ControllerContext == null)
                CreateControllerContext();

            var controllerName = this.ControllerContext.RouteData.Values["controller"];
            this.ControllerContext.RouteData.Values["controller"] = MailerName;

            try
            {
                return ViewEngines.Engines.FindView(this.ControllerContext, viewName, masterName).View != null;
            }
            finally
            {
                this.ControllerContext.RouteData.Values["controller"] = controllerName;
            }
        }
    }

}