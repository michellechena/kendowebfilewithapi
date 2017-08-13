using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcWebMail
{
    public static class SessionMangment
    {
        public class Users_
        {
            public static string APIHostUrl
            {
                get { return Convert.ToString(HttpContext.Current.Session["APIHostUrl"]); }
                set { HttpContext.Current.Session["APIHostUrl"] = value; }
            }
        }
    }
}
