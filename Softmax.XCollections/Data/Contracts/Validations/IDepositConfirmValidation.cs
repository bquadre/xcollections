using Softmax.XCollections.Extensions;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Validations
{
    public interface IDepositConfirmValidation
    {
        ValidationResult ValidateCreate(DepositConfirmModel model);        
    }
}
