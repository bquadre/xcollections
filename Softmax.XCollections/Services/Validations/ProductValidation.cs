using Softmax.XCollections.Data.Contracts.Validations;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Models;
using Softmax.XCollections.Extensions;

namespace Softmax.XCollections.Services.Validations
{
    public class ProductValidation : IProductValidation
    {
        private ValidationResult validationResult;
        
        public ValidationResult ValidateCreate(ProductModel model)
        {
            this.validationResult = new ValidationResult();

            if (model == null)
            {
                this.validationResult.AddErrorMessage(ResourceDesignation.Invalid_Designation);
                return this.validationResult;
            }

            this.ValidateName(model.Name);

            return this.validationResult;
        }

        public ValidationResult ValidateUpdate(ProductModel model)
        {
            this.validationResult = new ValidationResult();

            if (model == null)
            {
                this.validationResult.AddErrorMessage(ResourceDesignation.Invalid_Designation);
                return this.validationResult;
            }

            this.ValidateName(model.Name);

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
