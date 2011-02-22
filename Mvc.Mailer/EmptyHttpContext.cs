using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Mvc.Mailer
{
    public class EmptyHttpContext : HttpContextBase
    {
        public override HttpRequestBase Request
        {
            get
            {
                return new HttpRequestWrapper(new HttpRequest("", "", ""));
            }
        }
    }
}
