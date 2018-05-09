using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Data.Contracts.Services;
using Softmax.XCollections.Data.Contracts.Validations;
using Softmax.XCollections.Data.Entities;
using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Extensions;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Services
{
    public class LoanApprovalService : ILoanApprovalService, IDisposable
    {
        private readonly IRepository<LoanApproval> _loanApprovalRepository;
        private readonly ILoanApprovalValidation _loanApprovalValidation;
        private readonly IGenerator _generator;
        private readonly IMapper _mapper;

        public LoanApprovalService(
            IRepository<LoanApproval> loanApprovalRepository,
            ILoanApprovalValidation loanApprovalValidation,
            IGenerator generator,
            IMapper mapper
        )
        {
            _loanApprovalRepository = loanApprovalRepository;
            _loanApprovalValidation = loanApprovalValidation;
            _generator = generator;
            _mapper = mapper;
        }

        public Response<LoanApprovalModel> Create(LoanApprovalModel model)
        {
            try
            {
                var validationResult = _loanApprovalValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<LoanApprovalModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };
                model.LoanApprovalId = _generator.GenerateGuid();
                model.DateCreated = DateTime.UtcNow.ToLocalTime();
                 

                _loanApprovalRepository.Insert(_mapper.Map<LoanApproval>(model));
                _loanApprovalRepository.Save();

                return new Response<LoanApprovalModel>
                {
                    Result = model,
                    ResultType = ResultType.Success
                };
            }
            catch (Exception ex)
            {
                //online error log
                var err = ex.Message;
            }

            return new Response<LoanApprovalModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<LoanApprovalModel> List()
        {
            var result = _loanApprovalRepository.GetAll();
            return result?.ProjectTo<LoanApprovalModel>();
        }

        public void Dispose()
        {
            _loanApprovalRepository?.Dispose();
        }
    }
}