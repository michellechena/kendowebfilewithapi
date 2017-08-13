using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TelerikMvcWebMail.Models
{
    public class MailsService
    {
        private WebMailEntities entities;

        private static bool UpdateDatabase = false;

        public MailsService(WebMailEntities entities)
        {
            this.entities = entities;
           
        }

        public IList<MailViewModel> Read(string UserId,string AjaxRequest=null,string MailBoxId=null)
        {
            long _UserId = Convert.ToInt32(UserId);
            long _MailBoxId = Convert.ToInt32(MailBoxId);           
            IList<MailViewModel> result = HttpContext.Current.Session["Mails"] as IList<MailViewModel>;
                   
                result = entities.Mails.Where(x=>x.MailBoxFolder.MailBox.MailBoxId== _MailBoxId).Select(message => new MailViewModel
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
                    Status=message.Status,
                    Owner=message.MailBoxFolder.MailBox.UserId== _UserId?"YES":"NO",
                    Name=message.Name,
                    IsValid=message.IsValid,
                    Url=message.Url
                }).ToList();

               
            

            return result;
        }
        public MailViewModel ReadMailDetails(string MailId)
        {
            long _MailId = Convert.ToInt32(MailId);
            MailViewModel result = new Models.MailViewModel();

            result = entities.Mails.Where(x => x.MessageID== _MailId).Select(message => new MailViewModel
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
            
            return result;
        }

        public void Create(MailViewModel mail)
        {
            if (!UpdateDatabase)
            {
                var first = Read(HttpContext.Current.Session["UserId"].ToString()).OrderByDescending(e => e.ID).FirstOrDefault();
                var id = (first != null) ? first.ID : 0;

                mail.ID = id + 1;

                Read(HttpContext.Current.Session["UserId"].ToString()).Insert(0, mail);
            }
            else
            {
                var entity = mail.ToEntity();

                entities.Mails.Add(entity);
                entities.SaveChanges();

                mail.ID = entity.MessageID;
            }
        }

        public void Update(MailViewModel mail)
        {
            UpdateDatabase = true;

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

        public MailViewModel One(Func<MailViewModel, bool> predicate)
        {
            return Read(HttpContext.Current.Session["UserId"].ToString()).FirstOrDefault(predicate);
        }

        public void Dispose()
        {
            entities.Dispose();
        }

       public int GetFirstFolderId(string UserId,int MailBoxId=0)
        {
            long Id = Convert.ToInt32(UserId);
            int _MailBoxId = Convert.ToInt32(MailBoxId);
            int FolderId = 0;
            if (_MailBoxId==0)
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
            return FolderId;
        }
        public bool UpdateEmailSubject(MailViewModel mail,string UpdateOnlyDisable=null)
        {
            try
            {
                using (var Entity = new WebMailEntities())
                {
                    Mail Mail = Entity.Mails.Where(s => s.MessageID == mail.ID).FirstOrDefault();
                    if (UpdateOnlyDisable == "Disable")
                    {
                        if (Mail != null)
                        {                            
                            Mail.Status ="D";
                            Entity.SaveChanges();
                        }
                    }
                   else if (UpdateOnlyDisable == "Deleted")
                    {
                        if (Mail != null)
                        {

                            Entity.Mails.Remove(Mail);
                            Entity.SaveChanges();
                            
                        }
                    }
                    else
                    {
                        if (Mail != null)
                        {
                            Mail.Subject = mail.Subject;
                            Mail.Status = mail.Status;
                            Entity.SaveChanges();
                        }
                    }
                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }

        }
    }
}