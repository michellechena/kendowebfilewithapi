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
        private MailsService mailsService;

        public HomeController()
        {

            mailsService = new MailsService(new WebMailEntities());

        }
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
                var FirstFolderId = mailsService.GetFirstFolderId(Session["UserId"].ToString(), MailBoxid);
                ViewBag.FirstFolderId = FirstFolderId;
            }
            else
            {
                var FirstFolderId = mailsService.GetFirstFolderId(Session["UserId"].ToString());
                ViewBag.FirstFolderId = FirstFolderId;
            }
             
              return View();
        }

        public ActionResult NewMail(string id, string mailTo, string subject)
        {
            var idString = "";

            if (!String.IsNullOrEmpty(id))
            {
                idString = id + ": ";
            }
            ViewBag.MailTo = mailTo;
            ViewBag.Subject = idString + HttpUtility.UrlDecode(subject);

            return PartialView("NewMail");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return PartialView();
        }
                

        [ValidateInput(false)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, MailViewModel mail)
        {
            if (mail.Category == "Disable")
            {
                bool Result = mailsService.UpdateEmailSubject(mail, "Disable");
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else if(mail.Category=="Deleted")
            {
                bool Result = mailsService.UpdateEmailSubject(mail, "Deleted");
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else
            {                

                    mailsService.Update(mail);
               
            }

            return Json(new[] { mail }.ToDataSourceResult(request, ModelState));
        }
       
        

        [ValidateInput(false)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, MailViewModel mail)
        {
            
            if (mail != null && ModelState.IsValid)
            {
                mailsService.Create(mail);
            }

            return Json(new[] { mail }.ToDataSourceResult(request, ModelState));
        }

     
        [ValidateInput(false)]
        public ActionResult ReadMailDetails(string MailId)
        {
            MailViewModel Model = new MailViewModel();
            Model = mailsService.ReadMailDetails(MailId);
            return PartialView("EmailDetailes", Model);

        }
        [HttpPost]
        public ActionResult UpdateEmailSubject(MailViewModel Model)
        {
            bool Result = mailsService.UpdateEmailSubject(Model);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MailBoxFolders()
        {
            
            return View();
        }
        public ActionResult ReadFolderList([DataSourceRequest] DataSourceRequest request)
        {
            TelerikMvcWebMail.DataLayer.CommonFunctions Obj = new DataLayer.CommonFunctions();
            List<MailBoxFolderModel> Model = Obj.AllUserFolderList( Session["UserId"].ToString());
            return Json(Model.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            
        }
        public ActionResult returnFolderPartalView(string FolderId)
        {
            MailBoxFolderModel Model = new MailBoxFolderModel();
            TelerikMvcWebMail.DataLayer.CommonFunctions Obj1 = new DataLayer.CommonFunctions();
            List<SelectListItem> List = new List<SelectListItem>() {
                new SelectListItem {
                    Text="Select",
                    Value=""
                }
            };
            List.AddRange(Obj1.MailBoxList(Convert.ToInt32(Session["UserId"])));
            ViewBag.FolderList = List;
           
            if (!string.IsNullOrEmpty(FolderId))
            {
                TelerikMvcWebMail.DataLayer.CommonFunctions Obj = new DataLayer.CommonFunctions();
                 Model = Obj.GetFolderDeatiles(Convert.ToInt32(FolderId));                
            }
            
            return View("_PartialAddEditFolder", Model);

        }
        public ActionResult AddEditFolder(MailBoxFolderModel Model)
        {
            TelerikMvcWebMail.DataLayer.CommonFunctions Obj = new DataLayer.CommonFunctions();
            Model.UserId = Session["UserId"].ToString();
            bool Result=Obj.AddEditFolder(Model);            
            return Json(Result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult FunctionDeleteFolder(string id)
        {
            TelerikMvcWebMail.DataLayer.CommonFunctions Obj = new DataLayer.CommonFunctions();           
            bool Result = Obj.FunctionDeleteFolder(id);
            return Json(Result, JsonRequestBehavior.AllowGet);

        }
        
        public ActionResult getFolderlistBySelectedmailBox(long MailBoxId)
        {
            TelerikMvcWebMail.DataLayer.CommonFunctions Obj = new DataLayer.CommonFunctions();
            List<MailBoxFolderModel> Model = Obj.MailBoxFolderList(Convert.ToInt32(MailBoxId), Session["UserId"].ToString());
            return Json(Model,JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNewEmail()
        {
            return PartialView();
        }

        public ActionResult SaveNewEmail(MailViewModel _MailViewModel)
        {            
           
            TelerikMvcWebMail.DataLayer.CommonFunctions Obj = new DataLayer.CommonFunctions();
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

            bool Result= Obj.SaveNewEmail(_MailViewModel);
            
            return Json(Result,JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateMailActiveDisable(string MessageId, string Flag)
        {
            TelerikMvcWebMail.DataLayer.CommonFunctions Obj = new DataLayer.CommonFunctions();
            bool Result = Obj.UpdateMailStatus(MessageId, Flag);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }


    }
}
