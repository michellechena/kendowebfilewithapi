using APIs.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIs.Models
{
    public class MailApiModel
    {
        public int ID { get; set; }

        public bool? IsRead { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public DateTime? Date { get; set; }

        [AllowHtml]
        public string Text { get; set; }

        public string Category { get; set; }

        public string To { get; set; }

        public string Email { get; set; }
        public string Status { get; set; }
        public string Owner { get; set; }

        public string Name { get; set; }
        public string Url { get; set; }
        public Nullable<bool> IsValid { get; set; }
        public string RequestFrom { get; set; }
    }
    public class MailBoxModel
    {
        public int MailBoxId { get; set; }
        public string MailBoxName { get; set; }
        public int UserId { get; set; }
        public long MailBoxSequence { get; set; }
        public virtual User User { get; set; }
        public string Owener { get; set; }

    }

    public class MailBoxFolderModel
    {
        public string MailBoxName { get; set; }
        public int MailBoxFolderId { get; set; }        
        public string MailBoxFolderName { get; set; }       
        public int MailBoxId { get; set; }
        public long Sequence { get; set; }
        public string Owner { get; set; }
        public string UserId { get; set; }
        public string Sequenseids { get; set; }
    }

    public class Folders
    {
        public string text { get; set; }
        public string value { get; set; }
        public string number { get; set; }
        public string MailBox { get; set; }
        public string Active { get; set; }
        public string Disable { get; set; }
        public string Owner { get; set; }
    }
}