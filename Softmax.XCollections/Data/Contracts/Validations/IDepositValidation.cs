using Softmax.XCollections.Extensions;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Validations
{
    public interface IDepositValidation
    {
        ValidationResult ValidateCreate(DepositModel model);
        ValidationResult ValidateUpdate(DepositModel model);
    }
}
