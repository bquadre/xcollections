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
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Softmax.XCollections.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : BaseController
    {
        readonly ILogger<ProductsController> _logger;
        readonly IProductService _productService;

        public ProductsController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ProductsController> logger,
             IMapper mapper,
             IGenerator generator,
             IProductService productService
           ) : base(
            userManager,
            roleManager,
            mapper,
            generator)
        {
            _logger = logger;
            _productService = productService;

        }

        // GET: /<controller>/
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_asc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "code_asc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var products = _productService.List();
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "date_asc":
                    products = products.OrderBy(s => s.DateCreated);
                    break;
                case "name_asc":
                    products = products.OrderBy(s => s.Name);
                    break;
                default:
                    products = products.OrderByDescending(s => s.DateCreated);
                    break;
            }

            return View(PaginatedList<ProductModel>.Create((IEnumerable<ProductModel>)products, page ?? 1, 10));
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
                var result = _productService.List().FirstOrDefault(x => x.ProductId == id);
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        [HttpPost]
        public IActionResult Save(ProductModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }

                try
                {
                    if (string.IsNullOrEmpty(model.ProductId))
                    {
                         model.DateCreated = DateTime.UtcNow;
                        _productService.Create(model);
                    }
                    else
                    {
                        _productService.Update(model);
                    }
                }
                catch (Exception ex)
                {
                    var err = ex.Message;
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.InnerException);
            }
            return Content("Internal server error please contact the system administrator");
        }

        private Dictionary<int, string> GetProductCodes()
        {
            var codes = new Dictionary<int, string>();
            codes.Add((int)ProductCode.Savings, "Savings");
            codes.Add((int)ProductCode.Loans, "Loans");
            return codes;
        }

        private Dictionary<int, string> GetTenures(int min, int max)
        {
            var tenures = new Dictionary<int, string>();
            tenures.Add(0, "Nil");
            for (int i = min; i < max; i++)
            {
                tenures.Add(i, i + " Days");
            }

            return tenures;
        }

        private Dictionary<int, string> GetRates(int min, int max)
        {
            var rates = new Dictionary<int, string>();
            rates.Add(0, "Nil");
            for (int i = min; i < max; i++)
            {
                rates.Add(i, i + "%");
            }

            return rates;
        }

        private void ViewBagData()
        {
            ViewBag.Codes = new SelectList(GetProductCodes(), "Key", "Value", "--Select One--");
            ViewBag.Tenures = new SelectList(GetTenures(7, 366), "Key", "Value", "--Select One--");
            ViewBag.Rates = new SelectList(GetRates(15, 50), "Key", "Value", "--Select One--");
            ViewBag.Products = _productService.List().Where(x => x.IsDeleted == false);
        }
    }
}
