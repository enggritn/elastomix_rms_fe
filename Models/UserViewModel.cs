using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class UserViewModel
    {
        public string Username { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}