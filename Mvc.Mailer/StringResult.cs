using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Web.UI;

namespace Mvc.Mailer
{
    /// <summary>
    /// Mimics the ViewResult with an important difference - the view is only storted in the Output property insted of written to a
    /// browser stream!
    /// </summary>
    public class StringResult : ViewResult
    {

        public StringResult()
        {

        }

        public StringResult(string viewName)
        {
            ViewName = viewName;
        }

        /// <summary>
        /// Contains the output after compiling the view and putting in the values
        /// </summary>
        public string Output
        {
            get;
            private set;
        }

    	public HttpContext CurrentHttpContext { get; set; }

        public virtual void ExecuteResult(ControllerContext context, string mailerName)
        {
            //remember the controller name
            var controllerName = context.RouteData.Values["controller"];
            
            //temporarily change it to the mailer name so that FindView works
            context.RouteData.Values["controller"] = mailerName;
            try
            {
                ExecuteResult(context);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally{
                //restore the controller name
                context.RouteData.Values["controller"] = controllerName;
            }


        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (String.IsNullOrEmpty(ViewName))
            {
                throw new ArgumentNullException("ViewName of StringResult cannot be null or empty");
            }

            ViewEngineResult result = null;

            if (View == null)
            {
                result = FindView(context);
                View = result.View;
            }

            StringBuilder stringBuilder = new StringBuilder();
            TextWriter writer = new StringWriter(stringBuilder);

        	if (HttpContext.Current != null)
			{
				ViewContext viewContext = new ViewContext(context, View, ViewData, TempData, writer);
        		View.Render(viewContext, writer);
				Output = stringBuilder.ToString();
			}
        	else
				Output = ViewToString(((BuildManagerCompiledView) View).ViewPath, ViewData);
        }

		public string ViewToString(string viewPath, ViewDataDictionary viewData)
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				using (HtmlTextWriter htmlTextWriter = new HtmlTextWriter(stringWriter))
				{
					var workerRequest = new SimpleWorkerRequest(viewPath, "", htmlTextWriter);
					HttpContext.Current = CurrentHttpContext ?? new HttpContext(workerRequest);

					object view = BuildManager.CreateInstanceFromVirtualPath(viewPath, typeof(object));

					ViewPage viewPage = view as ViewPage;
					if (viewPage != null)
					{
						viewPage.ViewData = viewData;
					}
					else
					{
						ViewUserControl viewUserControl = view as ViewUserControl;
						if (viewUserControl != null)
						{
							viewPage = new ViewPage();
							viewPage.Controls.Add(viewUserControl);
						}
					}

					if (viewPage != null)
					{
						viewPage.Url = new UrlHelper(HttpContext.Current.Request.RequestContext);
						HttpContext.Current.Server.Execute(viewPage, htmlTextWriter, true);

						return stringBuilder.ToString();
					}

					throw new InvalidOperationException();
				}
			}
		}
    }
}