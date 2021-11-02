using System;
using System.Web.Mvc;
using System.Web.Security;
using RioValleyChili.Client.Mvc.Core.Messaging;
using RioValleyChili.Client.Mvc.Core.Security;
using RioValleyChili.Client.Mvc.Models.Employees;
using RioValleyChili.Client.Mvc.Models.Security;
using RioValleyChili.Services.Interfaces;
using Solutionhead.Services;

namespace RioValleyChili.Client.Mvc.Controllers
{
    [ClaimsAuthorize(ClaimActions.View, ClaimTypes.Solutionhead)]
    public partial class SecurityController : RvcControllerBase
    {
        private readonly IEmployeesService _employeesService;

        public SecurityController(IEmployeesService employeesService)
        {
            if (employeesService == null) throw new ArgumentNullException("employeesService");
            _employeesService = employeesService;
        }

        //
        // GET: /Security

        public virtual ActionResult Index()
        {
            var employeesResult = _employeesService.GetEmployees();
            
            var model = new MembersViewModel
            {
                Members = employeesResult.ResultingObject
            };
            
            return View(model);
        }

        //
        // GET: /Security/Permissions/1

        public virtual ActionResult Permissions(string id)
        {
            var employeeDetailsResult = _employeesService.GetEmployeeDetailsByKey(id);
            if (!employeeDetailsResult.Success)
            {
                ViewBag.Message = "Employee not found";
                MessengerService.SetMessage(MessageType.Informational, "Employee not found");
                return RedirectToAction(MVC.Security.Index());
            }

            var employeeDetails = employeeDetailsResult.ResultingObject;
            var model = new MemberPermissions
            {
                EmployeeKey = employeeDetails.EmployeeKey,
                EmployeeName = employeeDetails.DisplayName,
                Claims = employeeDetails.Claims
            };
            return View(model);
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public virtual ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost, AllowAnonymous]
        public virtual ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(model.EmployeeKey))
                {
                    var getEmployeeResult = _employeesService.GetEmployeeDetailsByUserName(model.UserName);
                    if (getEmployeeResult.State == ResultState.Invalid)
                    {
                        ModelState.AddModelError("",
                            string.Format("Employee with username \"{0}\" was not found.", model.UserName));
                        return View(MVC.Security.Views.Register, model);
                    }

                    if (!getEmployeeResult.ResultingObject.EmployeeKey.Equals(model.EmployeeKey, StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("", string.Format("Employee ID and Username do not match."));
                        return View(MVC.Security.Views.Register, model);
                    }
                    if (!getEmployeeResult.Success) { throw new ApplicationException(string.Format("Unable to get employee data. Message: \"{0}\"", getEmployeeResult.Message)); }

                    var updateEmployeeParam = new ActivateEmployeeParameters
                    {
                        EmployeeKey = model.EmployeeKey,
                        EmailAddress = model.Email,
                    };
                    var result = _employeesService.ActivateEmployee(updateEmployeeParam);
                    if (!result.Success)
                    {
                        ModelState.AddModelError("", "Unable to update employee information.");
                        return View(MVC.Security.Views.Register, model);
                    }
                }
                else
                {
                    var createEmployeeResult = _employeesService.CreateEmployee(new CreateEmployeeParameters
                    {
                        UserName = model.UserName,
                        EmailAddress = model.Email,
                    });
                    if (!createEmployeeResult.Success)
                    {
                        ModelState.AddModelError("", "Unable to create new employee.");
                        return View(MVC.Security.Views.Register, model);
                    }
                }

                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    MessengerService.SetMessage(MessageType.Informational, "Employee membership created successfully.");
                    ViewBag.AsActionMessage = "Employee membership created successfully.";
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
