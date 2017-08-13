using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TelerikMvcWebMail.Models;

namespace TelerikMvcWebMail.Controllers
{
    public class SignInController : Controller
    {
        TelerikMvcWebMail.DataLayer.UsersDataLayer _UsersDataLayer = new DataLayer.UsersDataLayer();
        //
        // GET: /SignIn/
        public ActionResult Index(string ReturnUrl)
        {
            
           
            Session["APIHostUrl"] = System.Configuration.ConfigurationManager.AppSettings["APIHostUrl"];
            if (ReturnUrl=="Sessionexpire")
            {
                ViewBag.LoginError = "Session Expire";
            }
            SignIn Model = new Models.SignIn();
            return View("SignIn", Model);
        }

        [HttpPost]
        public ActionResult SignIn(SignIn Model)
        {
            if (!ModelState.IsValid)
            {
                return View("SignIn", Model);
            }
            else
            {
                User _User = _UsersDataLayer.ValidateUser(Model.UserName, Model.Password);
                if(_User ==null)
                {
                    ViewBag.LoginError = "User Name and Password Incurrect";
                    return View("SignIn", Model);                    
                }
                else
                {
                    Session["FullName"] = _User.FullName;
                    Session["UserEmail"] = _User.Email;
                    Session["UserId"] = _User.UserId;
                    return RedirectToAction("index", "Home");
                }
                
            }
        }
    }
}