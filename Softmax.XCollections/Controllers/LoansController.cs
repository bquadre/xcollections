using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Data.Contracts.Services;
using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Softmax.XCollections.Controllers
{
    [Authorize]
    public class LoansController : BaseController
    {
        readonly ILogger<LoansController> _logger;
        readonly IEmployeeService _employeeService;
        readonly ICustomerService _customerService;
        readonly ILoanService _loanService;
        readonly ILoanApprovalService _loanapprovalService;
        readonly IRefundService _refundService;
        readonly IRefundConfirmService _refundconfirmService;
        readonly IProductService _productService;

        public LoansController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<LoansController> logger,
            IMapper mapper,
            IGenerator generator,
            IProductService productService,
            IEmployeeService employeeService,
            ICustomerService customerService,
            ILoanService loanService,
            ILoanApprovalService loanApprovalService,
            IRefundService refundService,
            IRefundConfirmService refundConfirmService

            ) : base(userManager, roleManager, mapper, generator)
        {
            _logger = logger;
            _employeeService = employeeService;
            _customerService = customerService;
            _loanService = loanService;
            _loanapprovalService = loanApprovalService;
            _refundService = refundService;
            _refundconfirmService = refundConfirmService;
            _productService = productService;
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

            var loans = _loanService.List().Where(x => x.Amount > 0).ToList();
            var loansGroupedResult = new List<GroupedDepositModel>();

            if (loans.Count > 0)
            {
                loansGroupedResult = loans.OrderByDescending(x => x.DateCreated).OrderBy(x => x.Customer.AspNetUsers.FirstName).GroupBy(emp => emp.CustomerId)
                    .Select(y => new GroupedDepositModel()
                    {
                        UniqueNumber = y.First().Customer.AspNetUsers.UniqueNumber,
                        Name = y.First().Customer.AspNetUsers.FirstName + " " + y.First().Customer.AspNetUsers.OtherName + " " + y.First().Customer.AspNetUsers.LastName,
                        Balance = y.Where(x => x.StatusCode == Data.Enums.StatusCode.Approved).Sum(x => x.Amount),
                        Count = y.Count(),
                        CustomerId = y.First().CustomerId
                    }).ToList<GroupedDepositModel>();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                loansGroupedResult = loansGroupedResult.Where(s => s.Name.ToLower().Contains(searchString.ToLower())
                || s.UniqueNumber.Contains(searchString)
                || s.Balance.ToString().Contains(searchString)).ToList();
            }

            switch (sortOrder)
            {
                case "account_desc":
                    loansGroupedResult = loansGroupedResult.OrderByDescending(s => s.UniqueNumber).ToList();
                    break;
                case "name_desc":
                    loansGroupedResult = loansGroupedResult.OrderByDescending(s => s.Name).ToList();
                    break;
                case "balance_desc":
                    loansGroupedResult = loansGroupedResult.OrderByDescending(s => s.Balance).ToList();
                    break;
                default:
                    loansGroupedResult = loansGroupedResult.OrderBy(s => s.Name).ToList();
                    break;
            }

            ViewData["RecordCount"] = loansGroupedResult.Count;
            return View(PaginatedList<GroupedDepositModel>.Create((IEnumerable<GroupedDepositModel>)loansGroupedResult, page ?? 1, 10));
        }

        [HttpGet]
        public IActionResult History(string id, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewData["CustomerSortParm"] = String.IsNullOrEmpty(sortOrder) ? "customer_asc" : "";
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

            var loans = _loanService.List().Where(x=>x.CustomerId == id);
            if (!String.IsNullOrEmpty(searchString))
            {
                loans = loans.Where(s => s.Customer.AspNetUsers.FirstName.ToLower().Contains(searchString.ToLower())
                                       || s.Employee.AspNetUsers.FirstName.ToLower().Contains(searchString.ToLower())
                                       || s.Customer.AspNetUsers.UniqueNumber.Contains(searchString)
                                       || s.Amount.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "date_asc":
                    loans = loans.OrderBy(s => s.DateCreated);
                    break;
                case "customer_asc":
                    loans = loans.OrderBy(s => s.Customer.AspNetUsers.FirstName);
                    break;
                case "amount_desc":
                    loans = loans.OrderByDescending(s => s.Amount);
                    break;
                default:
                    loans = loans.OrderByDescending(s => s.DateCreated);
                    break;
            }

            ViewData["RecordCount"] = loans.ToList().Count;
            ViewDataCustomerInfo(id);
            return View(PaginatedList<LoanModel>.Create((IEnumerable<LoanModel>)loans, page ?? 1, 10));
        }

        [HttpGet]
        public IActionResult Insights(string id)
        {
            var loans = _loanService.List().Where(x => x.CustomerId == id).ToList();
            var refunds = _refundService.List().Where(x => x.CustomerId == id).ToList();

            var insight = new LoanInsightModel();
            insight.Account = loans?.First().Customer.AspNetUsers?.UniqueNumber;
            insight.Name = loans?.First().Customer.AspNetUsers?.FirstName + " " + loans?.First().Customer.AspNetUsers?.OtherName + " " + loans?.First().Customer.AspNetUsers?.LastName;
            insight.CustomerId = loans?.First().CustomerId;
            insight.Loans = loans?.ToList().Count;
            insight.TotalLoans = loans?.Where(x => x.StatusCode == Data.Enums.StatusCode.Approved).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG"));
            insight.Refunds = refunds?.ToList().Count;
            insight.TotalRefunds = refunds?.Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG"));
            insight.LastLoan = loans?.OrderByDescending(x => x.DateCreated).FirstOrDefault(x => x.StatusCode == Data.Enums.StatusCode.Approved).DateCreated.ToString();
            insight.LastLoanAmount = loans?.OrderByDescending(x => x.DateCreated).FirstOrDefault(x => x.StatusCode == Data.Enums.StatusCode.Approved).Amount.ToString("C", new CultureInfo("HA-LATN-NG"));
            insight.LastRefund = refunds?.OrderByDescending(x => x.DateCreated).FirstOrDefault(x => x.StatusCode == Data.Enums.StatusCode.Confirmed).DateCreated.ToString();
            insight.LastRefundAmount = refunds?.OrderByDescending(x => x.DateCreated).FirstOrDefault(x => x.StatusCode == Data.Enums.StatusCode.Confirmed).Amount.ToString("C", new CultureInfo("HA-LATN-NG"));

            return View(insight);
        }

        [HttpGet]
        public IActionResult Refund(string id, string sortOrder, string currentFilter, string searchString, int? page)
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
            ViewData["LoanId"] = id;

            var refunds = _refundService.List().Where(x =>x.LoanId == id);
            if (!String.IsNullOrEmpty(searchString))
            {
                refunds = refunds.Where(s => s.Amount.ToString().Contains(searchString)
                || s.Loan.Employee.AspNetUsers.FirstName.ToLower().Contains(searchString.ToLower()));
            }
            switch (sortOrder)
            {
                case "date_asc":
                    refunds = refunds.OrderBy(s => s.DateCreated);
                    break;
                case "amount_desc":
                    refunds = refunds.OrderByDescending(s => s.Amount);
                    break;
                default:
                    refunds = refunds.OrderByDescending(s => s.DateCreated);
                    break;
            }
            ViewDataCustomerLoanInfo(id);
            ViewData["RecordCount"] = refunds.ToList().Count;
            return View(PaginatedList<RefundModel>.Create((IEnumerable<RefundModel>)refunds, page ?? 1, 10));
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            var loan = _loanService.List().FirstOrDefault(x => x.LoanId.Equals(id));
            return View(loan);
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Confirmation(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewData["EmployeeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "employee_asc" : "";
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

            var refundComfirmations = _refundconfirmService.List();
            if (!String.IsNullOrEmpty(searchString))
            {
                refundComfirmations = refundComfirmations.Where(s => s.Customer.AspNetUsers.FirstName.ToLower().Contains(searchString.ToLower())
                                       || s.Employee.AspNetUsers.FirstName.ToLower().Contains(searchString.ToLower())
                                       || s.Customer.AspNetUsers.UniqueNumber.Contains(searchString)
                                       || s.Refund.Amount.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "date_asc":
                    refundComfirmations = refundComfirmations.OrderBy(s => s.DateCreated);
                    break;
                case "customer_asc":
                    refundComfirmations = refundComfirmations.OrderBy(s => s.Employee.AspNetUsers.FirstName);
                    break;
                case "amount_desc":
                    refundComfirmations = refundComfirmations.OrderByDescending(s => s.Refund.Amount);
                    break;
                default:
                    refundComfirmations = refundComfirmations.OrderByDescending(s => s.DateCreated);
                    break;
            }

            return View(PaginatedList<RefundConfirmModel>.Create((IEnumerable<RefundConfirmModel>)refundComfirmations, page ?? 1, 10));
        }

        [Authorize(Roles = "Manager")]
        public IActionResult ApprovalRequest(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewData["CustomerSortParm"] = String.IsNullOrEmpty(sortOrder) ? "customer_asc" : "";
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

            var loans = _loanService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Pending);
            if (!String.IsNullOrEmpty(searchString))
            {
                loans = loans.Where(s => s.Customer.AspNetUsers.FirstName.ToLower().Contains(searchString.ToLower())
                                       || s.Employee.AspNetUsers.FirstName.ToLower().Contains(searchString.ToLower())
                                       || s.Customer.AspNetUsers.UniqueNumber.Contains(searchString)
                                       || s.Amount.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "date_asc":
                    loans = loans.OrderBy(s => s.DateCreated);
                    break;
                case "customer_asc":
                    loans = loans.OrderBy(s => s.Customer.AspNetUsers.FirstName);
                    break;
                case "amount_desc":
                    loans = loans.OrderByDescending(s => s.Amount);
                    break;
                default:
                    loans = loans.OrderByDescending(s => s.DateCreated);
                    break;
            }

            ViewData["RecordCount"] = loans.ToList().Count;
            return View(PaginatedList<LoanModel>.Create((IEnumerable<LoanModel>)loans, page ?? 1, 10));
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Approval(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewData["EmployeeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "employee_asc" : "";
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

            var loanApprovals = _loanapprovalService.List();
            if (!String.IsNullOrEmpty(searchString))
            {
                loanApprovals = loanApprovals.Where(s => s.Customer.AspNetUsers.FirstName.ToLower().Contains(searchString.ToLower())
                                       || s.Employee.AspNetUsers.FirstName.ToLower().Contains(searchString.ToLower())
                                       || s.Customer.AspNetUsers.UniqueNumber.Contains(searchString)
                                       || s.Loan.Amount.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "date_asc":
                    loanApprovals = loanApprovals.OrderBy(s => s.DateCreated);
                    break;
                case "employee_asc":
                    loanApprovals = loanApprovals.OrderBy(s => s.Employee.AspNetUsers.FirstName);
                    break;
                case "amount_desc":
                    loanApprovals = loanApprovals.OrderByDescending(s => s.Loan.Amount);
                    break;
                default:
                    loanApprovals = loanApprovals.OrderByDescending(s => s.DateCreated);
                    break;
            }
            ViewData["RecordCount"] = loanApprovals.ToList().Count;
            return View(PaginatedList<LoanApprovalModel>.Create((IEnumerable<LoanApprovalModel>)loanApprovals, page ?? 1, 10));
        }

        [Authorize(Roles = "Officer, Supervisor")]
        public IActionResult Create()
        {
            try
            {
                LoanSelectLists();
                return View();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [Authorize(Roles = "Officer, Supervisor")]
        public IActionResult Payment(string id)
        {
            try
            {
                ViewBagData(id);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Approve(string id)
        {
            try
            {
                var request = _loanService.List().Single(x => x.LoanId == id);
                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Decline(string id)
        {
            try
            {
                var request = _loanService.List().Single(x => x.LoanId == id);
                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }


        [Authorize(Roles = "Supervisor")]
        public IActionResult ConfirmRefund(string id)
        {
            try
            {
                var request = _refundService.List().Single(x => x.RefundId == id);
                return View(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [Authorize(Roles = "Supervisor")]
        public IActionResult DeclineRefund(string id)
        {
            try
            {
                var request = _refundService.List().Single(x => x.RefundId == id);
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
        public IActionResult Create(LoanModel model)
        {
            try
            {
                LoanSelectLists();
                model.EmployeeId = GetEmployeeId().Result;
                model.CustomerId = GetCustomerId(model.CustomerId);
                model.Reference = _generator.DateCodeString() + _generator.RandomNumber(1111111, 9999999);
                model.DateCreated = DateTime.UtcNow;
                if (ModelState.IsValid)
                {
                    var request = _loanService.Create(model);
                    if (request.Successful)
                        return RedirectToAction("Index", "Loans");

                    if (request.ResultType == ResultType.PendingTransaction)
                        TempData["Error"] = "This custormer has a serving loan";
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Officer, Supervisor")]
        [ValidateAntiForgeryToken]
        public IActionResult Payment(RefundModel model)
        {
            this.ViewBagData(model.LoanId);
            model.EmployeeId = GetEmployeeId().Result;

            if (!ModelState.IsValid) return View(model);
            var request = _refundService.Create(model);
            if (request.Successful)
            {
                return RedirectToAction("Refund", "Loans", new { id = model.LoanId });
            }

            switch (request.ResultType)
            {
                case ResultType.PendingTransaction:
                    TempData["Error"] = "This custormer has a pending loan transaction";
                    break;
                case ResultType.DataIntegrity:
                    TempData["Error"] = "Data inetgrity error";
                    break;
            }

            return View(model);
        }


        [HttpPost]
        [ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveConfirmed(string id)
        {
            try
            {
                var loan = _loanService.List().FirstOrDefault(x => x.LoanId == id);
                if (loan != null)
                {
                    var loanProduct = _productService.Get(loan.ProductId);
                    loan.StatusCode = Data.Enums.StatusCode.Approved;
                    loan.DueDate = DateTime.UtcNow.AddDays(loanProduct.Tenure);
                    loan.DateApproved = DateTime.UtcNow;
                    _loanService.Update(loan);
                    _loanapprovalService.Create(new LoanApprovalModel
                    {
                        LoanId = id,
                        EmployeeId = GetEmployeeId().Result,
                        CustomerId = loan.CustomerId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return RedirectToAction("Approval");
        }

        [HttpPost]
        [ActionName("Decline")]
        [ValidateAntiForgeryToken]
        public IActionResult DeclineConfirmed(string id)
        {
            try
            {
                var loan = _loanService.List().Single(x => x.LoanId == id);
                if (loan != null)
                {
                    loan.StatusCode = Data.Enums.StatusCode.Declined;
                    _loanService.Update(loan);

                    _loanapprovalService.Create(new LoanApprovalModel
                    {
                        LoanId = id,
                        EmployeeId = GetEmployeeId().Result,
                        CustomerId = loan.CustomerId
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + " " + e.InnerException);
            }
            return RedirectToAction("Approval");
        }

        [HttpPost]
        [ActionName("ConfirmRefund")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmRefundConfirmed(string id)
        {
            try
            {
                var refund = _refundService.List().Single(x => x.RefundId == id);
                if (refund == null) return RedirectToAction("Refund", "Loans", new { id = id });
                var lastCustomerConfirmedTransaction = _refundService.CustomerLastConfirmedTransaction(refund);

                refund.Balance = lastCustomerConfirmedTransaction.Balance - refund.Amount;
                refund.StatusCode = (refund.Amount == refund.Balance)? Data.Enums.StatusCode.Settled: Data.Enums.StatusCode.Confirmed;
                _refundService.Update(refund);


                _refundconfirmService.Create(new RefundConfirmModel()
                {
                    RefundId = id,
                    EmployeeId = GetEmployeeId().Result,
                    CustomerId = refund.CustomerId
                });

                return RedirectToAction("Refund", "Loans", new { id = refund.LoanId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("DeclineRefund")]
        [ValidateAntiForgeryToken]
        public IActionResult DeclineRefundConfirmed(string id)
        {
            try
            {
                var refund = _refundService.List().FirstOrDefault(x => x.RefundId == id);
                if (refund == null) return RedirectToAction("Refund", "Loans", new { id = id });
                refund.StatusCode = Data.Enums.StatusCode.Declined;
                _refundService.Update(refund);

                _refundconfirmService.Create(new RefundConfirmModel
                {
                    RefundId = id,
                    EmployeeId = GetEmployeeId().Result,
                    CustomerId = refund.CustomerId
                });

                return RedirectToAction("Refund", "Loans", new { id = refund.LoanId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }

            return RedirectToAction("Index");

        }

        private void LoanSelectLists()
        {
            ViewBag.Products =
                new SelectList(GetLoansProducts(), "ProductId", "Name", "--Select One--");

            ViewBag.Customers =
                new SelectList(GetUsers().Where(x=>x.UserCode==UserCode.Customer&&x.IsActive==true), "Id", "UniqueNumber", "--Select One--");
        }

        private List<ProductModel> GetLoansProducts()
        {
            var result = _productService.List()
                .Where(x => x.ProductCode == ProductCode.Loans && x.IsDeleted == false).ToList();
            return result;
        }

        private void ViewBagData(string id)
        {
            var result = _loanService.List().FirstOrDefault(x => x.LoanId == id);
            var refunds = _refundService.List().Where(x => x.LoanId == id);


            if (result != null)
            {
                ViewBag.LoanId = id;
                ViewBag.CustomerId = result.CustomerId;
                ViewBag.Customer = result.Customer.AspNetUsers.FirstName + " " + result.Customer.AspNetUsers.LastName;
                ViewBag.AccountNumber = result.Customer.AspNetUsers.UniqueNumber;
                ViewBag.Loan = result.Amount.ToString("C", new CultureInfo("HA-LATN-NG"));
                ViewBag.Interest = result.Interest.ToString("C", new CultureInfo("HA-LATN-NG"));
                ViewBag.Payable = (result.Amount + result.Interest).ToString("C", new CultureInfo("HA-LATN-NG"));
                ViewBag.LoanStartDate = result.DateApproved;
                ViewBag.LoanEndDate = result.DueDate;

                if(refunds.Count() > 0)
                {
                    var refund = refunds.Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed)
                        .OrderByDescending(x=>x.DateCreated).Take(1)
                        .FirstOrDefault();
                    ViewBag.OutstandingPayment = refund.Balance.ToString("C", new CultureInfo("HA-LATN-NG"));
                };
            }
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

        private void ViewDataCustomerLoanInfo(string id)
        {
            var customerInfo = _loanService.List().FirstOrDefault(x => x.LoanId == id);
            if (customerInfo != null)
            {
                ViewData["Name"] = customerInfo?.Customer.AspNetUsers?.FirstName + " " + customerInfo?.Customer.AspNetUsers?.OtherName +" " + customerInfo?.Customer.AspNetUsers?.LastName;
                ViewData["Account"] = customerInfo?.Customer.AspNetUsers?.UniqueNumber;
                ViewData["Product"] = customerInfo?.Product?.Name;
                ViewData["CustomerId"] = customerInfo?.CustomerId;
            }
        }

        private  async Task<List<LoanModel>> GetLoans()
        {

            var currentUser = await CurrentUser();
            var officer = await IsOfficer();
            var employee = _employeeService.List().FirstOrDefault(x => x.AspNetUsersId == currentUser.Id);
            var employeeId = string.Empty;

            if (employee != null)
            {
                employeeId = employee.EmployeeId;
            }

            var loans = _loanService.List();
            return officer ? loans.Where(x => x.EmployeeId.Equals(employeeId)).ToList() : loans.ToList();
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