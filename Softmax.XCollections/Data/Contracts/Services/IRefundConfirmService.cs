using Softmax.XCollections.Extensions;
using System.Linq;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Services
{
    public interface IRefundConfirmService 
    {
        Response<RefundConfirmModel> Create(RefundConfirmModel model);

        IQueryable<RefundConfirmModel> List();
    }
}
