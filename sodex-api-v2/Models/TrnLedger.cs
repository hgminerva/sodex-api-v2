using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sodex_api_v2.Models
{
    public class TrnLedger
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public String CardNumber { get; set; }
        public String LedgerDateTime { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public String Particulars { get; set; }
    }
}