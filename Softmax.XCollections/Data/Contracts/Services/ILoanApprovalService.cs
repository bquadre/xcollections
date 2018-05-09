using System.Collections.Generic;
using System.Linq;
using Softmax.XCollections.Extensions;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Services
{
    public interface ILoanApprovalService 
    {
        Response<LoanApprovalModel> Create(LoanApprovalModel model);

        IQueryable<LoanApprovalModel> List();
    }
}
