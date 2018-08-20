﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace sodex_api_v2.Controllers
{
    [Authorize, RoutePrefix("api/profile")]
    public class ProfileController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.SodexDatabaseDataContext db = new Data.SodexDatabaseDataContext();

        // ================
        // Detail - Profile
        // ================
        [HttpGet, Route("detail")]
        public Models.MstUser DetailProfile()
        {
            var currentUser = from d in db.MstUsers
                              where d.AspNetUserId == User.Identity.GetUserId()
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
                                  Balance = d.Balance
                              };

            return currentUser.FirstOrDefault();
        }

        // ================
        // Update - Profile
        // ================
        [HttpPut, Route("update")]
        public HttpResponseMessage UpdateProfile(Models.MstUser objUser)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.AspNetUserId == User.Identity.GetUserId()
                                  select d;

                if (currentUser.Any())
                {
                    var updateCurrentUser = currentUser.FirstOrDefault();
                    updateCurrentUser.FullName = objUser.FullName;
                    updateCurrentUser.Address = objUser.Address;
                    updateCurrentUser.Email = objUser.Email;
                    updateCurrentUser.ContactNumber = objUser.ContactNumber;
                    updateCurrentUser.MotherCardNumber = objUser.MotherCardNumber;
                    updateCurrentUser.Balance = objUser.Balance;

                    db.SubmitChanges();

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. Your profile was not found in the server.");
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
