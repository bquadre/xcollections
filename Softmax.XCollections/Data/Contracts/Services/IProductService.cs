using System.Collections.Generic;
using Softmax.XCollections.Extensions;
using System.Linq;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Services
{
    public interface IProductService 
    {
        Response<ProductModel> Create(ProductModel model);

        Response<ProductModel> Update(ProductModel model);

        Response<ProductModel> Delete(string id);

        IQueryable<ProductModel> List();

        ProductModel Get(string id);
    }
}
