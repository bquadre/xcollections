using Softmax.XCollections.Extensions;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Validations
{
    public interface IProductValidation
    {
        ValidationResult ValidateCreate(ProductModel model);
        ValidationResult ValidateUpdate(ProductModel model);
    }
}
