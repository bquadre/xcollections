using Softmax.XCollections.Extensions;
using System.Linq;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Services
{
    public interface IDepositConfirmService 
    {
        Response<DepositConfirmModel> Create(DepositConfirmModel model);

        IQueryable<DepositConfirmModel> List();
    }
}
