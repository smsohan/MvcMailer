using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Web.Mvc;
using Mvc.Mailer;

namespace $rootnamespace$
{
	public class AbsoluteUrlHelper :  Mvc.Mailer.AbsoluteUrlHelper
	{
		public AbsoluteUrlHelper():
			base(HttpContext.HttpContext.Current.Request.RequestContext)
		{
		}
	}
}
