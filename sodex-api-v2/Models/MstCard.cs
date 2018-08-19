using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sodex_api_v2.Models
{
    public class MstCard
    {
        public Int32 Id { get; set; }
        public String CardNumber { get; set; }
        public Decimal Balance { get; set; }
        public Int32 UserId { get; set; }
        public String FullName { get; set; }
        public String Email { get; set; }
        public String Address { get; set; }
        public String ContactNumber { get; set; }
        public String Particulars { get; set; }
        public String Status { get; set; }
    }
}