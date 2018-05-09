using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Data.Contracts.Services;
using Softmax.XCollections.Models;
using System;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Softmax.XCollections.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BranchesController : BaseController
    {
        readonly ILogger<BranchesController> _logger;
        readonly IBranchService _branchService;

        public BranchesController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<BranchesController> logger,
            IMapper mapper,
            IGenerator generator,
            IBranchService branchService
           ) : base(
             userManager,
             roleManager,
             mapper,
             generator)
        {
            _branchService = branchService;
            _logger = logger;
        }

        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewData["CodeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "code_asc" : "";
            ViewData["LocationSortParm"] = String.IsNullOrEmpty(sortOrder) ? "location_asc" : "";
            ViewData["AddressSortParm"] = String.IsNullOrEmpty(sortOrder) ? "addr_asc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var branches = _branchService.List();
            if (!String.IsNullOrEmpty(searchString))
            {
                branches = branches.Where(s => s.BranchCode.Contains(searchString)
                                       || s.Location.Contains(searchString)
                                       || s.Address.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "date_asc":
                    branches = branches.OrderBy(s => s.DateCreated);
                    break;
                case "code_asc":
                    branches = branches.OrderBy(s => s.BranchCode);
                    break;
                case "location_asc":
                    branches = branches.OrderBy(s => s.Location);
                    break;
                case "addr_asc":
                    branches = branches.OrderBy(s => s.Address);
                    break;
                default:
                    branches = branches.OrderByDescending(s => s.DateCreated);
                    break;
            }

            return View(PaginatedList<BranchModel>.Create((IEnumerable<BranchModel>)branches, page ?? 1, 10));
        }

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

        public IActionResult Edit(string id)
        {
            try
            {
                ViewBagData();
                var result = _branchService.List().FirstOrDefault(x => x.BranchId == id);
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpPost]
        public IActionResult Save(BranchModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                return RedirectToAction("Index");

                if (string.IsNullOrEmpty(model.BranchId))
                {
                    model.DateCreated = DateTime.UtcNow;
                    _branchService.Create(model);
                }
                else
                {
                    _branchService.Update(model);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        private void ViewBagData()
        {
            ViewBag.States = new SelectList(GetStates(), "Key", "Value", "--Select One--");
            ViewBag.Branches = _branchService.List().Where(x=>x.IsDeleted == false);
        }
    }
}