using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Softmax.XCollections.Models.AccountViewModels
{
    public class UserRoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}
