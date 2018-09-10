using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using XWTWeb.Models;

namespace XWTWeb.Controllers
{
    [RequireHttps]
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View(new UserAccount());
        }


        [HttpPost]
        public ActionResult Index(UserAccount user)
        {
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                user.LoginFails++;
                return View(user);
            }


            user.Password = "";
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.UserName, DateTime.Now, DateTime.Now.AddDays(7), false, JsonConvert.SerializeObject(user), FormsAuthentication.FormsCookiePath);
            string hashCookies = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hashCookies)
            {
                HttpOnly = true,
                Expires = ticket.Expiration
            };

            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            //return Json(Url.Action("Main", "Player"));
            return RedirectToAction("Main", "Player");
        }

        //private async System.Threading.Tasks.Task TestEmailAsync()
        //{
        //    var apiKey = Environment.GetEnvironmentVariable("SendGridAPIKey");
        //    var client = new SendGridClient(apiKey);
        //    var from = new EmailAddress("test@example.com", "Example User");
        //    var subject = "Sending with SendGrid is Fun";
        //    var to = new EmailAddress("aaron.sayles@gmail.com", "Example User");
        //    var plainTextContent = "and easy to do anywhere, even with C#";
        //    var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        //    Response response = await client.SendEmailAsync(msg);
        //}


        [HttpPost]
        public void Logout()
        {
            // Delete the user details from cache.
            //session.Abandon();

            // Delete the authentication ticket and sign out.
            FormsAuthentication.SignOut();

            // Clear authentication cookie.
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
        }

    }
}