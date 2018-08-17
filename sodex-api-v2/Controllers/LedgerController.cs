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

        // POST api/ledger/transfer
        [HttpPost, Route("Transfer")]
        public HttpResponseMessage Post(Models.TransferData transferData)
        {
            var source = from d in db.MstCards 
                         where d.CardNumber == transferData.SourceCardNumber 
                         select d;

            var destination = from d in db.MstCards
                              where d.CardNumber == transferData.DestinationCardNumber
                              select d;

            if (source.Any() && destination.Any())
            {
                decimal sourceBalance = 0;

                sourceBalance = source.FirstOrDefault().Balance;


                if (sourceBalance >= transferData.Amount)
                {
                    try
                    {
                        var s = source.FirstOrDefault();
                        var d = destination.FirstOrDefault();
                        DateTime timeStamp = DateTime.Now;

                        Data.TrnLedger newLedger1 = new Data.TrnLedger()
                        {
                            CardId = s.Id,
                            CardNumber = s.CardNumber,
                            LedgerDateTime = timeStamp,
                            DebitAmount = 0,
                            CreditAmount = transferData.Amount,
                            Particulars = transferData.Particulars
                        };

                        Data.TrnLedger newLedger2 = new Data.TrnLedger()
                        {
                            CardId = d.Id,
                            CardNumber = d.CardNumber,
                            LedgerDateTime = timeStamp,
                            DebitAmount = transferData.Amount,
                            CreditAmount = 0,
                            Particulars = transferData.Particulars
                        };

                        db.TrnLedgers.InsertOnSubmit(newLedger1);
                        db.TrnLedgers.InsertOnSubmit(newLedger2);

                        db.SubmitChanges();

                        s.Balance = s.TrnLedgers.Sum(t => t.DebitAmount - t.CreditAmount);
                        d.Balance = d.TrnLedgers.Sum(t => t.DebitAmount - t.CreditAmount);

                        db.SubmitChanges();

                    }
                    catch(Exception e)
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.ToString()) }; 
                    }
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Success") }; 
        }

    }
}
