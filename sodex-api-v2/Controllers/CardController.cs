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
                            Balance = d.Balance,
                            Particulars = d.Particulars
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
                    Data.MstCard newCard = new Data.MstCard
                    {
                        CardNumber = objCard.CardNumber,
                        Balance = objCard.Balance,
                        UserId = currentUser.FirstOrDefault().Id,
                        Particulars = objCard.Particulars
                    };

                    db.MstCards.InsertOnSubmit(newCard);
                    db.SubmitChanges();

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
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
                        updateCurrentCard.Balance = objCard.Balance;
                        updateCurrentCard.UserId = currentUser.FirstOrDefault().Id;
                        updateCurrentCard.Particulars = objCard.Particulars;

                        db.SubmitChanges();

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
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
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}