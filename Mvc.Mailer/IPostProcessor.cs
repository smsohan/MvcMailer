using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;


namespace Mvc.Mailer
{
    public interface IPostProcessor
    {
        string Process(string body);
    }
}