using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Data.Contracts.Services;
using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        readonly ILogger<HomeController> _logger;
        readonly IEmployeeService _employeeService;
        readonly ICustomerService _customerService;
        readonly IDepositService _depositService;
        readonly ILoanService _loanService;
        readonly IRefundService _refundService;

        public HomeController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            ILogger<HomeController> logger,
            IGenerator generator,
            IEmployeeService employeeService,
            ICustomerService customerService,
            IDepositService depositService,
            ILoanService loanService,
            IRefundService refundService
          ) : base(
            userManager,
            roleManager,
            mapper,
            generator
           )
        {
            _logger = logger;
            _employeeService = employeeService;
            _customerService = customerService;
            _depositService = depositService;
            _loanService = loanService;
            _refundService = refundService;
        }

        public IActionResult Index()
        {
            try
            {
                var deposits = (this.GetDeposits().Result.Count > 0) ? this.GetDeposits().Result.Where(x => x.TransactionCode == TransactionCode.Deposit).Sum(x => x.Amount) : 0;
                var withdrawals = (this.GetDeposits().Result.Count > 0) ? this.GetDeposits().Result.Where(x => x.TransactionCode == TransactionCode.Withdrawal).Sum(x => x.Amount) : 0;
                var fees = (this.GetDeposits().Result.Count > 0) ? this.GetDeposits().Result.Where(x => x.TransactionCode == TransactionCode.Fee).Sum(x => x.Amount) : 0;
                var loans = (this.GetLoans().Result.Count > 0) ? this.GetLoans().Result.Sum(x => x.Amount) : 0;
                var refunds = (this.GetRefunds().Result.Count > 0) ? this.GetRefunds().Result.Sum(x => x.Amount) : 0;
                var interest = refunds - loans;
                var incomes = interest + fees;
                
                ViewBag.TotalDeposits = deposits;
                ViewBag.TotalWithdrawals = withdrawals;
                ViewBag.TotalLoans = loans;
                ViewBag.TotalRefunds = refunds;
                ViewBag.TotalIncomes = incomes;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
            
        }

        private  async Task<List<DepositModel>> GetDeposits()
        {
            var deposits = new List<DepositModel>();
            try
            {
                var currentUser = await CurrentUser();
                var admin = await IsAdmin();
                var employee = _employeeService.List().FirstOrDefault(x => x.AspNetUsersId == currentUser.Id);
                var employeeId = string.Empty;

                if (employee != null)
                {
                    employeeId = employee.EmployeeId;
                }

              
                var result = _depositService.List().ToList();

                if(result.Count > 0)
                {
                    deposits = result.Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed).ToList();
                }
                    
                return admin ? deposits.ToList() : deposits.Where(x => x.EmployeeId.Equals(employeeId)).ToList();

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message + " " + ex.InnerException);
            }

            return deposits;
        }

        private  async Task<List<LoanModel>> GetLoans()
        {
            try
            {
                var currentUser = await CurrentUser();
                var admin = await IsAdmin();
                var employee = _employeeService.List().FirstOrDefault(x => x.AspNetUsersId == currentUser.Id);
                var employeeId = string.Empty;

                if (employee != null)
                {
                    employeeId = employee.EmployeeId;
                }

                var loans = _loanService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Approved);
                return admin ? loans.ToList() : loans.Where(x => x.EmployeeId.Equals(employeeId)).ToList();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message + " " + ex.InnerException);
            }

            return new List<LoanModel>();
        }

        private async Task<List<RefundModel>> GetRefunds()
        {
            try
            {
                var currentUser = await CurrentUser();
                var admin = await IsAdmin();
                var employee = _employeeService.List().FirstOrDefault(x => x.AspNetUsersId == currentUser.Id);
                var employeeId = string.Empty;

                if (employee != null)
                {
                    employeeId = employee.EmployeeId;
                }

                var refunds = _refundService.List().Where(x => x.StatusCode == Data.Enums.StatusCode.Confirmed);
                return admin ? refunds.ToList() : refunds.Where(x => x.EmployeeId.Equals(employeeId)).ToList();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message + " " + ex.InnerException);
            }

            return new List<RefundModel>();

        }


        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<ChartModel> Daily()
        {
            var result = await this.GetDeposits();
            var last15Days = result
                .OrderBy(x => x.DateCreated)
                .GroupBy(x => x.DateCreated.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Deposits = g.Where(x => x.TransactionCode == TransactionCode.Deposit).Sum(x => x.Amount),
                    Withdrawals = g.Where(x => x.TransactionCode == TransactionCode.Withdrawal).Sum(x => x.Amount),
                    Incomes = g.Where(x => x.TransactionCode == TransactionCode.Fee).Sum(x => x.Amount),
                }).Distinct().TakeLast(15).ToList();


            var dates = new List<string>();
            var deposits = new List<int>();
            var withdrawals = new List<int>();
            var incomes = new List<int>();

            foreach (var item in last15Days)
            {
                dates.Add(item.Date.ToShortDateString());
                deposits.Add(item.Deposits);
                withdrawals.Add(item.Withdrawals);
                incomes.Add(item.Incomes);
            }

            var data = new ChartModel()
            {
                Dates = dates,
                Deposits = deposits,
                Withdrawals = withdrawals,
                Incomes = incomes
            };
            return data;
        }


        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<ChartModel> Monthly()
        {
            var result = await this.GetDeposits();
            var monthly = result
                .OrderBy(x => x.DateCreated)
                .GroupBy(x => x.DateCreated.Month)
                .Select(g => new
                {
                    Date = g.Key,
                    Deposits = g.Where(x => x.TransactionCode == TransactionCode.Deposit).Sum(x => x.Amount),
                    Withdrawals = g.Where(x => x.TransactionCode == TransactionCode.Withdrawal).Sum(x => x.Amount),
                    Incomes = g.Where(x => x.TransactionCode == TransactionCode.Fee).Sum(x => x.Amount),
                }).Distinct().TakeLast(6).ToList();


            var dates = new List<string>();
            var deposits = new List<int>();
            var withdrawals = new List<int>();
            var incomes = new List<int>();

            foreach (var item in monthly)
            {
                dates.Add(item.Date.ToString());
                deposits.Add(item.Deposits);
                withdrawals.Add(item.Withdrawals);
                incomes.Add(item.Incomes);
            }

            var data = new ChartModel()
            {
                Dates = dates,
                Deposits = deposits,
                Withdrawals = withdrawals,
                Incomes = incomes
            };
            return data;
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<PieChartModel> Overall()
        {
            var loans = this.GetLoans().Result.Sum(x => x.Amount);
            var refunds = this.GetRefunds().Result.Sum(x => x.Amount);
            var interest = refunds - loans;
            var result = await this.GetDeposits();
            var deposits = result.Where(x => x.TransactionCode == TransactionCode.Deposit).Sum(x => x.Amount);
            var withdrawals = result.Where(x => x.TransactionCode == TransactionCode.Withdrawal).Sum(x => x.Amount);
            var fee = result.Where(x => x.TransactionCode == TransactionCode.Fee).Sum(x => x.Amount);
            var total = interest + fee;
            var incomes = (total > 0) ? total : 0;

            var data = new PieChartModel()
            {
                Totals = new List<int> { deposits, withdrawals, loans, incomes}
            };

            return data;
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
