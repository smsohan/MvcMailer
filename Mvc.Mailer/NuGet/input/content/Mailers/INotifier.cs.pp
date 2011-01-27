using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace $rootnamespace$.Mailers
{
    public interface INotifier
    {
        MailMessage WelcomeMessage();
    }
}

