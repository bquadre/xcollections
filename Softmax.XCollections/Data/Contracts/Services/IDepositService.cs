using System.Collections.Generic;
using Softmax.XCollections.Extensions;
using System.Linq;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Services
{
    public interface IDepositService 
    {
        Response<DepositModel> Create(DepositModel model);

        Response<DepositModel> Update(DepositModel model);

        Response<DepositModel> Delete(string id);

        IQueryable<DepositModel> List();

        DepositModel Get(string id);

        DepositModel CustomerLastTransaction(DepositModel model);

        DepositModel CustomerLastConfirmedTransaction(DepositModel model);

    }
}
