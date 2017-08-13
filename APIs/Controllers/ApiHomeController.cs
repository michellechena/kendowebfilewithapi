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
using System.Web.WebPages.Html;

namespace APIs.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ApiHomeController : ApiController
    {
        [HttpPost]
        public IHttpActionResult ValidateUser(SignIn Model)
        {
            UserApIModel _User = new UserApIModel();
            string Result = string.Empty;
            using (var Entity = new WebMailEntities())
            {
                _User= Entity.Users.Where(s => s.Email.ToUpper() == Model.UserName.ToUpper()
                && s.Password.ToUpper() == Model.Password.ToUpper()).Select(x => new UserApIModel
                {
                    Email = x.Email,
                    FullName = x.FullName,
                    Password = x.Password,
                    UserId = x.UserId
                }).FirstOrDefault();

            }
            return Json(_User);
        }
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

        [HttpPost]
        [System.Web.Mvc.ValidateInput(false)]
        public IHttpActionResult UpdateEmailDetailes([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request, MailApiModel mail)
        {
            if (mail.Category == "Disable")
            {
                using (var Entity = new WebMailEntities())
                {
                    Mail Mail = Entity.Mails.Where(s => s.MessageID == mail.ID).FirstOrDefault();
                    if (Mail != null)
                    {
                        Mail.Status = "D";
                        Entity.SaveChanges();
                    }
                }
                }
            else if (mail.Category == "Deleted")
            {
                using (var Entity = new WebMailEntities())
                {
                    Mail Mail = Entity.Mails.Where(s => s.MessageID == mail.ID).FirstOrDefault();
                    if (Mail != null)
                    {
                        Entity.Mails.Remove(Mail);
                        Entity.SaveChanges();
                    }
                }
            }
            else
            {

                using (var Entity = new WebMailEntities())
                {
                    var target = Entity.Mails.Where(x => x.MessageID == mail.ID).FirstOrDefault();
                    if (target != null)
                    {
                        int _Category = Convert.ToInt32(mail.Category);
                        target.From = mail.From;
                        target.Subject = mail.Subject;
                        target.IsRead = mail.IsRead;
                        target.To = mail.To;
                        target.Category = _Category;
                        target.Email = mail.Email;
                        Entity.SaveChanges();
                    }
                }
            }

            return Json(new[] { mail }.ToDataSourceResult(request));
        }

        [HttpPost]
        public IHttpActionResult UpdateEmailSubject( MailApiModel mail)
        {
            try
            {
                using (var Entity = new WebMailEntities())
                {
                    Mail Mail = Entity.Mails.Where(s => s.MessageID == mail.ID).FirstOrDefault();
                    if (Mail != null)
                    {
                        Mail.Status = mail.Status;
                        Mail.Subject = mail.Subject;
                        Entity.SaveChanges();
                    }
                }
                return Json(true);
            }
            catch(Exception ex)
            {
                return Json(false);
            }

            
        }

        [HttpGet]
        public MailApiModel ReadMailDetails(string MailId)
        {
            long _MailId = Convert.ToInt32(MailId);
            MailApiModel result = new Models.MailApiModel();

            using (var Entity = new WebMailEntities())
            {
                result = Entity.Mails.Where(x => x.MessageID == _MailId).Select(message => new MailApiModel
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
                }).FirstOrDefault();
            }
            return result;
        }

        [HttpGet]
        public string GetFirstFolderId(string UserId, int MailBoxId = 0)
        {
            long Id = Convert.ToInt32(UserId);
            int _MailBoxId = Convert.ToInt32(MailBoxId);
            int FolderId = 0;
            if (_MailBoxId == 0)
            {
                using (var Entity = new WebMailEntities())
                {
                    _MailBoxId = Entity.MailBoxes.Where(s => s.UserId == Id).OrderBy(s => s.MailBoxSequence).Take(1).Select(z => z.MailBoxId).FirstOrDefault();

                }
            }
            using (var Entity = new WebMailEntities())
            {
                FolderId = Entity.MailBoxFolders.Where(s => s.MailBoxId == _MailBoxId).OrderBy(s => s.Sequence).Take(1).Select(z => z.MailBoxFolderId).FirstOrDefault();

            }
            return FolderId.ToString();
        }

        [HttpPost]
        public bool SaveNewEmail(MailApiModel _MailViewModel)
        {
            try
            {
                Mail _Mail = new Mail();
                _Mail.Category = Convert.ToInt32(_MailViewModel.Category);
                _Mail.IsValid = _MailViewModel.IsValid;
                _Mail.Name = _MailViewModel.Name;
                _Mail.Url = _MailViewModel.Url;
                using (var Entity = new WebMailEntities())
                {
                    Entity.Mails.Add(_Mail);
                    Entity.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

        [HttpGet]
        public bool UpdateMailStatus(string Id, string flag)
        {
            try
            {
                int _Id = Convert.ToInt32(Id);
                using (var Entity = new WebMailEntities())
                {
                    Mail _Mail = Entity.Mails.Where(x => x.MessageID == _Id).FirstOrDefault();
                    if (_Mail != null)
                    {
                        _Mail.Status = flag;
                        Entity.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

        [HttpGet]
        public IHttpActionResult UserFolderList([ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request,string UserId)
        {
            long _UserId = Convert.ToInt32(UserId);
            List<MailBoxFolderModel> Model = new List<MailBoxFolderModel>();
            using (var Entity = new WebMailEntities())
            {
                Model = Entity.MailBoxFolders.Where(a => a.MailBox.UserId == _UserId && a.IsActive == true).Select(s => new MailBoxFolderModel
                {
                    MailBoxFolderId = s.MailBoxFolderId,
                    MailBoxFolderName = s.MailBoxFolderName,
                    MailBoxId = s.MailBoxId,
                    Owner = s.MailBox.UserId == _UserId ? "YES" : "NO",
                    Sequence = s.Sequence,
                    MailBoxName = s.MailBox.MailBoxName
                }).OrderBy(x => x.MailBoxId).ThenBy(x => x.Sequence).ToList();

            }
            return Json(Model.ToDataSourceResult(request));
           
        }

        [HttpGet]
        public List<SelectListItem> MailBoxListForFolder(string UserId)
        {
            List<SelectListItem> _MainBoxList = new List<SelectListItem>();
            long _UserId = Convert.ToInt32(UserId);
            using (var Entity = new WebMailEntities())
            {
                _MainBoxList = Entity.MailBoxes.Where(x => x.UserId == _UserId).Select(s => new SelectListItem
                {
                    Text = s.MailBoxName,
                    Value = s.MailBoxId.ToString(),
                }).ToList();

            }
            return _MainBoxList;
        }
        [HttpGet]
        public MailBoxFolderModel GetFolderDeatiles(string FolderId)
        {
            long _FolderId = Convert.ToInt32(FolderId);
            MailBoxFolderModel Model = new MailBoxFolderModel();
            using (var Entity = new WebMailEntities())
            {
                Model = Entity.MailBoxFolders.Select(s => new MailBoxFolderModel
                {
                    MailBoxFolderId = s.MailBoxFolderId,
                    MailBoxFolderName = s.MailBoxFolderName,
                    MailBoxId = s.MailBoxId,
                    Sequence = s.Sequence,
                }).Where(x => x.MailBoxFolderId == _FolderId).FirstOrDefault();

            }
            return Model;
        }

        [HttpPost]
        public bool AddEditFolder(MailBoxFolderModel Model)
        {
            try
            {
                MailBoxFolder _MailBoxFolders = new MailBoxFolder();
                //For Add
                if (string.IsNullOrEmpty(Model.MailBoxFolderId.ToString()) || Model.MailBoxFolderId == 0)
                {
                    using (var Entity = new WebMailEntities())
                    {
                        _MailBoxFolders.MailBoxFolderName = Model.MailBoxFolderName;
                        _MailBoxFolders.MailBoxId = Model.MailBoxId;
                        _MailBoxFolders.Sequence = 0;
                        _MailBoxFolders.IsActive = true;
                        Entity.MailBoxFolders.Add(_MailBoxFolders);
                        Entity.SaveChanges();
                    }
                }
                else
                {
                    using (var Entity = new WebMailEntities())
                    {
                        MailBoxFolder _MailBoxUpdate = Entity.MailBoxFolders.Where(s => s.MailBoxFolderId == Model.MailBoxFolderId).FirstOrDefault();
                        if (_MailBoxUpdate != null)
                        {
                            _MailBoxUpdate.MailBoxFolderName = Model.MailBoxFolderName;
                            _MailBoxUpdate.MailBoxId = Model.MailBoxId;
                            Entity.SaveChanges();
                        }
                    }
                }
                using (var entity = new WebMailEntities())
                {
                    var SequenceArry = Model.Sequenseids.Split(',');
                    for (int i = 0; i < SequenceArry.Count(); i++)
                    {
                        MailBoxFolder _UpdateSequence = new MailBoxFolder();
                        if (SequenceArry[i].ToString() == "NEWREC")
                        {
                            _UpdateSequence = entity.MailBoxFolders.SingleOrDefault(b => b.MailBoxFolderId == _MailBoxFolders.MailBoxFolderId);
                        }
                        else
                        {
                            Int32 id = Convert.ToInt32(SequenceArry[i].ToString());
                            _UpdateSequence = entity.MailBoxFolders.SingleOrDefault(b => b.MailBoxFolderId == id);
                        }
                        _UpdateSequence.Sequence = i + 1;
                        entity.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        [HttpGet]
        public bool DeleteFolder(string Id)
        {
            try
            {
                long _Id = Convert.ToInt32(Id);
                MailBoxFolder _MailBoxFolders = new MailBoxFolder();

                using (var Entity = new WebMailEntities())
                {
                    MailBoxFolder _MailBoxUpdate = Entity.MailBoxFolders.Where(s => s.MailBoxFolderId == _Id).FirstOrDefault();
                    if (_MailBoxUpdate != null)
                    {
                        _MailBoxUpdate.IsActive = false;
                        Entity.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet]
        public List<MailBoxFolderModel> MailBoxFolderListForFolderPage(long MailBoxId, string UserId)
        {
            long _UserId = Convert.ToInt32(UserId);
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
            return Model;
        }


    }
}
