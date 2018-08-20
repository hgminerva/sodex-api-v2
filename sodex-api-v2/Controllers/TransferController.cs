using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace sodex_api_v2.Controllers
{
    [Authorize, RoutePrefix("api/transfer")]
    public class TransferController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.SodexDatabaseDataContext db = new Data.SodexDatabaseDataContext();

        // =======================
        // Get Mother Card Balance
        // =======================
        [HttpGet, Route("motherCardBalance")]
        public Decimal GetMotherCardBalance()
        {
            var currentUser = from d in db.MstUsers
                              where d.AspNetUserId == User.Identity.GetUserId()
                              select d;

            return currentUser.FirstOrDefault().Balance;
        }

        // =============
        // Detail - Card
        // =============
        [HttpGet, Route("detail/card/{cardNumber}")]
        public Models.MstCard DetailTransferCard(String cardNumber)
        {
            var card = from d in db.MstCards.OrderByDescending(d => d.Id)
                       where d.CardNumber.Equals(cardNumber)
                       select new Models.MstCard
                       {
                           Id = d.Id,
                           CardNumber = d.CardNumber,
                           FullName = d.FullName,
                           Address = d.Address,
                           Email = d.Email,
                           ContactNumber = d.ContactNumber,
                           UserId = d.UserId,
                           Balance = d.Balance,
                           Particulars = d.Particulars,
                           Status = d.Status,
                       };

            return card.FirstOrDefault();
        }

        // ======================
        // Transfer Amount - Card
        // ======================
        [HttpPut, Route("transferAmount")]
        public HttpResponseMessage TransferAmount(Models.TransferData transferData)
        {
            var currentUser = from d in db.MstUsers
                              where d.AspNetUserId == User.Identity.GetUserId()
                              select d;

            var source = from d in db.MstCards
                         where d.CardNumber == currentUser.FirstOrDefault().MotherCardNumber
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
                            Particulars = currentUser.FirstOrDefault().FullName + " " + DateTime.Now.ToString()
                        };

                        Data.TrnLedger newLedger2 = new Data.TrnLedger()
                        {
                            CardId = d.Id,
                            CardNumber = d.CardNumber,
                            LedgerDateTime = timeStamp,
                            DebitAmount = transferData.Amount,
                            CreditAmount = 0,
                            Particulars = currentUser.FirstOrDefault().FullName + " " + DateTime.Now.ToString()
                        };

                        db.TrnLedgers.InsertOnSubmit(newLedger1);
                        db.TrnLedgers.InsertOnSubmit(newLedger2);

                        db.SubmitChanges();

                        s.Balance = s.TrnLedgers.Sum(t => t.DebitAmount - t.CreditAmount);
                        d.Balance = d.TrnLedgers.Sum(t => t.DebitAmount - t.CreditAmount);

                        db.SubmitChanges();

                    }
                    catch (Exception e)
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(e.ToString()) };
                    }
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Success") };
        }
    }
}
