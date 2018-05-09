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
    public class RefundConfirmService : IRefundConfirmService, IDisposable
    {
        private readonly IRepository<RefundConfirm> _refundConfirmRepository;
        private readonly IRefundConfirmValidation _refundConfirmValidation;
        private readonly IGenerator _generator;
        private readonly IMapper _mapper;

        public RefundConfirmService(
            IRepository<RefundConfirm> refundConfirmRepository,
            IRefundConfirmValidation refundConfirmValidation,
            IGenerator generator,
            IMapper mapper
        )
        {
            _refundConfirmRepository = refundConfirmRepository;
            _refundConfirmValidation = refundConfirmValidation;
            _generator = generator;
            _mapper = mapper;
        }

        public Response<RefundConfirmModel> Create(RefundConfirmModel model)
        {
            try
            {
                var validationResult = _refundConfirmValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<RefundConfirmModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };
                model.RefundConfirmId = _generator.GenerateGuid();
                model.DateCreated = DateTime.UtcNow.ToLocalTime();
                 

                _refundConfirmRepository.Insert(_mapper.Map<RefundConfirm>(model));
                _refundConfirmRepository.Save();

                return new Response<RefundConfirmModel>
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

            return new Response<RefundConfirmModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<RefundConfirmModel> List()
        {
            var result = _refundConfirmRepository.GetAll();
            return result?.ProjectTo<RefundConfirmModel>();
        }

        public void Dispose()
        {
            _refundConfirmRepository?.Dispose();
        }
    }
}