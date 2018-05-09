using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required, Display(Name = "Login ID")]     
        public string Email { get; set; }
    }
}
