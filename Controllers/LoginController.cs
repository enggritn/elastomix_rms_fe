using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMS_FE.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {

            HttpCookie cookie = Request.Cookies["rmi_cookie"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Server = ConfigurationManager.AppSettings["server"].ToString();
            ViewBag.BaseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            return View();
        }

        [HttpPost]
        public ActionResult SessionSetter(string token, string username, string full_name, string login_date, string rememberMe)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();

            string message = "";
            bool status = false;

            try
            {
                if(!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(login_date) && !string.IsNullOrEmpty(rememberMe))
                {
                    bool isRemember = Convert.ToBoolean(rememberMe);
                    if (isRemember)
                    {
                        HttpCookie cookie = new HttpCookie("rmi_cookie");
                        cookie.Values.Add("token", token);
                        cookie.Values.Add("login_date", login_date);
                        cookie.Values.Add("username", username);
                        cookie.Values.Add("full_name", full_name);
                        cookie.Expires = DateTime.Now.AddYears(10);
                        Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie("rmi_cookie");
                        cookie.Values.Add("token", token);
                        cookie.Values.Add("login_date", login_date);
                        cookie.Values.Add("username", username);
                        cookie.Values.Add("full_name", full_name);
                        cookie.Expires = DateTime.Now.AddHours(24);
                        Response.Cookies.Add(cookie);
                    }

                    Session["username"] = username;
                    Session["token"] = token;
                    Session["login_date"] = login_date;
                    Session["full_name"] = full_name;

                    status = true;
                    message = "Authentication succeeded.";
                }
                else
                {
                    throw new Exception("Bad parameters.");
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            obj.Add("status", status);
            obj.Add("message", message);

            return Json(obj);
        }

        [HttpGet]
        public ActionResult Exit()
        {
            Session.Clear();
            Session.Abandon();

            HttpCookie cookie = new HttpCookie("rmi_cookie");
            cookie.Values.Add("token", null);
            cookie.Values.Add("login_date", null);
            cookie.Values.Add("username", null);
            cookie.Values.Add("full_name", null);
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Login");
        }
    }
}