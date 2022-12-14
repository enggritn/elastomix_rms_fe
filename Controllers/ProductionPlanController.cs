using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMS_FE.Controllers
{
    [SessionCheck]
    public class ProductionPlanController : Controller
    {
        // GET: ProductionPlan
        public ActionResult Index()
        {
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        //public ActionResult Schedule()
        //{
        //    if (Session["token"] == null)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }
        //    ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
        //    ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();

        //    return View();
        //}

        public ActionResult Create()
        {
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }
        public ActionResult Edit()
        {
            ViewBag.Param = this.Request.QueryString["id"];
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        [HttpGet]
        public ActionResult Schedule(int year, int month, int line)
        {
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            ViewBag.Year = year;
            ViewBag.Month = month;
            ViewBag.LineNumber = line;

            return View();
        }
    }
}