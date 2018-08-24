using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace sodex_api_v2.Controllers
{
    [Authorize, RoutePrefix("api/reportLedger")]
    public class ReportLedgerController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.SodexDatabaseDataContext db = new Data.SodexDatabaseDataContext();

        // ====================
        // Get Bulleted Strings
        // ====================
        public String GetCardNumberBulletedString(String cardNumber)
        {
            var firsChar = cardNumber[0];

            var slicedChars = cardNumber.Substring(1, cardNumber.Length - 1);
            var bulletedChars = new string('•', slicedChars.Length - 1);

            var lastChar = cardNumber[cardNumber.Length - 1];

            return firsChar + bulletedChars + lastChar;
        }

        // =============
        // List - Ledger
        // =============
        [HttpGet, Route("list/{cardNumber}/{dateStart}/{dateEnd}")]
        public List<Models.TrnLedger> GetLedger(string cardNumber, string dateStart, string dateEnd)
        {
            var TrnLedgerData = from d in db.TrnLedgers
                                where d.MstCard.CardNumber.Equals(cardNumber)
                                && d.LedgerDateTime >= Convert.ToDateTime(dateStart)
                                && d.LedgerDateTime <= Convert.ToDateTime(dateEnd).AddHours(24)
                                select new Models.TrnLedger
                                {
                                    Id = d.Id,
                                    CardId = d.CardId,
                                    CardNumber = GetCardNumberBulletedString(d.CardNumber),
                                    LedgerDateTime = d.LedgerDateTime.ToShortDateString(),
                                    DebitAmount = d.DebitAmount,
                                    CreditAmount = d.CreditAmount,
                                    Particulars = d.Particulars
                                };

            return TrnLedgerData.OrderByDescending(d => d.Id).ToList();
        }
    }
}
