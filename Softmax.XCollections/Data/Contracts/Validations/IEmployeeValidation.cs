using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Extensions;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Validations
{
    public interface IEmployeeValidation
    {
        ValidationResult ValidateCreate(EmployeeModel model);
        ValidationResult ValidateUpdate(EmployeeModel model);
    }
}
