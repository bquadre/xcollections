using Softmax.XCollections.Extensions;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Validations
{
    public interface ILoanApprovalValidation
    {
        ValidationResult ValidateCreate(LoanApprovalModel model);        
    }
}
