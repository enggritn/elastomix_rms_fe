using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS_FE.Models;

namespace WMS_FE.Controllers
{
    [SessionCheck]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Title = "User";
            ViewBag.Module = "User";
            ViewBag.Method = "Index";
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }


        public ActionResult Create()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Title = "User";
            ViewBag.Module = "User";
            ViewBag.Method = "Create";
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }

        public ActionResult Edit()
        {
            if (Session["token"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.Title = "User";
            ViewBag.Module = "User";
            ViewBag.Method = "Edit";
            ViewBag.Param = this.Request.QueryString["username"];
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            return View();
        }
    }
}