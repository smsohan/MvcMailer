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
        protected virtual string EmailBody(string viewName, string masterName=null)
        {
            var result = new StringResult
            {
                ViewName = viewName,// GetViewPath(viewName),
                ViewData = ViewData,               
                MasterName = masterName ?? MasterName
            };
            ControllerContext = ControllerContext ?? CreateControllerContext();
            result.ExecuteResult(ControllerContext);
            return result.Output;
        }

        /// <summary>
        /// This method generates the EmailBody from the given viewName, masterName
        /// </summary>
        /// <param name="mailMessage">@example: new MailMessage{Subject = "Welcome!"}; If it is null, an instance will be created!</param>
        /// <param name="viewName">@example: "WelcomeMessage" </param>
        /// <param name="masterName">@example: "_MyLayout" if nothing is set, then the MasterName property will be used instead</param>
        /// <returns>the raw html content of the email view and its master page</returns>
        protected virtual string PopulateBody(MailMessage mailMessage, string viewName, string masterName = null)
        {
            mailMessage = mailMessage ?? new MailMessage();
            mailMessage.IsBodyHtml = IsBodyHtml;
            mailMessage.Body = EmailBody(viewName, masterName);
            return mailMessage.Body;
        }

        private ControllerContext CreateControllerContext()
        {
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
            routeData.Values["controller"] = MailerName;
            return new ControllerContext(new HttpContextWrapper(HttpContext.Current), routeData, this);
        }

        protected virtual string MailerName
        {
            get
            {
                return this.GetType().Name;;
            }
        }

    }

}