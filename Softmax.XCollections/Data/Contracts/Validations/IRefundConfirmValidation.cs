using Softmax.XCollections.Extensions;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Validations
{
    public interface IRefundConfirmValidation
    {
        ValidationResult ValidateCreate(RefundConfirmModel model);        
    }
}
