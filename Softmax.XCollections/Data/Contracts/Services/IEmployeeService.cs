using System.Collections.Generic;
using Softmax.XCollections.Extensions;
using System.Linq;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Services
{
    public interface IEmployeeService 
    {
        Response<EmployeeModel> Create(EmployeeModel model);

        Response<EmployeeModel> Update(EmployeeModel model);

        Response<EmployeeModel> Delete(string id);

        IQueryable<EmployeeModel> List();

        EmployeeModel Get(string id);

    }
}
