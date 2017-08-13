using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Web.Http.ModelBinding;

using System.Web.Http.Cors;
using APIs.Entity;
using APIs.Models;

namespace APIs.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ApiHomeController : ApiController
    {
        public IHttpActionResult GetEmailList([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request, string MailBoxId, string UserId, string AjaxRequest = null)
        {
            long _UserId = Convert.ToInt32(UserId);
            long _MailBoxId = Convert.ToInt32(MailBoxId);
            using (var Entity = new WebMailEntities())
            {
                List<MailApiModel> result = new List<MailApiModel>();
                result = Entity.Mails.Where(x => x.MailBoxFolder.MailBox.MailBoxId == _MailBoxId).Select(message => new MailApiModel
                {
                    ID = message.MessageID,
                    IsRead = message.IsRead,
                    From = message.From,
                    To = message.To,
                    Subject = message.Subject,
                    Date = message.Received,
                    Text = message.Body,
                    Category = message.Category.ToString(),
                    Email = message.Email,
                    Status = message.Status,
                    Owner = message.MailBoxFolder.MailBox.UserId == _UserId ? "YES" : "NO",
                    Name = message.Name,
                    IsValid = message.IsValid,
                    Url = message.Url
                }).ToList();
                return Json(result.ToDataSourceResult(request));
            }


        }

        [HttpGet]
        public IHttpActionResult GetMailBoxFolderList(long MailBoxId, string Defoult=null, string SerchedFolderString = null, string UserId = null)
        {

            long _UserId = Convert.ToInt32(UserId);
            long DefoultFolderId = 0;
            List<MailBoxFolderModel> Model = new List<MailBoxFolderModel>();
            using (var Entity = new WebMailEntities())
            {
                Model = Entity.MailBoxFolders.Where(x => x.MailBoxId == MailBoxId && x.IsActive == true).Select(s => new MailBoxFolderModel
                {
                    MailBoxFolderId = s.MailBoxFolderId,
                    MailBoxFolderName = s.MailBoxFolderName,
                    MailBoxId = s.MailBoxId,
                    Owner = s.MailBox.UserId == _UserId ? "YES" : "NO",
                    Sequence = s.Sequence,
                }).OrderBy(x => x.Sequence).ToList();

            }

            List<Folders> _FolderList = new List<Folders>();
            if (string.IsNullOrEmpty(SerchedFolderString))
            {
                _FolderList = Model.Select(x => new Folders
                {
                    MailBox = MailBoxId.ToString(),
                    text = x.MailBoxFolderName,
                    value = x.MailBoxFolderId.ToString(),
                    Owner = x.Owner
                }).ToList();
            }
            else
            {
                _FolderList = Model.Where(x => x.MailBoxFolderName.ToUpper().Contains(SerchedFolderString.ToUpper())).Select(x => new Folders
                {
                    MailBox = MailBoxId.ToString(),
                    text = x.MailBoxFolderName,
                    value = x.MailBoxFolderId.ToString(),
                    Owner = x.Owner
                }).ToList();
            }
            if (!string.IsNullOrEmpty(Defoult))
            {
                DefoultFolderId = Model.Select(x => x.MailBoxFolderId).Take(1).FirstOrDefault();
                return Json(DefoultFolderId);
            }
            else
            {
                return Json(_FolderList);
            }


        }

        [HttpGet]
        public List<System.Web.Mvc.SelectListItem> MailBoxList(int UserId, string request)
        {
            List<MailBoxModel> _Model = new List<MailBoxModel>();
            List<MailBoxModel> AssinedMailBox = new List<MailBoxModel>();
            List<System.Web.Mvc.SelectListItem> _MainBoxList = new List<System.Web.Mvc.SelectListItem>();
            string Result = string.Empty;
            using (var Entity = new WebMailEntities())
            {
                _Model = Entity.MailBoxes.Where(x => x.UserId == UserId).Select(s => new MailBoxModel
                {
                    MailBoxId = s.MailBoxId,
                    MailBoxName = s.MailBoxName,
                    MailBoxSequence = s.MailBoxSequence,
                    UserId = s.UserId,
                    Owener = "YES"
                }).OrderBy(x => x.MailBoxSequence).ToList();

            }
            using (var Entity = new WebMailEntities())
            {
                AssinedMailBox = Entity.MailBoxAccesses.Where(x => x.UserId == UserId).Select(s => new MailBoxModel
                {
                    MailBoxId = s.MailBoxId,
                    MailBoxName = s.MailBox.MailBoxName + " : " + s.MailBox.User.FullName,
                    MailBoxSequence = s.MailBox.MailBoxSequence,
                    UserId = s.UserId,
                    Owener = "NO"
                }).OrderBy(x => x.MailBoxSequence).ToList();
            }
            if (AssinedMailBox.Count() > 0)
            {
                _Model.AddRange(AssinedMailBox);
            }
            if (request == "DRP")
            {

                _MainBoxList = _Model.Select(s => new System.Web.Mvc.SelectListItem
                {
                    Text = s.MailBoxName,
                    Value = s.MailBoxId.ToString(),
                    //  Selected = s.MailBoxSequence == 1 ? true : false
                }).ToList();
            };

            return _MainBoxList;
        }

    }
}
