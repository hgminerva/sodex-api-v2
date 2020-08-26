using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sodex_api_v2.Models
{
    public class RepDailySummaryReport
    {
        public Decimal BeginningBalance { get; set; }
        public Decimal TotalDebit { get; set; }
        public Decimal TotalCredit { get; set; }
        public Decimal EndingBalance { get; set; }
        public Decimal MotherCardEndingBalance { get; set; }
    }
}