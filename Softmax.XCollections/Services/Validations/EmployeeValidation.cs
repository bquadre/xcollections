using Softmax.XCollections.Data.Contracts.Validations;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Extensions;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Services.Validations
{
    public class EmployeeValidation : IEmployeeValidation
    {
        private ValidationResult validationResult;
        
        public ValidationResult ValidateCreate(EmployeeModel model)
        {
            this.validationResult = new ValidationResult();

            if (model == null)
            {
                this.validationResult.AddErrorMessage(ResourceDesignation.Invalid_Designation);
                return this.validationResult;
            }

            this.ValidateName(model.AspNetUsersId);

            return this.validationResult;
        }

        public ValidationResult ValidateUpdate(EmployeeModel model)
        {
            this.validationResult = new ValidationResult();

            if (model == null)
            {
                this.validationResult.AddErrorMessage(ResourceDesignation.Invalid_Designation);
                return this.validationResult;
            }

            this.ValidateName(model.AspNetUsersId);

            return this.validationResult;
        }

        private void ValidateName(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                this.validationResult.AddErrorMessage(ResourceDesignation.Invalid_Designation);
            }
        }
    }
}
