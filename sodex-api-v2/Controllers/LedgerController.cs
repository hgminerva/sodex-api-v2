using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace sodex_api_v2.Controllers
{
    [RoutePrefix("api/Ledger")] 
    public class LedgerController : ApiController
    {
        private Data.SodexDatabaseDataContext db = new Data.SodexDatabaseDataContext();

        // GET api/ledger/getBalance/cardNumber
        [HttpGet, Route("GetBalance")]
        public Decimal GetBalance(string cardNumber)
        {
            decimal balance = 0;

            var card = from d in db.MstCards 
                       where d.CardNumber == cardNumber 
                       select d;

            if (card.Any())
            {
                balance = card.FirstOrDefault().Balance;
            }

            return balance;
        }

        // GET api/ledger/getStatus
        [HttpGet, Route("GetStatus")]
        public String GetStatus()
        {
            return "OK";
        }

        // GET api/ledger/getLedger
        [HttpGet, Route("GetLedger/{cardNumber}/{dateStart}/{dateEnd}")]
        public List<Models.TrnLedger> GetLedger(string cardNumber, string dateStart, string dateEnd)
        {
            var TrnLedgerData = from d in db.TrnLedgers
                                where d.MstCard.CardNumber == cardNumber &&
                                      (d.LedgerDateTime >= Convert.ToDateTime(dateStart) &&
                                       d.LedgerDateTime <= Convert.ToDateTime(dateEnd))
                                orderby d.Id ascending
                                select new Models.TrnLedger
                                {
                                    Id = d.Id,
                                    CardId = d.CardId,
                                    CardNumber = d.CardNumber,
                                    LedgerDateTime = d.LedgerDateTime.ToShortDateString(),
                                    DebitAmount = d.DebitAmount,
                                    CreditAmount = d.CreditAmount,
                                    Particulars = d.Particulars
                                };

            return TrnLedgerData.ToList();
        }
    }
}
