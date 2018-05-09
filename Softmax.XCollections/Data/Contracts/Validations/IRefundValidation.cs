using Softmax.XCollections.Extensions;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Validations
{
    public interface IRefundValidation
    {
        ValidationResult ValidateCreate(RefundModel model);
        ValidationResult ValidateUpdate(RefundModel model);
    }
}
