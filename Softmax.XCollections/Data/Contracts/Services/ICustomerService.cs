using System.Collections.Generic;
using Softmax.XCollections.Extensions;
using System.Linq;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Services
{
    public interface ICustomerService 
    {
        Response<CustomerModel> Create(CustomerModel model);

        Response<CustomerModel> Update(CustomerModel model);

        Response<CustomerModel> Delete(string id);

        IQueryable<CustomerModel> List();

        CustomerModel Get(string id);
    }
}
