using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Data.Contracts.Services;
using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Models;
using Softmax.XCollections.Models.Reports;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Softmax.XCollections.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ReportsController : BaseController
    {
        readonly ILogger<ReportsController> _logger;
        readonly IBranchService _branchService;
        readonly ICustomerService _customerService;
        readonly IDepositService _depositService;
        readonly IDepositConfirmService _depositconfirmService;
        readonly IEmployeeService _employeeService;
        readonly ILoanService _loanService;
        readonly ILoanApprovalService _loanapprovalService;
        readonly IProductService _productService;
        readonly IRefundService _refundService;
        readonly IRefundConfirmService _refundconfirmService;

        public ReportsController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ReportsController> logger,
            IMapper mapper,
            IGenerator generator,
            IBranchService branchService,
            IProductService productService,
            ICustomerService customerService,
            IEmployeeService employeeService,
            IDepositService depositService,
            IDepositConfirmService depositconfirmService,
            ILoanService loanService,
            ILoanApprovalService loanapprovalService,
            IRefundService refundService,
            IRefundConfirmService refundconfirmService) : base(userManager, roleManager, mapper, generator)
        {
            _logger = logger;
            _branchService = branchService;
            _productService = productService;   
            _customerService = customerService;
            _employeeService = employeeService;
            _depositService = depositService;
            _depositconfirmService = depositconfirmService;
            _loanService = loanService;
            _loanapprovalService = loanapprovalService;
            _refundService = refundService;
            _refundconfirmService = refundconfirmService;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Manager, Admin")]
        public IActionResult DailySavingsSummary()
        {
            try
            {
                var savings = _depositService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed).ToList();
                var dailyGroupedResult = new List<SavingsReportModel>();

                if (savings.Count > 0)
                {
                    dailyGroupedResult = savings.GroupBy(emp => emp.DateCreated.Date)
                        .Select(y => new SavingsReportModel()
                        {
                            Date = y.First().DateCreated.ToString("M/d/yyyy"),
                            Deposits = y.Where(s => s.TransactionCode == TransactionCode.Deposit).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            Withdrawals = y.Where(s => s.TransactionCode == TransactionCode.Withdrawal).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            Incomes = y.Where(s => s.TransactionCode == TransactionCode.Fee).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<SavingsReportModel>();


                }

                ViewBag.DailySavingsSummary = dailyGroupedResult;
                return View();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Manager, Admin")]
        public IActionResult DailySavings(string id)
        {
            try
            {
                var savings = _depositService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed && x.DateCreated.Date.ToString("M/d/yyyy").Equals(id)).ToList();
                var dailyGroupedResult = new List<SavingsReportModel>();

                if (savings.Count > 0)
                {
                    dailyGroupedResult = savings.GroupBy(emp => emp.EmployeeId)
                        .Select(y => new SavingsReportModel()
                        {
                            Date = y.First().DateCreated.ToString("M/d/yyyy"),
                            Id = y.First().Employee.AspNetUsers.UniqueNumber,
                            Name = y.First().Employee.AspNetUsers.FirstName + " " + y.First().Employee.AspNetUsers.LastName,
                            Deposits = y.Where(s => s.TransactionCode == TransactionCode.Deposit).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            Withdrawals = y.Where(s => s.TransactionCode == TransactionCode.Withdrawal).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            Incomes = y.Where(s => s.TransactionCode == TransactionCode.Fee).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<SavingsReportModel>();


                }

                ViewBag.DailySavings = dailyGroupedResult;
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
        public IActionResult MonthlySavingsSummary()
        {
            try
            {
                var savings = _depositService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed).ToList();
                var monthlyGroupedResult = new List<SavingsReportModel>();
                if (savings.Count > 0)
                {
                    monthlyGroupedResult = savings.GroupBy(emp => new { emp.DateCreated.Date.Month })
                        .Select(y => new SavingsReportModel()
                        {
                            Date = y.First().DateCreated.ToString("MMMM, yyyy"),
                            Deposits = y.Where(s => s.TransactionCode == TransactionCode.Deposit).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            Withdrawals = y.Where(s => s.TransactionCode == TransactionCode.Withdrawal).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            Incomes = y.Where(s => s.TransactionCode == TransactionCode.Fee).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<SavingsReportModel>();


                }

                ViewBag.MonthlySavingsSummary = monthlyGroupedResult;
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
        public IActionResult MonthlySavings(string id)
        {
            try
            {
                var savings = _depositService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed && x.DateCreated.ToString("MMMM, yyyy").Equals(id)).ToList();
                var monthlyGroupedResult = new List<SavingsReportModel>();
                if (savings.Count > 0)
                {


                    monthlyGroupedResult = savings.GroupBy(emp => new { emp.EmployeeId })
                        .Select(y => new SavingsReportModel()
                        {
                            Date = y.First().DateCreated.ToString("MMMM, yyyy"),
                            Id = y.First().Employee.AspNetUsers.UniqueNumber,
                            Name = y.First().Employee.AspNetUsers.FirstName + " " + y.First().Employee.AspNetUsers.LastName,
                            Deposits = y.Where(s => s.TransactionCode == TransactionCode.Deposit).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            Withdrawals = y.Where(s => s.TransactionCode == TransactionCode.Withdrawal).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            Incomes = y.Where(s => s.TransactionCode == TransactionCode.Fee).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<SavingsReportModel>();


                }

                ViewBag.MonthlySavings = monthlyGroupedResult;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult YearlySavingsSummary()
        {
            try
            {
                var savings = _depositService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed).ToList();
                var yearlyGroupedResult = new List<SavingsReportModel>();
                if (savings.Count > 0)
                {
                    yearlyGroupedResult = savings.GroupBy(emp => new { emp.DateCreated.Date.Year })
                        .Select(y => new SavingsReportModel()
                        {
                            Date = y.First().DateCreated.Date.Year.ToString(),
                            Deposits = y.Where(s => s.TransactionCode == TransactionCode.Deposit).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            Withdrawals = y.Where(s => s.TransactionCode == TransactionCode.Withdrawal).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            Incomes = y.Where(s => s.TransactionCode == TransactionCode.Fee).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<SavingsReportModel>();
                }

                ViewBag.YearlySavingsSummary = yearlyGroupedResult;
                return View();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult YearlySavings(string id)
        {
            try
            {
                var savings = _depositService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed && x.DateCreated.Date.Year.ToString().Equals(id)).ToList();
                var yearlyGroupedResult = new List<SavingsReportModel>();
                if (savings.Count > 0)
                {
                    yearlyGroupedResult = savings.GroupBy(emp => new { emp.EmployeeId })
                        .Select(y => new SavingsReportModel()
                        {
                            Date = y.First().DateCreated.Date.Year.ToString(),
                            Id = y.First().Employee.AspNetUsers.UniqueNumber,
                            Name = y.First().Employee.AspNetUsers.FirstName + " " + y.First().Employee.AspNetUsers.LastName,
                            Deposits = y.Where(s => s.TransactionCode == TransactionCode.Deposit).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            Withdrawals = y.Where(s => s.TransactionCode == TransactionCode.Withdrawal).Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<SavingsReportModel>();
                }

                ViewBag.YearlySavings = yearlyGroupedResult;
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
        public IActionResult MonthlyLoansSummary()
        {
            try
            {
                var loans = _loanService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Approved).ToList();
                var monthlyGroupedResult = new List<LoansReportModel>();
                if (loans.Count > 0)
                {

                    monthlyGroupedResult = loans.GroupBy(emp => new { emp.DateCreated.Date.Month })
                        .Select(y => new LoansReportModel()
                        {
                            Date = y.First().DateCreated.ToString("MMMM, yyyy"),
                            Disbursed = y.Count().ToString(),
                            AmountDisbursed = y.Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<LoansReportModel>();


                }

                ViewBag.MonthlyLoansSummary = monthlyGroupedResult;
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
        public IActionResult MonthlyLoans(string id)
        {
            try
            {
                var loans = _loanService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Approved && x.DateCreated.ToString("MMMM, yyyy").Equals(id)).ToList();
                var monthlyGroupedResult = new List<LoansReportModel>();
                if (loans.Count > 0)
                {

                    monthlyGroupedResult = loans.GroupBy(emp => new { emp.EmployeeId })
                        .Select(y => new LoansReportModel()
                        {
                            Date = y.First().DateCreated.ToString("MMMM, yyyy"),
                            Id = y.First().Employee.AspNetUsers.UniqueNumber,
                            Name = y.First().Employee.AspNetUsers.FirstName + " " + y.First().Employee.AspNetUsers.LastName,
                            Disbursed = y.Count().ToString(),
                            Defaulted = y.Count(x => x.DueDate.Value.Date < DateTime.UtcNow.Date).ToString(),
                            AmountDisbursed = y.Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            ExpectedRefund = y.Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            ExpectedProfit = y.Sum(x => x.Interest).ToString("C", new CultureInfo("HA-LATN-NG"))
                        }).ToList<LoansReportModel>();


                }

                ViewBag.MonthlyLoans = monthlyGroupedResult;
                return View();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult YearlyLoansSummary()
        {
            try
            {
                var loans = _loanService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Approved).ToList();
                var yearlyGroupedResult = new List<LoansReportModel>();
                if (loans.Count > 0)
                {
                    yearlyGroupedResult = loans.GroupBy(emp => new { emp.DateCreated.Date.Year })
                        .Select(y => new LoansReportModel()
                        {
                            Date = y.First().DateCreated.Date.Year.ToString(),
                            Disbursed = y.Count().ToString(),
                            Defaulted = y.Count(x => x.DueDate.Value.Date < DateTime.UtcNow.Date).ToString(),
                            AmountDisbursed = y.Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            ExpectedRefund = (y.Sum(x => x.Amount) + y.Sum(x => x.Amount)).ToString("C", new CultureInfo("HA-LATN-NG")),
                            ExpectedProfit = y.Sum(x => x.Interest).ToString("C", new CultureInfo("HA-LATN-NG"))
                        }).ToList<LoansReportModel>();
                }

                ViewBag.YearlyLoansSummary = yearlyGroupedResult;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult YearlyLoans(string id)
        {
            try
            {
                var loans = _loanService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Approved).ToList();
                var yearlyGroupedResult = new List<LoansReportModel>();
                if (loans.Count > 0)
                {
                    yearlyGroupedResult = loans.GroupBy(emp => new { emp.EmployeeId, emp.DateCreated.Date.Year })
                        .Select(y => new LoansReportModel()
                        {
                            Date = y.First().DateCreated.Date.Year.ToString(),
                            Id = y.First().Employee.AspNetUsers.UniqueNumber,
                            Name = y.First().Employee.AspNetUsers.FirstName + " " + y.First().Employee.AspNetUsers.LastName,
                            Disbursed = y.Count().ToString(),
                            Defaulted = y.Count(x => x.DueDate.Value.Date < DateTime.UtcNow.Date).ToString(),
                            AmountDisbursed = y.Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            ExpectedRefund = y.Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                            ExpectedProfit = y.Sum(x => x.Interest).ToString("C", new CultureInfo("HA-LATN-NG"))
                        }).ToList<LoansReportModel>();
                }

                ViewBag.YearlyLoans = yearlyGroupedResult;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Manager, Admin")]
        public IActionResult DailyRefundsSummary()
        {
            try
            {
                var refunds = _refundService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed).ToList();
                var dailyGroupedResult = new List<RefundsReportModel>();
                if (refunds.Count > 0)
                {
                    dailyGroupedResult = refunds.GroupBy(emp => new { emp.DateCreated.Date })
                        .Select(y => new RefundsReportModel()
                        {
                            Date = y.First().DateCreated.ToString("d"),
                            Refunds = y.Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<RefundsReportModel>();


                }

                ViewBag.DailyRefundsSummary = dailyGroupedResult;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor, Manager, Admin")]
        public IActionResult DailyRefunds(string id)
        {
            try
            {
                var refunds = _refundService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed && x.DateCreated.ToString("d").Equals(id)).ToList();
                var dailyGroupedResult = new List<RefundsReportModel>();
                if (refunds.Count > 0)
                {
                    dailyGroupedResult = refunds.GroupBy(emp => new { emp.EmployeeId })
                        .Select(y => new RefundsReportModel()
                        {
                            Date = y.First().DateCreated.ToString("d"),
                            Id = y.First().Loan.Employee.AspNetUsers.UniqueNumber,
                            Name = y.First().Loan.Employee.AspNetUsers.FirstName + " " + y.First().Loan.Employee.AspNetUsers.LastName,
                            Refunds = y.Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<RefundsReportModel>();


                }

                ViewBag.DailyRefunds = dailyGroupedResult;

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
        public IActionResult MonthlyRefundsSummary()
        {
            try
            {
                var refunds = _refundService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed).ToList();
                var monthlyGroupedResult = new List<RefundsReportModel>();
                if (refunds.Count > 0)
                {

                    monthlyGroupedResult = refunds.GroupBy(emp => new { emp.DateCreated.Date.Month })
                        .Select(y => new RefundsReportModel()
                        {
                            Date = y.First().DateCreated.ToString("MMMM, yyyy"),
                            Refunds = y.Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<RefundsReportModel>();
                }

                ViewBag.MonthlyRefundsSummary = monthlyGroupedResult;
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
        public IActionResult MonthlyRefunds(string id)
        {
            try
            {
                var refunds = _refundService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed && x.DateCreated.ToString("MMMM, yyyy").Equals(id)).ToList();
                var monthlyGroupedResult = new List<RefundsReportModel>();
                if (refunds.Count > 0)
                {
                    monthlyGroupedResult = refunds.GroupBy(emp => new { emp.EmployeeId })
                        .Select(y => new RefundsReportModel()
                        {
                            Date = y.First().DateCreated.ToString("MMMM, yyyy"),
                            Id = y.First().Loan.Employee.AspNetUsers.UniqueNumber,
                            Name = y.First().Loan.Employee.AspNetUsers.FirstName + " " + y.First().Loan.Employee.AspNetUsers.LastName,
                            Refunds = y.Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<RefundsReportModel>();
                }

                ViewBag.MonthlyRefunds = monthlyGroupedResult;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult YearlyRefundsSummary()
        {
            try
            {
                var refunds = _refundService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed).ToList();
                var yearlyGroupedResult = new List<RefundsReportModel>();
                if (refunds.Count > 0)
                {

                    yearlyGroupedResult = refunds.GroupBy(emp => new { emp.DateCreated.Date.Year })
                        .Select(y => new RefundsReportModel()
                        {
                            Date = y.First().DateCreated.Date.Year.ToString(),
                            Refunds = y.Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<RefundsReportModel>();
                }

                ViewBag.YearlyRefundsSummary = yearlyGroupedResult;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public IActionResult YearlyRefunds(string id)
        {
            try
            {
                var refunds = _refundService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed && x.DateCreated.Date.Year.ToString().Equals(id)).ToList();
                var yearlyGroupedResult = new List<RefundsReportModel>();
                if (refunds.Count > 0)
                {
                    yearlyGroupedResult = refunds.GroupBy(emp => new { emp.EmployeeId, emp.DateCreated.Date.Year })
                        .Select(y => new RefundsReportModel()
                        {
                            Date = y.First().DateCreated.Date.Year.ToString(),
                            Id = y.First().Loan.Employee.AspNetUsers.UniqueNumber,
                            Name = y.First().Loan.Employee.AspNetUsers.FirstName + " " + y.First().Loan.Employee.AspNetUsers.LastName,
                            Refunds = y.Sum(x => x.Amount).ToString("C", new CultureInfo("HA-LATN-NG")),
                        }).ToList<RefundsReportModel>();
                }

                ViewBag.YearlyRefunds = yearlyGroupedResult;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }
    }
}