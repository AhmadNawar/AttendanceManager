using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceManager.Models
{
    public class ActiveSessionVm
    {
        public string SessionKey { get; set; }
        public List<string> Students { get; set; }
    }
}