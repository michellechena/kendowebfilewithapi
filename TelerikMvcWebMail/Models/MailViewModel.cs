using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TelerikMvcWebMail.Models
{
    public class MailViewModel
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

        [Required(ErrorMessage = "name required")]
        public string Name { get; set; }
        public string Url { get; set; }
        public Nullable<bool> IsValid { get; set; }
        public string RequestFrom { get; set; }
    
    }
    
    public class MailBoxFolderModel
    {
        public string MailBoxName { get; set; }
        public int MailBoxFolderId { get; set; }
        [Display(Name = "Folder Name")]
        [Required(ErrorMessage = "Folder name required")]
        public string MailBoxFolderName { get; set; }
        [Required(ErrorMessage ="MailBox required")]
        [Display(Name ="Mail Box")]
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