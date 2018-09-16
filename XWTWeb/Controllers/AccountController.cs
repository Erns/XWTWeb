using Newtonsoft.Json;
using RestSharp;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using XWTWeb.Classes;
using XWTWeb.Models;

namespace XWTWeb.Controllers
{
    [RequireHttps]
    public class AccountController : Controller
    {
        RestClient client = new RestClient(ConfigurationManager.AppSettings["XWTWebAPIAddress"].ToString());

        // GET: Account
        public ActionResult Index()
        {
            return View(new LoginPage());
        }


        [HttpPost]
        public ActionResult Index(LoginPage user, string submitCommand)
        {

            switch (submitCommand)
            {
                case "Login":

                    if (string.IsNullOrEmpty(user.LoginUser.UserName) || string.IsNullOrEmpty(user.LoginUser.Password))
                    {
                        user.LoginUser.LoginFails++;
                        break;
                    }

                    //Hash up our password here
                    user.LoginUser.Password = SHA1.Encode(user.LoginUser.Password);

                    var requestLogin = new RestRequest("UserAccount", Method.GET);
                    requestLogin.AddParameter("value", JsonConvert.SerializeObject(user.LoginUser));

                    user.LoginUser.Password = "";

                    // execute the request
                    IRestResponse responseLogin = client.Execute(requestLogin);
                    var contentLogin = responseLogin.Content;

                    //Check if we get a fail from API
                    if (contentLogin.ToUpper().Contains("GET: FALSE"))
                    {
                        user.LoginUser.LoginFails++;
                        break;
                    }

                    //Put this in its own catch just in case it returns bad info
                    try
                    {
                        UserAccount result = JsonConvert.DeserializeObject<UserAccount>(JsonConvert.DeserializeObject(contentLogin).ToString());
                        if (result.Id != 0)
                        {
                            return LoginUser(result);
                        }
                    }
                    catch
                    {
                        //Do nothing
                    }

                    user.LoginUser.LoginFails++;
                    break;
                    

                case "Register":
                    user.RegisterUser.Password = SHA1.Encode(user.RegisterUser.Password);

                    var requestRegister = new RestRequest("UserAccount", Method.POST);
                    requestRegister.AddJsonBody(JsonConvert.SerializeObject(user.RegisterUser));

                    user.RegisterUser.Password = "";

                    // execute the request
                    IRestResponse responseRegister = client.Execute(requestRegister);
                    var contentRegister = responseRegister.Content;

                    if (contentRegister.ToUpper().Contains("POST: SUCCESS"))
                    {
                        return LoginUser(user.RegisterUser);
                    }                   

                    break;
            }

            return View(user);

        }


        private ActionResult LoginUser(UserAccount user)
        {
            user.Password = "";
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.UserName, DateTime.Now, DateTime.Now.AddDays(7), false, JsonConvert.SerializeObject(user), FormsAuthentication.FormsCookiePath);
            string hashCookies = FormsAuthentication.Encrypt(ticket);
            System.Web.HttpCookie cookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, hashCookies)
            {
                HttpOnly = true,
                Expires = ticket.Expiration
            };

            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
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
            System.Web.HttpCookie cookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
        }

    }
}