using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMS_FE.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult RawMaterial()
        {
            return View();
        }

        public ActionResult RawMaterial2()
        {
            return View();
        }

        public ActionResult FinishGood()
        {
            return View();
        }

        public ActionResult RackWIP()
        {
            return View();
        }


    }
}