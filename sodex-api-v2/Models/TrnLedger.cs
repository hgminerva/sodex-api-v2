using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sodex_api_v2.Models
{
    public class TrnLedger
    {
        public Int32 Id { get; set; }
        public String Document { get; set; }
        public Int32 CardId { get; set; }
        public String CardNumber { get; set; }
        public String LedgerDateTime { get; set; }
        public String CardOwner { get; set; }
        public Decimal DebitAmount { get; set; }
        public Decimal CreditAmount { get; set; }
        public Decimal RunningBalance { get; set; }
        public String Particulars { get; set; }
    }
}