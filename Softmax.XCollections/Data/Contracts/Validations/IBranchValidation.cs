using Softmax.XCollections.Extensions;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Validations
{
    public interface IBranchValidation
    {
        ValidationResult ValidateCreate(BranchModel model);
        ValidationResult ValidateUpdate(BranchModel model);
    }
}
