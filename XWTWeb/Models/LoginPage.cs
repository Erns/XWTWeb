using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XWTWeb.Models
{
    public class LoginPage
    {
        public UserAccount LoginUser { get; set; } = new UserAccount();

        public UserAccount RegisterUser { get; set; } = new UserAccount();
    }
}