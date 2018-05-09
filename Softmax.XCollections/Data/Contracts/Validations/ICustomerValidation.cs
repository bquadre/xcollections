using Softmax.XCollections.Extensions;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Validations
{
    public interface ICustomerValidation
    {
        ValidationResult ValidateCreate(CustomerModel model);
        ValidationResult ValidateUpdate(CustomerModel model);
    }
}
