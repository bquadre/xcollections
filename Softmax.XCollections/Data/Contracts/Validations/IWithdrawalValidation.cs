using Softmax.XCollections.Extensions;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Validations
{
    public interface IWithdrawalValidation
    {
        ValidationResult ValidateCreate(WithdrawalModel model);
        ValidationResult ValidateUpdate(WithdrawalModel model);
    }
}
