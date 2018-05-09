using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Data.Contracts.Services;
using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]/[action]")]
    public class UploadsController : BaseController
    {

        readonly ILogger<UploadsController> _logger;
        readonly IEmployeeService _employeeService;
        readonly ICustomerService _customerService;
        readonly IDepositService _depositService;

        public UploadsController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UploadsController> logger,
            IMapper mapper,
            IGenerator generator,
            IEmployeeService employeeService,
            ICustomerService customerService,
            IDepositService depositService
             ) : base(userManager, roleManager, mapper, generator)
             {
                _logger = logger;
                _employeeService = employeeService;
                _customerService = customerService;
                _depositService = depositService;   
             }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Process(int table, IFormFile csv)
        {
            try
            {
                if (csv == null || csv.Length == 0)
                {
                    TempData["Error"] = "File not selected";
                    return RedirectToAction("Index");
                }

                var fileSplit = csv.FileName.Split('.');
                var fileExtension = fileSplit.Last();
                var newFileName = _generator.DateCodeString() + Guid.NewGuid() + "." + fileExtension;
                var fileSize = csv.Length;
                var csvFile = fileExtension.Equals("csv");

                if (!csvFile)
                {
                    TempData["Error"] = "Only csv file is accepted";
                    return RedirectToAction("Index");
                }

                var path = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot/uploads/csv",
                    newFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await csv.CopyToAsync(stream);
                }

                if (table == 1)
                {
                    await Users(path);
                }
                else
                {
                    Savings(path);
                }

                TempData["Message"] = "Uploaded file has been sent to the processing server";
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        private async Task<IActionResult> Users(string fileName)
        {
            try
            {
                var csvData = System.IO.File.ReadAllText(fileName);
                var users = (from row in csvData.Split('\n')
                             where !string.IsNullOrEmpty(row)
                             select new UserModel()
                             {
                                 UserCode = (row.Split(',')[0] == "1") ? UserCode.Employee : UserCode.Customer,
                                 GenderCode = (row.Split(',')[1] == "1") ? GenderCode.Male : GenderCode.Female,
                                 FirstName = row.Split(',')[2],
                                 LastName = row.Split(',')[3],
                                 Email = row.Split(',')[4],
                                 PhoneNumber = row.Split(',')[5]
                             }).ToList();

                foreach (var item in users)
                {
                    await ProcessUsers(item);
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        private async Task<IActionResult> ProcessUsers(UserModel model)
        {
            try
            {
                var userName = await GenerateUserName();
                var uniqueNumber = _generator.DateCodeString() + userName;
                var user = new ApplicationUser
                {
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
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        private IActionResult Savings(string fileName)
        {
            try
            {
                var csvData = System.IO.File.ReadAllText(fileName);
                var deposits = (from row in csvData.Split('\n')
                                where !string.IsNullOrEmpty(row)
                                select new DepositModel()
                                {
                                    EmployeeId = GetEmployeeId().Result,
                                    CustomerId = GetCustomerId(row.Split(',')[0]),
                                    Amount = Convert.ToInt32(row.Split(',')[1]),
                                    TransactionCode = TransactionCode.Deposit,
                                    Reference = _generator.DateCodeString() + _generator.RandomNumber(1111111, 7777777),
                                    DateCreated = DateTime.UtcNow
                                }).ToList();

                foreach (var item in deposits)
                {
                    ProcessSavings(item);
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        private async Task<string> GetEmployeeId()
        {
            var currentUser = await CurrentUser();
            var employee = _employeeService.List().FirstOrDefault(x => x.AspNetUsersId == currentUser.Id);
            var employeeId = string.Empty;

            if (employee != null )
            {
                employeeId = employee.EmployeeId;
            }

            return employeeId;

        }

        private string GetCustomerId(string id)
        {

            var user = _userManager.Users.FirstOrDefault(x => x.UniqueNumber == id);
            if(user == null)
            {
                return string.Empty;
            }

            var customer = _customerService.List().FirstOrDefault(x => x.AspNetUsersId == user.Id);

            if(customer == null)
            {
                return string.Empty;
            }


            return customer.CustomerId;
        }

        private IActionResult ProcessSavings(DepositModel model)
        {

            try
            {
                if (model.CustomerId != null)
                {
                    _depositService.Create(model);

                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }
    }
}