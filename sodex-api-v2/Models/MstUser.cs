using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sodex_api_v2.Models
{
    public class MstUser
    {
        public Int32 Id { get; set; }
        public String AspNetUserId { get; set; }
        public String Username { get; set; }
        public Int32 UserTypeId { get; set; }
        public String FullName { get; set; }
        public String Address { get; set; }
        public String Email { get; set; }
        public String ContactNumber { get; set; }
        public String MotherCardNumber { get; set; }
        public Decimal Balance { get; set; }
    }
}