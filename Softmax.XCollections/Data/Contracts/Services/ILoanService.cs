using System.Collections.Generic;
using Softmax.XCollections.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Services
{
    public interface ILoanService 
    {
        Response<LoanModel> Create(LoanModel model);

        Response<LoanModel> Update(LoanModel model);

        Response<LoanModel> Delete(string id);

        IQueryable<LoanModel> List();

        LoanModel Get(string id);
    }
}
