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
    public class DepositConfirmService : IDepositConfirmService, IDisposable
    {
        private readonly IRepository<DepositConfirm> _depositConfirmRepository;
        private readonly IDepositConfirmValidation _depositConfirmValidation;
        private readonly IGenerator _generator;
        private readonly IMapper _mapper;

        public DepositConfirmService(
            IRepository<DepositConfirm> depositConfirmRepository,
            IDepositConfirmValidation depositConfirmValidation,
            IGenerator generator,
            IMapper mapper
        )
        {
            _depositConfirmRepository = depositConfirmRepository;
            _depositConfirmValidation = depositConfirmValidation;
            _generator = generator;
            _mapper = mapper;
        }

        public Response<DepositConfirmModel> Create(DepositConfirmModel model)
        {
            try
            {
                var validationResult = _depositConfirmValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<DepositConfirmModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };
                model.DepositConfirmId = _generator.GenerateGuid();
                model.DateCreated = DateTime.UtcNow.ToLocalTime();
                 
                _depositConfirmRepository.Insert(_mapper.Map<DepositConfirm>(model));
                _depositConfirmRepository.Save();

                return new Response<DepositConfirmModel>
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

            return new Response<DepositConfirmModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<DepositConfirmModel> List()
        {
            var result = _depositConfirmRepository.GetAll();
            return result.ProjectTo<DepositConfirmModel>();
        }

        public void Dispose()
        {
            _depositConfirmRepository?.Dispose();
        }
    }
}