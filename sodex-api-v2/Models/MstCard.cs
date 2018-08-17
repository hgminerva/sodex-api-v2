using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sodex_api_v2.Models
{
    public class MstCard
    {
        public int Id { get; set; }
        public String CardNumber { get; set; }
        public decimal Balance { get; set; }
    }
}