﻿using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using XWTWeb.Models;

namespace XWTWeb.Classes
{
    public class Utilities
    {

        public static UserAccount CurrentUser
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    // The user is authenticated. Return the user from the forms auth ticket.
                    return ((MyPrincipal)(System.Web.HttpContext.Current.User)).User;
                }
                else if (HttpContext.Current.Items.Contains("User"))
                {
                    // The user is not authenticated, but has successfully logged in.
                    return (UserAccount)HttpContext.Current.Items["User"];
                }
                else
                {
                    return null;
                }
            }
        }

        public static RestClient InitializeRestClient()
        {
            RestClient client = new RestClient(ConfigurationManager.AppSettings["XWTWebAPIAddress"].ToString())
            {
                Authenticator = new HttpBasicAuthenticator(CurrentUser.UserName, CurrentUser.APIPassword)
            };

            return client;
        }
    }
   
    

}