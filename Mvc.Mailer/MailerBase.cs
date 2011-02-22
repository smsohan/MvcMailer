using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Web.Mvc.Html;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web.Routing;
using System.Net.Mime;
using System.Diagnostics;

namespace Mvc.Mailer
{
    /// <summary>
    /// The Base class for Mailers. Your mailer should subclass:
    /// 
    /// public clas MyMailer : Mvc.Mailer.MailerBase{
    ///     public MailMessage WelcomeMessage(User newUser){
    ///         MailMessage mailMessage = new MailMessage{Subject = "Welcome to TheBestEverSite.com"};
    ///         mailMessage.To.Add(newUser.EmailAddress);
    ///         mailMessage.Body = EmailBody("WelcomeMessage");
    ///         return mailMessage;
    ///     }
    /// }
    /// 
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

        /// <summary>
        /// The Path to the MasterPage or Layout
        /// myMailer.MasterName = "_MyLayout.cshtml"
        /// </summary>
        public string MasterName
        {
            get;
            set;
        }

        public bool IsBodyHtml
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


        public virtual void PopulatePart(MailMessage mailMessage, string viewName, string mime, string masterName = null)
        {
            var part = EmailBody(viewName, masterName ?? MasterName);
            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(part, new ContentType(mime)));
        }

        /// <summary>
        /// This method generates the EmailBody from the given viewName, masterName
        /// </summary>
        /// <param name="mailMessage">@example: new MailMessage{Subject = "Welcome!"}; If it is null, it will throw an exception</param>
        /// <param name="viewName">@example: "WelcomeMessage" </param>
        /// <param name="masterName">@example: "_MyLayout" if nothing is set, then the MasterName property will be used instead</param>
        public virtual void PopulateBody(MailMessage mailMessage, string viewName, string masterName = null)
        {
            masterName = masterName ?? MasterName;
            if (mailMessage == null)
            {
                throw new ArgumentNullException("mailMessage", "mailMessage cannot be null");
            }
            if(IsMultiPart(viewName, masterName))
            {
                var textMasterName = string.IsNullOrEmpty(masterName) ? null : masterName + ".text";
                PopulatePart(mailMessage, viewName + ".text", "text/plain", textMasterName);
                PopulatePart(mailMessage, viewName, "text/html", masterName);
            }
            else{
                mailMessage.Body = EmailBody(viewName, masterName);
                mailMessage.IsBodyHtml = IsBodyHtml;
            }
        }

        private ControllerContext CreateControllerContext()
        {
            var routeData = RouteTable.Routes.GetRouteData(CurrentHttpContext);
            ControllerContext = new ControllerContext(CurrentHttpContext, routeData, this);
            return ControllerContext;
        }

        protected virtual string MailerName
        {
            get
            {
                return this.GetType().Name;;
            }
        }


        public HttpContextBase CurrentHttpContext
        {
            get;
            set;
        }


        public static bool IsTestModeEnabled
        {
            get;
            set;
        }

        public virtual bool IsMultiPart(string viewName, string masterName)
        {
            return ViewExists(viewName, masterName) && ViewExists(viewName + ".text", masterName + ".text");
        }

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