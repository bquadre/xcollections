using AutoMapper;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserModel>().ReverseMap();

            CreateMap<Branch, BranchModel>().ReverseMap();

            CreateMap<Customer, CustomerModel>().ReverseMap();

            CreateMap<Deposit, DepositModel>().ReverseMap();

            CreateMap<DepositConfirm, DepositConfirmModel>().ReverseMap();
          
            CreateMap<Employee, EmployeeModel>().ReverseMap();

            CreateMap<Loan, LoanModel>().ReverseMap();

            CreateMap<LoanApproval, LoanApprovalModel>().ReverseMap();

            CreateMap<Product, ProductModel>().ReverseMap();

            CreateMap<Refund, RefundModel>().ReverseMap();
            
            CreateMap<RefundConfirm, RefundConfirmModel>().ReverseMap();

        }
    }
}
