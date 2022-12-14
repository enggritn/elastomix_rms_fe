using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMS_FE.Controllers
{
    [SessionCheck]
    public class HomeController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            //commented by Muhammad Bhovdair 13 Dec 2020

            //String token = this.Request.QueryString["t"];
            //Session["token"] = token;
            //String username = this.Request.QueryString["u"];
            //Session["username"] = username;
            //String rememberMe = this.Request.QueryString["r"];
            //if (rememberMe == "true")
            //{
            //    HttpContext.Session.Timeout = 525600;
            //}
            //else
            //{
            //    HttpContext.Session.Timeout = 60;
            //}


            return View();
        }

        
    }
}