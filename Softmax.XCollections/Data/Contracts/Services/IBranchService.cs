using System.Collections.Generic;
using Softmax.XCollections.Extensions;
using System.Linq;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Data.Contracts.Services
{
    public interface IBranchService 
    {
        Response<BranchModel> Create(BranchModel model);

        Response<BranchModel> Update(BranchModel model);

        Response<BranchModel> Delete(string id);

        IQueryable<BranchModel> List();

        BranchModel Get(string id);
    }
}
