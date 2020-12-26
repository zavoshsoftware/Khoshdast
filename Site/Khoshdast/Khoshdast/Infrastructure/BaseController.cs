using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Helpers;

namespace Khoshdast.Infrastructure
{
    public class BaseController : Controller
    {
        public BaseController()
        {
        }

        protected override System.IAsyncResult BeginExecuteCore(System.AsyncCallback callback, object state)
        {
            System.Globalization.CultureInfo oCultureInfo =
                new System.Globalization.CultureInfo("fa-IR");

            System.Threading.Thread.CurrentThread.CurrentCulture = oCultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = oCultureInfo;

            //ViewBag.Name = GetUserInfo.GetUserFullName();
        
            return base.BeginExecuteCore(callback, state);
        }
    }
}