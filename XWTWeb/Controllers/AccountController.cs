﻿using Newtonsoft.Json;
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
                        //user.LoginUser.LoginFails++;
                        break;
                    }

                    user.LoginUser.Password = SHA1.Encode(user.LoginUser.Password);

                    var requestLogin = new RestRequest("UserAccount", Method.GET);
                    requestLogin.AddJsonBody(JsonConvert.SerializeObject(user.LoginUser));

                    //user.LoginUser.Password = "";

                    // execute the request
                    IRestResponse responseLogin = client.Execute(requestLogin);
                    var contentLogin = responseLogin.Content;

                    if (contentLogin == "GET: TRUE")
                    {
                        return LoginUser(user.LoginUser);
                    }

                    break;
                    

                case "Register":

                    user.RegisterUser.Password = SHA1.Encode(user.RegisterUser.Password);

                    var requestRegister = new RestRequest("UserAccount", Method.POST);
                    //request.AddUrlSegment("id", 0);
                    requestRegister.AddJsonBody(JsonConvert.SerializeObject(user.RegisterUser));

                    user.RegisterUser.Password = "";

                    // execute the request
                    IRestResponse responseRegister = client.Execute(requestRegister);
                    var contentRegister = responseRegister.Content;

                    if (contentRegister == "POST: Success")
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

        public void RegisterUserAccount(string loginInfo)
        {
            var result = JsonConvert.DeserializeObject(loginInfo);

            try
            {

                UserAccount user = new UserAccount();


                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                //var request = new RestRequest("UserAccount/{id}", Method.POST);
                //request.AddUrlSegment("id", 0);
                //request.AddJsonBody(JsonConvert.SerializeObject(result));

                //// execute the request
                //IRestResponse response = client.Execute(request);
                //var content = response.Content; // raw content as string

            }
            catch (Exception ex)
            {
                Console.Write(string.Format("AccountController.RegisterUserAccount{0}Error:{1}", Environment.NewLine, ex.Message));
            }


            return;
        }

    }
}