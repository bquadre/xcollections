using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Data.Contracts.Services;
using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Models;
using Softmax.XCollections.Models.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Softmax.XCollections.Controllers
{
    [Authorize]
    public class SavingsController : BaseController
    {
        readonly ILogger<SavingsController> _logger;
        readonly IEmployeeService _employeeService;
        readonly ICustomerService _customerService;
        readonly IDepositService _depositService;
        readonly IDepositConfirmService _depositConfirmService;
        readonly IProductService _productService;
        readonly FeeSettingsModel _feeSettings;

        public SavingsController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<SavingsController> logger,
            IMapper mapper,
            IGenerator generator,
            IProductService productService,
            IEmployeeService employeeService,
            ICustomerService customerService,
            IDepositService depositService,
            IDepositConfirmService depositConfirmService,
            IOptions<FeeSettingsModel> feeSettings
            ) : base(userManager, roleManager, mapper, generator)
            {
                _logger = logger;
                _depositService = depositService;
                _depositConfirmService = depositConfirmService;
                _employeeService = employeeService;
                _customerService = customerService;
                _productService = productService;
                _feeSettings = feeSettings.Value;
            }

        [HttpGet]
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["AccountSortParm"] = String.IsNullOrEmpty(sortOrder) ? "account_desc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["BalanceSortParm"] = String.IsNullOrEmpty(sortOrder) ? "balance_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var deposits = _depositService.List().Where(x => x.Amount > 0).ToList();
            var savingsGroupedResult = new List<GroupedDepositModel>();

            if (deposits.Count > 0)
            {
                savingsGroupedResult = deposits.OrderByDescending(x => x.DateCreated).OrderBy(x => x.Customer.AspNetUsers.FirstName).GroupBy(emp => emp.CustomerId)
                    .Select(y => new GroupedDepositModel()
                    {
                        UniqueNumber = y.First().Customer.AspNetUsers.UniqueNumber,
                        Name = y.First().Customer.AspNetUsers.FirstName + " "+ y.First().Customer.AspNetUsers.OtherName + " " + y.First().Customer.AspNetUsers.LastName,
                        Balance = y.First().Balance,
                        CustomerId = y.First().CustomerId
                    }).ToList<GroupedDepositModel>();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                savingsGroupedResult = savingsGroupedResult.Where(s => s.Name.ToLower().Contains(searchString.ToLower())
                || s.UniqueNumber.Contains(searchString)
                || s.Balance.ToString().Contains(searchString)).ToList();
            }

            switch (sortOrder)
            {
                case "account_desc":
                    savingsGroupedResult = savingsGroupedResult.OrderByDescending(s => s.UniqueNumber).ToList();
                    break;
                case "name_desc":
                    savingsGroupedResult = savingsGroupedResult.OrderByDescending(s => s.Name).ToList();
                    break;
                case "balance_desc":
                    savingsGroupedResult = savingsGroupedResult.OrderByDescending(s => s.Balance).ToList();
                    break;
                default:
                    savingsGroupedResult = savingsGroupedResult.OrderBy(s => s.Name).ToList();
                    break;
            }

            ViewData["RecordCount"] = savingsGroupedResult.Count;
            return View(PaginatedList<GroupedDepositModel>.Create((IEnumerable<GroupedDepositModel>)savingsGroupedResult, page ?? 1, 10));
        }

        public IActionResult History(string id, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewData["AmountSortParm"] = String.IsNullOrEmpty(sortOrder) ? "amount_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSortOrder"] = sortOrder;
            ViewData["CustomerId"] = id;

            var deposits = _depositService.List().Where(x => x.CustomerId == id);
            if (!String.IsNullOrEmpty(searchString))
            {
                deposits = deposits.Where(s => s.Amount.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date_asc":
                    deposits = deposits.OrderBy(s => s.DateCreated);
                    break;
                case "amount_desc":
                    deposits = deposits.OrderByDescending(s => s.Amount);
                    break;
                default:
                    deposits = deposits.OrderByDescending(s => s.DateCreated);
                    break;
            }


            ViewData["RecordCount"] = deposits.ToList().Count;
            ViewDataCustomerInfo(id);
            
            return View(PaginatedList<DepositModel>.Create((IEnumerable<DepositModel>)deposits, page ?? 1, 10));
        }

        public IActionResult ConfirmationRequest(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewData["AmountSortParm"] = String.IsNullOrEmpty(sortOrder) ? "amount_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSortOrder"] = sortOrder;
         

            var deposits = _depositService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Pending);
            if (!String.IsNullOrEmpty(searchString))
            {
                deposits = deposits.Where(s => s.Amount.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date_asc":
                    deposits = deposits.OrderBy(s => s.DateCreated);
                    break;
                case "amount_desc":
                    deposits = deposits.OrderByDescending(s => s.Amount);
                    break;
                default:
                    deposits = deposits.OrderByDescending(s => s.DateCreated);
                    break;
            }


            ViewData["RecordCount"] = deposits.ToList().Count;

            return View(PaginatedList<DepositModel>.Create((IEnumerable<DepositModel>)deposits, page ?? 1, 10));
        }

        public IActionResult Insights(string id)
        {
            var deposits = _depositService.List().Where(x => x.CustomerId == id);

            var insight = new DepositInsightModel
            {
                Account = deposits?.First().Customer.AspNetUsers?.UniqueNumber,
                Name = deposits?.First().Customer.AspNetUsers?.FirstName + " " + deposits?.First().Customer.AspNetUsers?.OtherName + " " + deposits?.First().Customer.AspNetUsers?.LastName,
                CustomerId = deposits?.First().CustomerId,
                Balance = deposits?.OrderByDescending(x => x.DateCreated).First().Balance.ToString("C", new CultureInfo("LA-LATN-NG")),
                LastDeposit = deposits?.OrderByDescending(x => x.DateCreated).FirstOrDefault(x => x.TransactionCode == TransactionCode.Deposit && x.StatusCode == Data.Enums.StatusCode.Confirmed).DateCreated.ToString(),
                LastWithdrawal = deposits?.OrderByDescending(x => x.DateCreated).FirstOrDefault(x => x.TransactionCode == TransactionCode.Withdrawal && x.StatusCode == Data.Enums.StatusCode.Confirmed).DateCreated.ToString(),
                LastDepositAmount = deposits?.OrderByDescending(x => x.DateCreated).FirstOrDefault(x => x.TransactionCode == TransactionCode.Deposit && x.StatusCode == Data.Enums.StatusCode.Confirmed).Amount.ToString("C", new CultureInfo("LA-LATN-NG")),
                LastWithdrawalAmount = deposits?.OrderByDescending(x => x.DateCreated).FirstOrDefault(x => x.TransactionCode == TransactionCode.Withdrawal && x.StatusCode == Data.Enums.StatusCode.Confirmed).Amount.ToString("C", new CultureInfo("LA-LATN-NG")),
                Deposits = deposits.Where(x => x.TransactionCode == TransactionCode.Deposit && x.StatusCode == Data.Enums.StatusCode.Confirmed).Count(),
                Withdrawals = deposits.Where(x => x.TransactionCode == TransactionCode.Withdrawal && x.StatusCode == Data.Enums.StatusCode.Confirmed).Count(),
                TotalDeposits = deposits.Where(x => x.TransactionCode == TransactionCode.Deposit && x.StatusCode == Data.Enums.StatusCode.Confirmed).Sum(x=>x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                TotalWithdrawals = deposits.Where(x => x.TransactionCode == TransactionCode.Withdrawal && x.StatusCode == Data.Enums.StatusCode.Confirmed).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                TotalFees = deposits.Where(x => x.TransactionCode == TransactionCode.Fee).Sum(x=>x.Amount).ToString("C", new CultureInfo("HA-LATN-NG"))
            };

            return View(insight);
        }

        [Authorize(Roles = "Officer, Supervisor")]
        public IActionResult Create()
        {
            try
            {
                ViewBagData();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }
        [HttpGet]
        [Authorize(Roles = "Manager, Admin")]
        public IActionResult Confirmation(string sortOrder, string currentFilter, string searchString, int? page)
        {
            
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_asc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var confirmations = _depositConfirmService.List();
            if (!String.IsNullOrEmpty(searchString))
            {
                confirmations = confirmations.Where(s => s.Customer.AspNetUsers.FirstName.Contains(searchString)
                || s.Customer.AspNetUsers.UniqueNumber.Contains(searchString)
                || s.Employee.AspNetUsers.FirstName.Contains(searchString)
                || s.Deposit.Amount.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "date_asc":
                    confirmations = confirmations.OrderBy(s => s.DateCreated);
                    break;
                case "name_asc":
                    confirmations = confirmations.OrderBy(s => s.Employee.AspNetUsers.FirstName);
                    break;
                default:
                    confirmations = confirmations.OrderByDescending(s => s.DateCreated);
                    break;
            }
            ViewData["RecordCount"] = confirmations.ToList().Count;
            return View(PaginatedList<DepositConfirmModel>.Create((IEnumerable<DepositConfirmModel>)confirmations, page ?? 1, 10));

        }

      

       

        [Authorize(Roles = "Supervisor, Manager, Admin")]
        public IActionResult Confirm(string id)
        {
            try
            {
                var request = _depositService.List().Single(x => x.DepositId == id);
                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");

        }

        [Authorize(Roles = "Supervisor, Manager, Admin")]
        public IActionResult Decline(string id)
        {
            try
            {
                var request = _depositService.List().Single(x => x.DepositId == id);
                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpPost]
        [Authorize(Roles = "Officer, Supervisor")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DepositModel model)
        {
            try
            {
                ViewBagData();

                if (!ModelState.IsValid) return View(model);
                model.EmployeeId = GetEmployeeId().Result;
                model.CustomerId = GetCustomerId(model.CustomerId);
                model.Reference = _generator.DateCodeString() + _generator.RandomNumber(1111111, 7777777);
                model.TransactionCode = model.TransactionCode == (TransactionCode)1
                    ? TransactionCode.Deposit
                    : TransactionCode.Withdrawal;

                var request = _depositService.Create(model);
                if (request.Successful)
                    return RedirectToAction("History", "Savings", new {id = model.CustomerId});

                if (request.ResultType == ResultType.InsufficientBalance)
                    TempData["Error"] = "Insufficient Balance";

                if (request.ResultType == ResultType.PendingTransaction)
                    TempData["Error"] = "This custormer has a pending transaction";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }

            return View(model);
            
        }

        [HttpPost]
        [ActionName("Confirm")]
        [ValidateAntiForgeryToken]
        public IActionResult SavingConfirmed(string id)
        {
            try
            {
                var deposit = _depositService.List().Single(x => x.DepositId == id);
                if (deposit != null)
                {
                    var lastCustomerTransaction = _depositService.CustomerLastTransaction(deposit);
                    var lastCustomerConfirmedTransaction = _depositService.CustomerLastConfirmedTransaction(deposit);
                    switch (deposit.TransactionCode)
                    {
                        case TransactionCode.Deposit:
                            deposit.Balance = lastCustomerConfirmedTransaction.Balance + deposit.Amount;
                            break;
                        case TransactionCode.Withdrawal:
                            deposit.Balance = lastCustomerConfirmedTransaction.Balance - deposit.Amount;
                            
                            break;
                    }

                    deposit.StatusCode = Data.Enums.StatusCode.Confirmed;
                    _depositService.Update(deposit);
                    

                    _depositConfirmService.Create(new DepositConfirmModel()
                    {
                        DepositId = id,
                        EmployeeId = GetEmployeeId().Result,
                        CustomerId = deposit.CustomerId
                    });

                    if(deposit.TransactionCode == TransactionCode.Withdrawal)
                    {
                        var withdrawalFee = Convert.ToInt32(_feeSettings.Withdrawal);
                        var balance = deposit.Balance - withdrawalFee;
                        var feemodel = new DepositModel
                        {
                            CustomerId = deposit.CustomerId,
                            EmployeeId = GetEmployeeId().Result,
                            ProductId = deposit.ProductId,
                            TransactionCode = TransactionCode.Fee,
                            StatusCode = Data.Enums.StatusCode.Confirmed,
                            Amount = withdrawalFee,
                            Balance = balance,
                            DateCreated = DateTime.UtcNow,
                        };

                        _depositService.Create(feemodel);
                    }
                }

                return RedirectToAction("Confirmation");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpPost]
        [ActionName("Decline")]
        [ValidateAntiForgeryToken]
        public IActionResult SavingDeclined(string id)
        {
            try
            {

                var deposit = _depositService.List().Single(x => x.DepositId == id);
                if (deposit != null)
                {
                    deposit.StatusCode = Data.Enums.StatusCode.Declined;
                    _depositService.Update(deposit);

                    _depositConfirmService.Create(new DepositConfirmModel()
                    {
                        DepositId = id,
                        EmployeeId = GetEmployeeId().Result,
                        CustomerId = deposit.CustomerId
                    });
                }
                return RedirectToAction("Confirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        private void ViewBagData()
        {
          
            ViewBag.products =
                new SelectList(GetSavingsProducts(), "ProductId", "Name", "--Select One--");

            ViewBag.Customers =
                new SelectList(GetUsers().Where(x=>x.UserCode == UserCode.Customer), "Id", "UniqueNumber", "--Select One--");

            ViewBag.TransactionCodes =
                new SelectList(GetTransactionCodes(), "Key", "Value", "--Select One--");
        }

        private void ViewDataCustomerInfo(string id)
        {
            var customerInfo = _customerService.List().FirstOrDefault(x => x.CustomerId == id);
            if (customerInfo != null)
            {
                ViewData["Name"] = customerInfo.AspNetUsers?.FirstName + " " + customerInfo.AspNetUsers?.OtherName + " " + customerInfo.AspNetUsers?.LastName;
                ViewData["Account"] = customerInfo.AspNetUsers?.UniqueNumber;
            }
        }
        private List<ProductModel> GetSavingsProducts()
        {
            var result = _productService.List()
                .Where(x => x.ProductCode == ProductCode.Savings && x.IsDeleted == false).ToList();
            return result;
        }
        private Dictionary<int, string> GetTransactionCodes()
        {
            var transactionCodes = new Dictionary<int, string>();
            transactionCodes.Add((int)TransactionCode.Deposit, "Deposits");
            transactionCodes.Add((int)TransactionCode.Withdrawal, "Withdrawals");

            return transactionCodes;
        }
        private  async Task<List<DepositModel>> GetDeposits()
        {
            var currentUser = await CurrentUser();
            var officer = await IsOfficer();
            var employee = _employeeService.List().FirstOrDefault(x => x.AspNetUsersId == currentUser.Id);
            var employeeId = string.Empty;

            if (employee != null)
            {
                employeeId = employee.EmployeeId;
            }

            var deposits = _depositService.List();
            return officer ? deposits.Where(x => x.EmployeeId.Equals(employeeId)).ToList() : deposits.ToList();
        }
        private async Task<string> GetEmployeeId()
        {
            var currentUser = await CurrentUser();
            var employee = _employeeService.List().FirstOrDefault(x => x.AspNetUsersId == currentUser.Id);
            var employeeId = string.Empty;

            if (employee != null)
            {
                employeeId = employee.EmployeeId;
            }

            return employeeId;

        }
        private string GetCustomerId(string id)
        {

            var user = _userManager.Users.FirstOrDefault(x => x.UniqueNumber == id);
            if (user == null)
            {
                return string.Empty;
            }

            var customer = _customerService.List().FirstOrDefault(x => x.AspNetUsersId == user.Id);

            if (customer == null)
            {
                return string.Empty;
            }


            return customer.CustomerId;
        }
    }
}
