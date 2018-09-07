using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace XWTWeb.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Index(string i)
        {

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket("test", false, 100);
            string cookieData = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieData)
            {
                HttpOnly = true,
                Expires = ticket.Expiration
            };

            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            return Json(Url.Action("Main", "Player"));
        }


        [HttpPost]
        public void Logout()
        {
            FormsAuthentication.SignOut();
        }

    }
}