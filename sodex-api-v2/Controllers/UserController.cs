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
    [Authorize, RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.SodexDatabaseDataContext db = new Data.SodexDatabaseDataContext();

        // =======================
        // Get Mother Card Balance
        // =======================
        public Decimal GetMotherCardBalance(String cardNumber)
        {
            Decimal motherCardBalance = 0;

            var card = from d in db.MstCards where d.CardNumber.Equals(cardNumber) select d;
            if (card.Any())
            {
                motherCardBalance = card.FirstOrDefault().Balance;
            }

            return motherCardBalance;
        }

        // ===========
        // List - User
        // ===========
        [HttpGet, Route("list")]
        public List<Models.MstUser> ListUser()
        {
            var users = from d in db.MstUsers.OrderByDescending(d => d.Id)
                        select new Models.MstUser
                        {
                            Id = d.Id,
                            AspNetUserId = d.AspNetUserId,
                            Username = d.Username,
                            UserTypeId = d.UserTypeId,
                            FullName = d.FullName,
                            Address = d.Address,
                            Email = d.Email,
                            ContactNumber = d.ContactNumber,
                            MotherCardNumber = d.MotherCardNumber,
                            MotherCardBalance = GetMotherCardBalance(d.MotherCardNumber),
                            Status = d.Status
                        };

            return users.ToList();
        }

        // =============
        // Update - User
        // =============
        [HttpPut, Route("update/{id}")]
        public HttpResponseMessage UpdateUser(String id, Models.MstUser objUser)
        {
            try
            {
                var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;
                if (currentUser.Any())
                {
                    if (currentUser.FirstOrDefault().UserTypeId != 3)
                    {
                        var user = from d in db.MstUsers where d.Id == Convert.ToInt32(id) select d;
                        if (user.Any())
                        {
                            var updateUser = user.FirstOrDefault();
                            updateUser.FullName = objUser.FullName;
                            updateUser.Address = objUser.Address;
                            updateUser.Email = objUser.Email;
                            updateUser.ContactNumber = objUser.ContactNumber;
                            updateUser.MotherCardNumber = objUser.MotherCardNumber;
                            updateUser.Status = objUser.Status;

                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. User not found in the server.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to update users.");
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
