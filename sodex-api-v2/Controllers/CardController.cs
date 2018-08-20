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
            var currentUser = from d in db.MstUsers
                              where d.AspNetUserId == User.Identity.GetUserId()
                              select d;

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

        // ==========
        // Add - Card
        // ==========
        [HttpPost, Route("add")]
        public HttpResponseMessage AddCard(Models.MstCard objCard)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.AspNetUserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.FirstOrDefault().UserTypeId != 3)
                {
                    var currentCard = from d in db.MstCards
                                      where d.CardNumber.Equals(objCard.CardNumber)
                                      select d;

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
                            Balance = objCard.Balance,
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
                var currentUser = from d in db.MstUsers
                                  where d.AspNetUserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.FirstOrDefault().UserTypeId != 3)
                {
                    var currentCard = from d in db.MstCards
                                      where d.Id == Convert.ToInt32(id)
                                      select d;

                    if (currentCard.Any())
                    {
                        var updateCurrentCard = currentCard.FirstOrDefault();
                        updateCurrentCard.CardNumber = objCard.CardNumber;
                        updateCurrentCard.FullName = objCard.FullName;
                        updateCurrentCard.Address = objCard.Address;
                        updateCurrentCard.Email = objCard.Email;
                        updateCurrentCard.ContactNumber = objCard.ContactNumber;
                        updateCurrentCard.UserId = currentUser.FirstOrDefault().Id;
                        updateCurrentCard.Balance = objCard.Balance;
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
                var currentUser = from d in db.MstUsers
                                  where d.AspNetUserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.FirstOrDefault().UserTypeId != 3)
                {
                    var currentCard = from d in db.MstCards
                                      where d.Id == Convert.ToInt32(id)
                                      select d;

                    if (currentCard.Any())
                    {
                        db.MstCards.DeleteOnSubmit(currentCard.First());
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
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Something's went wrong from the server. " + e.Message);
            }
        }
    }
}