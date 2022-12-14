using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMS_FE.Controllers
{
    [SessionCheck]
    public class PRBoxApprovalController : Controller
    {
        // GET: PRBoxApproval
        public ActionResult Index()
        {
            if(Session["token"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        // GET: PRBoxApproval/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
    }
}
