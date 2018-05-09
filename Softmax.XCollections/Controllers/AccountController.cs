using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Data.Contracts.Services;
using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Models;
using Softmax.XCollections.Models.AccountViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : BaseController
    {
        readonly SignInManager<ApplicationUser> _signInManager;
        readonly ILogger<AccountController> _logger;
        readonly IMessager _messager;
        readonly IBranchService _branchService;
        readonly IEmployeeService _employeeService;
        readonly ICustomerService _customerService;
        readonly IDepositService _depositService;
        readonly ILoanService _loanService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IMessager messager,
            IGenerator generator,
            IMapper mapper,
            IBranchService branchService,
            IEmployeeService employeeService,
            ICustomerService customerService,
            IDepositService depositService,
            ILoanService loanService
            ) : base(
            userManager,
            roleManager,
            mapper,
            generator)
        {
            _signInManager = signInManager;
            _logger = logger;
            _messager = messager;
            _branchService = branchService;
            _employeeService = employeeService;
            _customerService = customerService;
            _depositService = depositService;
            _loanService = loanService;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_asc" : "";
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

           var users = _userManager.Users.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s =>s.FirstName.Contains(searchString)
                || s.LastName.Contains(searchString)
                || s.UniqueNumber.Contains(searchString)
                || s.MobileNumber.Contains(searchString)
                || s.Email.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_asc":
                    users = users.OrderBy(f => f.FirstName);
                    break;
                case "date_asc":
                    users = users.OrderBy(s => s.DateCreated);
                    break;
                default:
                    users = users.OrderByDescending(s => s.DateCreated);
                    break;
            }

            return View(PaginatedList<ApplicationUser>.Create((IEnumerable<ApplicationUser>)users, page ?? 1, 10));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Register(string returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewBagData();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message + " " + ex.InnerException);
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserModel model, string returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewBagData();
                if (!ModelState.IsValid) return View(model);
                var userName = await GenerateUserName();
                var uniqueNumber = _generator.DateCodeString() + userName;
                var user = new ApplicationUser
                {
                    BranchId = model.BranchId,
                    UniqueNumber = uniqueNumber,
                    UserCode = model.UserCode,
                    GenderCode = model.GenderCode,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    UserName = userName,
                    DateCreated = DateTime.UtcNow,
                    IsTempPassword = true,
                    IsActive = true
                };
                var result = await _userManager.CreateAsync(user, "Password@1");
                if (result.Succeeded)
                {
                    if (model.UserCode == UserCode.Employee)
                    {
                        var employee = new EmployeeModel()
                        {
                            AspNetUsersId = user.Id
                        };
                        _employeeService.Create(employee);
                    }
                    else
                    {
                        var customer = new CustomerModel()
                        {
                            AspNetUsersId = user.Id
                        };
                        _customerService.Create(customer);
                    }

                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
           

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public IActionResult Profile()
        {
            try
            {
                ViewBagData();
                var userId = CurrentUser().Result.Id;
                var model = GetUsers().FirstOrDefault(x => x.Id == userId);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }

            return Content("Internal server error please contact the system adminstrator");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            //ViewBagData();
            if (!ModelState.IsValid) return View(model);

            var user = _userManager.Users.FirstOrDefault(x => x.Id == model.Id);
            if (user == null) return View(model);

            user.BranchId = model.BranchId;
            user.UserCode = model.UserCode;
            user.GenderCode = model.GenderCode;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.OtherName = model.OtherName;
            user.PhoneNumber = model.PhoneNumber;
            user.MobileNumber = model.MobileNumber;
            user.Address = model.Address;
            user.NearestBusStop = model.NearestBusStop;
            user.Occupation = model.Occupation;
            user.IsActive = model.IsActive;

            try
            {
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string id)
        {
            try
            {
                ViewBagData();
                var model = GetUsers().FirstOrDefault(x => x.Id == id);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }

            return Content("Internal server error please contact the system administrator");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserModel model, string returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
            ViewBagData();
            if (!ModelState.IsValid) return View(model);

            var user = _userManager.Users.FirstOrDefault(x => x.Id == model.Id);
            if (user == null) return View(model);

            user.BranchId = model.BranchId;
            user.UserCode = model.UserCode;
            user.GenderCode = model.GenderCode;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.OtherName = model.OtherName;
            user.PhoneNumber = model.PhoneNumber;
            user.MobileNumber = model.MobileNumber;
            user.Address = model.Address;
            user.NearestBusStop = model.NearestBusStop;
            user.Occupation = model.Occupation;
            user.IsActive = model.IsActive;

                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + " " + e.InnerException);
            }

            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Roles(string id)
        {
            try
            {
                if (id == null)
                {
                    TempData["Error"] = "Id can not null";
                    return RedirectToAction("Index");
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "No user record found";
                    return RedirectToAction("Index");
                }
                var userRoles = await _userManager.GetRolesAsync(user);
                var userModel = GetUsers().FirstOrDefault(x => x.Id == id);
                if (userModel != null)
                {
                    var model = new UserRoleViewModel
                    {
                        Id = user.Id,
                        Name = userModel.FirstName + " " + userModel.LastName,
                        RolesList = _roleManager.Roles.ToList().Select(x => new SelectListItem
                        {
                            Selected = userRoles.Contains(x.Name),
                            Text = x.Name,
                            Value = x.Name
                        })
                    };

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return View(id);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Roles(UserRoleViewModel editUserRole, params string[] selectedRole)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(editUserRole.Id);
                    if (user == null)
                        return BadRequest("no record found");


                    var userRoles = await _userManager.GetRolesAsync(user);

                    selectedRole = selectedRole ?? new string[] { };

                    var result = await _userManager.AddToRolesAsync(user, selectedRole.Except(userRoles).ToArray());

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First().ToString());
                        return View();
                    }
                    result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRole).ToArray());

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First().ToString());
                        return View();
                    }
                    return RedirectToAction("Index", "Account");
                }
                ModelState.AddModelError("", "Something failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Details(string id)
        {
            try
            {
                var model = GetUsers().FirstOrDefault(x => x.Id == id);

                if (model != null)
                {
                    var customer = _customerService.List().FirstOrDefault(x => x.AspNetUsersId == model.Id);
                    if (customer != null)
                    {
                        var customerId = customer.CustomerId;
                        ViewBag.Deposits = _depositService.List().Where(x => x.CustomerId == customerId && x.Amount > 0).OrderByDescending(x=>x.DateCreated).Take(25);
                        ViewBag.Loans = _loanService.List().Where(x => x.CustomerId == customerId && x.Amount > 0).OrderByDescending(x => x.DateCreated).Take(10);
                    }

                }
                return View(model); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);
            var result =
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                var user = 
                _userManager.Users.FirstOrDefault(x => x.UserName == model.Email);
                return user != null && user.IsTempPassword ? RedirectToAction("ChangePassword") : RedirectToLocal(returnUrl);
            }
             
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
           _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(string Id, IFormFile photo)
        {
            try
            {
                var userId = Id;
                if (photo == null || photo.Length == 0)
                {
                    TempData["Error"] = "File not selected";
                    return RedirectToAction("Edit", "Account", new { id = userId });
                }

                var fileSplit = photo.FileName.Split('.');
                var fileExtension = fileSplit.Last();
                var newFileName = Guid.NewGuid() + "." + fileExtension;
                var fileSize = photo.Length;
                var jpg = fileExtension.Equals("jpeg");
                var png = fileExtension.Equals("png");

                if (!jpg && !png)
                {
                    TempData["Error"] = "Only jpeg or png file is accepted";
                    return RedirectToAction("Edit", "Account", new { id = userId });
                }


                if (fileSize > 5000)
                {
                    TempData["Error"] = "Maximum of 5KB file is allowed";
                    return RedirectToAction("Edit", "Account", new { id = userId });
                }

                var path = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot/uploads/photos",
                    newFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
                if (user != null)
                {
                    user.Photo = newFileName;
                    await _userManager.UpdateAsync(user);
                }

                return RedirectToAction("Edit", "Account", new { id = userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ViewBag.Error = "Password reset failed";
                    return View(model);
                }

                var tempPassword = _generator.TempPassword(7);
                var passwordHasher = _userManager.PasswordHasher.HashPassword(user, tempPassword);
                user.PasswordHash = passwordHasher;
                user.IsTempPassword = true;
                await _userManager.UpdateAsync(user);

                _messager.SendEmailResetPassword(user.Email, tempPassword);

                ViewBag.Success = "A temporary password has been sent to " + model.Email;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        public IActionResult ChangePassword(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ResetPasswordViewModel model, string returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["Message"] = "Error";

                if (!ModelState.IsValid) return View(model);
                if (model.NewPassword.ToLower().Contains("password")) return View(model);
                var identityUser = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(identityUser);
                var check = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!check) return View(model);
           
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPassword  = await _userManager.ResetPasswordAsync(user, token, model.ConfirmPassword);

                if (!resetPassword.Succeeded) return View(model);
                    user.IsTempPassword = false;
                    await _userManager.UpdateAsync(user);
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        public  IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers
        private void ViewBagData()
        {
            ViewBag.Branches =
                new SelectList(_branchService.List().Where(x=>x.IsActive), "BranchId", "Location", "--Select One--");

            ViewBag.GenderCodes =
                new SelectList(GetGenderCodes(), "Key", "Value", "--Select One--");

            ViewBag.UserCodes =
                new SelectList(GetUserCodes(), "Key", "Value", "--Select One--");

        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion
    }
}