using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TelerikMvcWebMail.Filter;
using TelerikMvcWebMail.Models;

namespace TelerikMvcWebMail.Controllers
{
    [SessionExpire]
    public class HomeController : Controller
    {
        
        public ActionResult FromOtherPageRequest(string MailBoxId = null)
        {
            TempData["SelectedMailBoxId"] = MailBoxId;
            return  RedirectToAction("Index");
        } 
        public ActionResult Index()
        {
            int MailBoxid = 0;
            try
            {
                MailBoxid = Convert.ToInt32(TempData["SelectedMailBoxId"]);
            }
            catch(Exception ex)
            {                
            }
            if (MailBoxid!=0)
            {
                var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/GetFirstFolderId?UserId=" + Session["UserId"].ToString()+"&MailBoxid="+MailBoxid , RestSharp.Method.GET);
                string Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<string>(Data);
                ViewBag.FirstFolderId = Result;
            }
            else
            {
                var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/GetFirstFolderId?UserId=" + Session["UserId"].ToString(), RestSharp.Method.GET);
                string Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<string>(Data);
                ViewBag.FirstFolderId = Result;
                
            }
             
              return View();
        }

        

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return PartialView();
        }
            
     
        [ValidateInput(false)]
        public ActionResult ReadMailDetails(string MailId)
        {
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/ReadMailDetails?MailId="+ MailId, RestSharp.Method.GET);
            MailViewModel Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MailViewModel>(Data);           
            return PartialView("EmailDetailes", Result);

        }
        [HttpPost]
        public ActionResult UpdateEmailSubject(MailViewModel Model)
        {
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/UpdateEmailSubject", RestSharp.Method.POST, Model);
            bool Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<bool>(Data);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MailBoxFolders()
        {
            
            return View();
        }
        
        public ActionResult returnFolderPartalView(string FolderId)
        {
            MailBoxFolderModel Model = new MailBoxFolderModel();            
            List<SelectListItem> List = new List<SelectListItem>() {
                new SelectListItem {
                    Text="Select",
                    Value=""
                }
            };
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/MailBoxListForFolder?UserId="+ Session["UserId"], RestSharp.Method.GET);
            List<SelectListItem> Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<SelectListItem>>(Data);
            List.AddRange(Result);
            ViewBag.FolderList = List;
           
            if (!string.IsNullOrEmpty(FolderId))
            {
                var _MailBoxFolderModel = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/GetFolderDeatiles?FolderId=" + FolderId, RestSharp.Method.GET);
                Model = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<MailBoxFolderModel>(_MailBoxFolderModel);
                             
            }
            
            return View("_PartialAddEditFolder", Model);

        }


        public ActionResult AddEditFolder(MailBoxFolderModel Model)
        {
            
            Model.UserId = Session["UserId"].ToString();           
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/AddEditFolder", RestSharp.Method.POST, Model);
            bool Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<bool>(Data);
            return Json(Result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult FunctionDeleteFolder(string id)
        {            
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/DeleteFolder?Id="+id, RestSharp.Method.GET);
            bool Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<bool>(Data);            
            return Json(Result, JsonRequestBehavior.AllowGet);

        }
        
        public ActionResult getFolderlistBySelectedmailBox(long MailBoxId)
        {
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/MailBoxFolderListForFolderPage?MailBoxId=" + MailBoxId+ "&UserId="+ Session["UserId"].ToString(), RestSharp.Method.GET);
            List<MailBoxFolderModel> Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<MailBoxFolderModel>>(Data);
            return Json(Result, JsonRequestBehavior.AllowGet);            
           
        }
        public ActionResult GetNewEmail()
        {
            return PartialView();
        }

        public ActionResult SaveNewEmail(MailViewModel _MailViewModel)
        {
            if (string.IsNullOrEmpty(_MailViewModel.Url))
            {
                _MailViewModel.IsValid = false;
            }
            else
            {
                Uri uriResult;
                bool result = Uri.TryCreate(_MailViewModel.Url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (result)
                {
                    _MailViewModel.IsValid = true;
                }
                else
                {
                    _MailViewModel.IsValid = false;
                }
            }
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/SaveNewEmail", RestSharp.Method.POST, _MailViewModel);
            bool Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<bool>(Data);
            return Json(Result, JsonRequestBehavior.AllowGet);            
        }

        public ActionResult UpdateMailActiveDisable(string MessageId, string Flag)
        {
            var Data = TelerikMvcWebMail.Common.CallWebApi("api/ApiHome/UpdateMailStatus?Id="+ MessageId+ "&flag="+ Flag, RestSharp.Method.GET);
            bool Result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<bool>(Data);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }


    }
}
