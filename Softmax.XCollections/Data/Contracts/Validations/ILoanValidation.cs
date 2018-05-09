using Softmax.XCollections.Extensions;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Validations
{
    public interface ILoanValidation
    {
        ValidationResult ValidateCreate(LoanModel model);
        ValidationResult ValidateUpdate(LoanModel model);
    }
}
