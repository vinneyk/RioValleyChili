using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using RioValleyChili.Client.Mvc.Content;
using RioValleyChili.Client.Mvc.Core.Filters;
using RioValleyChili.Client.Mvc.Extensions;
using RioValleyChili.Client.Mvc.Models;
using RioValleyChili.Services.Interfaces;
using Solutionhead.Services;
using ClaimTypes = RioValleyChili.Client.Mvc.Core.Security.ClaimTypes;

namespace RioValleyChili.Client.Mvc.Controllers
{

    [BypassKillSwitchFilter]
    public partial class AccountController : RvcControllerBase
    {
        #region constructors

        private readonly IEmployeesService _employeesService;

        public AccountController(IEmployeesService employeesService)
        {
            if (employeesService == null) throw new ArgumentNullException("employeesService");
            _employeesService = employeesService;
        }

        #endregion

        // GET: /Account/Login

        [System.Web.Mvc.AllowAnonymous]
        public virtual ActionResult Login()
        {
            return ContextDependentView();
        }

        // POST: /Account/JsonLogin

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
        public virtual JsonResult JsonLogin(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    return Json(new { success = true, redirect = returnUrl });
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }

        //
        // POST: /Account/Login

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
        public virtual async Task<ActionResult> Login(LoginModel model, [FromUri]string returnUrl)
        {
            if(ModelState.IsValid && Membership.ValidateUser(model.UserName, model.Password))
            {
                var getEmployeeResult = await Task.Run(() => _employeesService.GetEmployeeDetailsByUserName(model.UserName));
                if (getEmployeeResult.State == ResultState.Invalid)
                {
                    ModelState.AddModelError("", UserMessages.UnauthorizedUser);
                    return View(model);
                }
                getEmployeeResult.EnsureSuccessWithHttpResponseException();
                
                var employee = getEmployeeResult.ResultingObject;
                if (!employee.IsActive)
                {
                    ModelState.AddModelError("", UserMessages.InactiveEmployeeAccount);
                    return View(model);
                }

                var claims = new List<Claim>();
                claims.Add(new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, employee.UserName));
                claims.Add(new Claim(System.Security.Claims.ClaimTypes.Name, employee.DisplayName));
                claims.Add(new Claim(System.Security.Claims.ClaimTypes.Email, employee.EmailAddress));
                claims.Add(new Claim("EmployeeId", employee.EmployeeKey));
                claims.Add(new Claim(ClaimTypes.SessionToken, Guid.NewGuid().ToString()));
                claims.Add(new Claim(ClaimTypes.UserToken, employee.UserName));

                claims.AddRange(employee.Claims.Select(claim => new Claim(claim.Key, claim.Value)));
                
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Forms"));
                var authManager = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager;
                authManager.Authenticate(string.Empty, claimsPrincipal);

                return Url.IsLocalUrl(returnUrl) 
                    ? Redirect(returnUrl) as ActionResult
                    : RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login.");
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public virtual ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            FederatedAuthentication.SessionAuthenticationModule.SignOut();

            return RedirectToAction("Index", "Home");
        }
        
        //
        // GET: /Account/ChangePassword

        public virtual ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [System.Web.Mvc.HttpPost]
        public virtual ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, userIsOnline: true);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public virtual ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        private ActionResult ContextDependentView()
        {
            string actionName = ControllerContext.RouteData.GetRequiredString("action");
            if (Request.QueryString["content"] != null)
            {
                ViewBag.FormAction = "Json" + actionName;
                return PartialView();
            }
            else
            {
                ViewBag.FormAction = actionName;
                return View();
            }
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }
    }
}
