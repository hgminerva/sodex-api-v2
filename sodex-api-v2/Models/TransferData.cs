using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sodex_api_v2.Models
{
    public class TransferData
    {
        public String SourceCardNumber { get; set; }
        public String DestinationCardNumber { get; set; }
        public decimal Amount { get; set; }
        public String Particulars { get; set; }
    }
}