using Softmax.XCollections.Extensions;
using System.Linq;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Services
{
    public interface IWithdrawalService 
    {
        Response<WithdrawalModel> Create(WithdrawalModel model);

        Response<WithdrawalModel> Update(WithdrawalModel model);

        Response<WithdrawalModel> Delete(string id);

        Response<IQueryable<WithdrawalModel>> List(string search = "");

        Response<WithdrawalModel> Get(string id);

    }
}
