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
    [Authorize, RoutePrefix("api/cards")]
    public class CardController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.SodexDatabaseDataContext db = new Data.SodexDatabaseDataContext();

        // ===========
        // List - Card
        // ===========
        [HttpGet, Route("list")]
        public List<Models.MstCard> ListCard()
        {
            var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;
            if (currentUser.Any())
            {
                var cards = from d in db.MstCards.OrderByDescending(d => d.Id)
                            where d.UserId == currentUser.FirstOrDefault().Id
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
                                Status = d.Status
                            };

                return cards.ToList();
            }
            else
            {
                return new List<Models.MstCard>();
            }
        }

        // =============
        // Detail - Card
        // =============
        [HttpGet, Route("detail/{cardNumber}")]
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
                           Status = d.Status
                       };

            return card.FirstOrDefault();
        }

        // ==========
        // Add - Card
        // ==========
        [HttpPost, Route("add")]
        public HttpResponseMessage AddCard(Models.MstCard objCard)
        {
            try
            {
                var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;
                if (currentUser.Any())
                {
                    if (currentUser.FirstOrDefault().UserTypeId != 3)
                    {
                        var currentCard = from d in db.MstCards where d.CardNumber.Equals(objCard.CardNumber) select d;
                        if (!currentCard.Any())
                        {
                            Data.MstCard newCard = new Data.MstCard
                            {
                                CardNumber = objCard.CardNumber,
                                FullName = objCard.FullName,
                                Address = objCard.Address,
                                Email = objCard.Email,
                                ContactNumber = objCard.ContactNumber,
                                UserId = currentUser.FirstOrDefault().Id,
                                Balance = 0,
                                Particulars = currentUser.FirstOrDefault().FullName + " " + DateTime.Now.ToString(),
                                Status = objCard.Status
                            };

                            db.MstCards.InsertOnSubmit(newCard);
                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. Your card number was already taken.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to add cards.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. No current user logged in.");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Something's went wrong from the server. " + e.Message);
            }
        }

        // =============
        // Update - Card
        // =============
        [HttpPut, Route("update/{id}")]
        public HttpResponseMessage UpdateCard(String id, Models.MstCard objCard)
        {
            try
            {
                var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;
                if (currentUser.Any())
                {
                    if (currentUser.FirstOrDefault().UserTypeId != 3)
                    {
                        var currentCard = from d in db.MstCards where d.Id == Convert.ToInt32(id) select d;
                        if (currentCard.Any())
                        {
                            var updateCurrentCard = currentCard.FirstOrDefault();
                            updateCurrentCard.CardNumber = objCard.CardNumber;
                            updateCurrentCard.FullName = objCard.FullName;
                            updateCurrentCard.Address = objCard.Address;
                            updateCurrentCard.Email = objCard.Email;
                            updateCurrentCard.ContactNumber = objCard.ContactNumber;
                            updateCurrentCard.UserId = currentUser.FirstOrDefault().Id;
                            updateCurrentCard.Particulars = currentUser.FirstOrDefault().FullName + " " + DateTime.Now.ToString();
                            updateCurrentCard.Status = objCard.Status;

                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. Your card number was not found in the server.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to update cards.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. No current user logged in.");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Something's went wrong from the server. " + e.Message);
            }
        }

        // =============
        // Delete - Card
        // =============
        [HttpDelete, Route("delete/{id}")]
        public HttpResponseMessage DeleteCard(String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;
                if (currentUser.Any())
                {
                    if (currentUser.FirstOrDefault().UserTypeId != 3)
                    {
                        var currentCard = from d in db.MstCards where d.Id == Convert.ToInt32(id) select d;
                        if (currentCard.Any())
                        {
                            db.MstCards.DeleteOnSubmit(currentCard.FirstOrDefault());
                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. Your card number was not found in the server.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to delete cards.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. No current user logged in.");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Something's went wrong from the server. " + e.Message);
            }
        }

        // =======================
        // Get Mother Card Balance
        // =======================
        [HttpGet, Route("motherCardBalance")]
        public Decimal GetMotherCardBalance()
        {
            Decimal motherCardBalance = 0;

            var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;
            if (currentUser.Any())
            {
                var card = from d in db.MstCards where d.CardNumber.Equals(currentUser.FirstOrDefault().MotherCardNumber) select d;
                if (card.Any())
                {
                    motherCardBalance = card.FirstOrDefault().Balance;
                }
            }

            return motherCardBalance;
        }

        // ======================
        // Transfer Amount - Card
        // ======================
        [HttpPut, Route("transfer")]
        public HttpResponseMessage TransferAmount(Models.TransferData transferData)
        {
            try
            {
                var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;
                if (currentUser.Any())
                {
                    var source = from d in db.MstCards where d.CardNumber == currentUser.FirstOrDefault().MotherCardNumber select d;
                    var destination = from d in db.MstCards where d.CardNumber == transferData.DestinationCardNumber select d;

                    if (source.Any() && destination.Any())
                    {
                        Decimal sourceBalance = source.FirstOrDefault().Balance;
                        if (sourceBalance >= transferData.Amount)
                        {
                            Data.TrnLedger newLedgerSource = new Data.TrnLedger()
                            {
                                CardId = source.FirstOrDefault().Id,
                                CardNumber = source.FirstOrDefault().CardNumber,
                                LedgerDateTime = DateTime.Now,
                                DebitAmount = 0,
                                CreditAmount = transferData.Amount,
                                Particulars = currentUser.FirstOrDefault().FullName + " " + DateTime.Now.ToString()
                            };

                            Data.TrnLedger newLedgerDestination = new Data.TrnLedger()
                            {
                                CardId = destination.FirstOrDefault().Id,
                                CardNumber = destination.FirstOrDefault().CardNumber,
                                LedgerDateTime = DateTime.Now,
                                DebitAmount = transferData.Amount,
                                CreditAmount = 0,
                                Particulars = currentUser.FirstOrDefault().FullName + " " + DateTime.Now.ToString()
                            };

                            db.TrnLedgers.InsertOnSubmit(newLedgerSource);
                            db.TrnLedgers.InsertOnSubmit(newLedgerDestination);

                            db.SubmitChanges();

                            var updateSource = source.FirstOrDefault();
                            var updateDistination = destination.FirstOrDefault();

                            updateSource.Balance = source.FirstOrDefault().TrnLedgers.Any() ? source.FirstOrDefault().TrnLedgers.Sum(s => s.DebitAmount - s.CreditAmount) : 0;
                            updateDistination.Balance = destination.FirstOrDefault().TrnLedgers.Any() ? destination.FirstOrDefault().TrnLedgers.Sum(d => d.DebitAmount - d.CreditAmount) : 0;

                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.BadRequest, "The source card balance should be greater than or equal to your destination card balance.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Some cards are not found in the server.");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. No current user logged in.");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Something's went wrong from the server. " + e.Message);
            }
        }
    }
}