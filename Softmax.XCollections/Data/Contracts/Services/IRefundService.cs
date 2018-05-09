using System.Collections.Generic;
using Softmax.XCollections.Extensions;
using System.Linq;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Services
{
    public interface IRefundService 
    {
        Response<RefundModel> Create(RefundModel model);

        Response<RefundModel> Update(RefundModel model);

        Response<RefundModel> Delete(string id);

       IQueryable<RefundModel> List();

       RefundModel Get(string id);

       RefundModel CustomerLastTransaction(RefundModel model);

       RefundModel CustomerLastConfirmedTransaction(RefundModel model);
    }
}
