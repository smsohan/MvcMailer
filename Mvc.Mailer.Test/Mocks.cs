using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mvc.Mailer;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;

namespace Mvc.Mailer.Test
{
    public class MockedView : IView
    {

        public static readonly string CannedResponse = "Hello World!";

        public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            writer.Write(MockedView.CannedResponse);
        }
    }

    public class EmptyHttpContext : HttpContextBase
    {

    }

    public class MyViewEngine : IViewEngine
    {

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            throw new NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return new ViewEngineResult(new MockedView(), this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            throw new NotImplementedException();
        }
    }
}