using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sodex_api_v2.Models
{
    public class MstUserForm
    {
        public Int32 Id { get; set; }
        public Int32 UserId { get; set; }
        public Int32 FormId { get; set; }
        public String Form { get; set; }
        public String Particulars { get; set; }
        public Boolean CanAdd { get; set; }
        public Boolean CanEdit { get; set; }
        public Boolean CanUpdate { get; set; }
        public Boolean CanDelete { get; set; }
    }
}