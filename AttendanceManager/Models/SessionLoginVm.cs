using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceManager.Models
{
    public class SessionLoginVm
    {
        [Display(Name = "Session Key")]
        public string SessionKey { get; set; }
        public string Title { get; set; }
        [Display(Name = "Student Id")]
        public string StudentId { get; set; }
        [Display(Name="First Name")]
        public string StudentFirstName { get; set; }
        [Display(Name="Last Name")]
        public string StudentLastName { get; set; }

        public string Works { get; set; }
    }
}