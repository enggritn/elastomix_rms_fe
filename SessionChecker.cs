using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WMS_FE
{
    public class SessionCheck : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookie cookie = filterContext.HttpContext.Request.Cookies["rmi_cookie"];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                filterContext.Result = new RedirectToRouteResult(
                   new RouteValueDictionary {
                                { "Controller", "Login" },
                                { "Action", "Index" }
                               });
            }
            else
            {
                if (!string.IsNullOrEmpty(cookie.Values["token"]))
                {
                    string token = cookie.Values["token"].ToString();
                    filterContext.HttpContext.Session["token"] = token;
                }

                if (!string.IsNullOrEmpty(cookie.Values["login_date"]))
                {
                    string token = cookie.Values["login_date"].ToString();
                    filterContext.HttpContext.Session["login_date"] = token;
                }
            }
        }
    }
}