using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Controllers
{
    public class BaseController : Controller
    {
        internal readonly UserManager<ApplicationUser> _userManager;
        internal readonly RoleManager<IdentityRole> _roleManager;
        internal readonly IMapper _mapper;
        internal readonly IGenerator _generator;
       
        internal BaseController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IGenerator generator
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _generator = generator;
        }
        
        internal List<UserModel> GetUsers()
        {
            var users = _userManager.Users.ToList();
            return _mapper.Map<List<UserModel>>(users);
        }
        internal async Task<ApplicationUser> CurrentUser()
        {
            var identity = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(identity);
            return user;
        }
        internal string GenerateGuid()
        {
            var result = _generator.GenerateGuid();
            return result;
        }
        internal string RandomNumbers(int start, int end)
        {
            var result = _generator.RandomNumber(start, end);
            return result;
        }
        internal async Task<string> GenerateUserName()
        {
            var id = string.Empty;

            for (var i = 0; i < 1000000; i++)
            {
                id = _generator
                    .RandomNumber(1000000, 9999999);
                var check = await _userManager.FindByNameAsync(id);
                if (check == null)
                    break;
            }

            return id;
        }
        internal Dictionary<int, string> GetGenderCodes()
        {
            var genders =
                new Dictionary<int, string> { { (int)GenderCode.None, "None" }, { (int)GenderCode.Male, "Male" }, { (int)GenderCode.Female, "Female" } };

            return genders;
        }
        internal Dictionary<int, string> GetUserCodes()
        {
            var users =
                new Dictionary<int, string> { { (int)UserCode.None, "None" }, { (int)UserCode.Employee, "Employee" }, { (int)UserCode.Customer, "Customer" } };

            return users;
        }
        internal Dictionary<int, string> GetStates()
        {
            var states =
                new Dictionary<int, string> { { (int)StateCode.Lagos, "Lagos" }, { (int)StateCode.Ibadan, "Ibadan" } };

            return states;
        }
      
        internal async Task<bool> IsAdmin()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var role = await _userManager.IsInRoleAsync(user, "Admin");
            return role;
        }
        internal async Task<bool> IsManager()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var role = await _userManager.IsInRoleAsync(user, "Manager");
            return role;
        }
        internal async Task<bool> IsSupervisor()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var role = await _userManager.IsInRoleAsync(user, "Supervisor");
            return role;
        }

        internal async Task<bool> IsOfficer()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var role = await _userManager.IsInRoleAsync(user, "Officer");
            return role;
        }



    }
}