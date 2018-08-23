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
    [Authorize, RoutePrefix("api/userForm")]
    public class UserFormController : ApiController
    {
        // ============
        // Data Context
        // ============
        private Data.SodexDatabaseDataContext db = new Data.SodexDatabaseDataContext();

        // ========================
        // List - Current User Form
        // ========================
        [HttpGet, Route("current/form/{form}")]
        public Models.MstUserForm CurrentUserForm(String form)
        {
            var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;
            if (currentUser.Any())
            {
                var userForms = from d in db.MstUserForms.OrderByDescending(d => d.Id)
                                where d.UserId == currentUser.FirstOrDefault().Id
                                && d.SysForm.Form.Equals(form)
                                select new Models.MstUserForm
                                {
                                    Id = d.Id,
                                    UserId = d.UserId,
                                    FormId = d.FormId,
                                    Form = d.SysForm.Form,
                                    Particulars = d.SysForm.Particulars,
                                    CanAdd = d.CanAdd,
                                    CanEdit = d.CanEdit,
                                    CanUpdate = d.CanUpdate,
                                    CanDelete = d.CanDelete
                                };

                return userForms.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        // ===========
        // List - Form
        // ===========
        [HttpGet, Route("forms/list")]
        public List<Models.SysForm> ListForms()
        {
            var forms = from d in db.SysForms.OrderByDescending(d => d.Id)
                        select new Models.SysForm
                        {
                            Id = d.Id,
                            Form = d.Form,
                            Particulars = d.Particulars
                        };

            return forms.ToList();
        }

        // ================
        // List - User Form
        // ================
        [HttpGet, Route("list/{userId}")]
        public List<Models.MstUserForm> ListUserForm(String userId)
        {
            var userForms = from d in db.MstUserForms.OrderByDescending(d => d.Id)
                            where d.UserId == Convert.ToInt32(userId)
                            select new Models.MstUserForm
                            {
                                Id = d.Id,
                                UserId = d.UserId,
                                FormId = d.FormId,
                                Form = d.SysForm.Form,
                                Particulars = d.SysForm.Particulars,
                                CanAdd = d.CanAdd,
                                CanEdit = d.CanEdit,
                                CanUpdate = d.CanUpdate,
                                CanDelete = d.CanDelete
                            };

            return userForms.ToList();
        }

        // ===============
        // Add - User Form
        // ===============
        [HttpPost, Route("add")]
        public HttpResponseMessage AddUserForm(Models.MstUserForm objUserForm)
        {
            try
            {
                var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;
                if (currentUser.Any())
                {
                    if (currentUser.FirstOrDefault().UserTypeId != 3)
                    {
                        Data.MstUserForm newUserForm = new Data.MstUserForm
                        {
                            UserId = objUserForm.UserId,
                            FormId = objUserForm.FormId,
                            CanAdd = objUserForm.CanAdd,
                            CanEdit = objUserForm.CanEdit,
                            CanUpdate = objUserForm.CanUpdate,
                            CanDelete = objUserForm.CanDelete
                        };

                        db.MstUserForms.InsertOnSubmit(newUserForm);
                        db.SubmitChanges();

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to add user forms.");
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

        // ==================
        // Update - User Form
        // ==================
        [HttpPut, Route("update/{id}")]
        public HttpResponseMessage UpdateUserForm(String id, Models.MstUserForm objUserForm)
        {
            try
            {
                var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;
                if (currentUser.Any())
                {
                    if (currentUser.FirstOrDefault().UserTypeId != 3)
                    {
                        var userForm = from d in db.MstUserForms where d.Id == Convert.ToInt32(id) select d;
                        if (userForm.Any())
                        {
                            var updateCurrentUserForm = userForm.FirstOrDefault();
                            updateCurrentUserForm.UserId = objUserForm.UserId;
                            updateCurrentUserForm.FormId = objUserForm.FormId;
                            updateCurrentUserForm.CanAdd = objUserForm.CanAdd;
                            updateCurrentUserForm.CanEdit = objUserForm.CanEdit;
                            updateCurrentUserForm.CanUpdate = objUserForm.CanUpdate;
                            updateCurrentUserForm.CanDelete = objUserForm.CanDelete;

                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. Your user form was not found in the server.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to update user forms.");
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

        // ==================
        // Delete - User Form
        // ==================
        [HttpDelete, Route("delete/{id}")]
        public HttpResponseMessage DeleteUserForm(String id)
        {
            try
            {
                var currentUser = from d in db.MstUsers where d.AspNetUserId == User.Identity.GetUserId() select d;
                if (currentUser.Any())
                {
                    if (currentUser.FirstOrDefault().UserTypeId != 3)
                    {
                        var userForm = from d in db.MstUserForms where d.Id == Convert.ToInt32(id) select d;
                        if (userForm.Any())
                        {
                            db.MstUserForms.DeleteOnSubmit(userForm.FirstOrDefault());
                            db.SubmitChanges();

                            return Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Sorry. Your user form was not found in the server.");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Sorry. You have no rights to delete user forms.");
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
