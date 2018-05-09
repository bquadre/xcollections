using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Data.Entities;
using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Softmax.XCollections.Extensions;
using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Data.Contracts.Services;
using Softmax.XCollections.Data.Contracts.Validations;
using Softmax.XCollections.Models;
using Softmax.XCollections.Models.Settings;
using Microsoft.Extensions.Options;

namespace Softmax.XCollections.Services
{
   
    public class DepositService : IDepositService, IDisposable
    {
        private readonly IRepository<Deposit> _depositRepository;
        private readonly IDepositValidation _depositValidation;
        private readonly IMapper _mapper;
        private readonly FeeSettingsModel _feeSettingsModel;

        public DepositService(
              IRepository<Deposit> depositRepository,
              IDepositValidation depositValidation,
              IMapper mapper,
              IOptions<FeeSettingsModel> feeSettingModel)
        {
            _depositRepository = depositRepository;
            _depositValidation = depositValidation;
            _mapper = mapper;
            _feeSettingsModel = feeSettingModel.Value;
        }
         
        public Response<DepositModel> Create(DepositModel model)
        {
            try
            {
                var validationResult = _depositValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                {
                    return new Response<DepositModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };
                }
                var lastTransaction = CustomerLastTransaction(model);
                var lastConfirmedTransaction = CustomerLastConfirmedTransaction(model);
                model.DateCreated = DateTime.UtcNow.ToLocalTime();
                model.Balance = (model.TransactionCode == TransactionCode.Fee)? model.Balance : lastConfirmedTransaction.Balance;
                if (lastTransaction.StatusCode == StatusCode.Pending)
                {
                    return new Response<DepositModel>
                    {
                        Result = model,
                        ResultType = ResultType.PendingTransaction
                    };
                }
             
                if (model.TransactionCode == TransactionCode.Withdrawal)
                {

                    var minimumBalance = Convert.ToInt32(_feeSettingsModel.MinBalance);
                    var withdrawalFee = Convert.ToInt32(_feeSettingsModel.Withdrawal);
                    var deductable = minimumBalance + withdrawalFee;
                    var availableFund = 0;
                    if (lastTransaction.StatusCode == StatusCode.Declined)
                    {
                        availableFund = lastConfirmedTransaction.Balance - deductable;
                    }
                    else
                    {
                        availableFund = lastTransaction.Balance - deductable;
                    }

                    if (model.Amount > availableFund)
                    {
                        return new Response<DepositModel>
                        {
                            Result = model,
                            ResultType = ResultType.InsufficientBalance
                        };
                    }
                }

                _depositRepository.Insert(_mapper.Map<Deposit>(model));
                _depositRepository.Save();

                return new Response<DepositModel>
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

            return new Response<DepositModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<DepositModel> Update(DepositModel model)
        {
            try
            {
                    var validationResult = _depositValidation.ValidateUpdate(model);
                    if (!validationResult.IsValid)
                    {
                        return new Response<DepositModel>
                        {
                            Message = validationResult.ErrorMessage,
                            ResultType = ResultType.ValidationError
                        };
                    }
                var updatingObj = _depositRepository.GetById(model.DepositId);
                    updatingObj.StatusCode = model.StatusCode;
                    updatingObj.Balance = model.Balance;
                        _depositRepository.Save();
               
                    return new Response<DepositModel>
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

            return new Response<DepositModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<DepositModel> Delete(string id)
        {
            try
            {
                //var validationResult = this.depositValidation.ValidateUpdate(model);
                //if (!validationResult.IsValid)
                //    return new Response<CustomerModel>
                //    {
                //        Message = validationResult.ErrorMessage,
                //        ResultType = ResultType.ValidationError
                //    };

                //var entity = mapper.Map<DepositModel>(model);
                _depositRepository.Delete(id);
                _depositRepository.Save();

                return new Response<DepositModel>
                {
                    //Result = model,
                    ResultType = ResultType.Success
                };
            }
            catch (Exception ex)
            {
                //online error log
                var err = ex.Message;
            }

            return new Response<DepositModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<DepositModel> List()
        {
            var result = _depositRepository.GetAll();
            return result.ProjectTo<DepositModel>();
        }

        public DepositModel Get(string id)
        {
            var result = _depositRepository.GetById(id);
            return _mapper.Map<DepositModel>(result);
        }

        public DepositModel CustomerLastTransaction(DepositModel model)
        {
            var deposits = _depositRepository.GetAll()
                .OrderByDescending(x=>x.DateCreated)
                .FirstOrDefault(x => x.CustomerId == model.CustomerId);
            if (deposits == null)
            {
                var firstDeposit = new Deposit
                {
                    Amount = 0,
                    Balance = 0,
                    TransactionCode = TransactionCode.Deposit,
                    CustomerId = model.CustomerId,
                    EmployeeId = model.EmployeeId,
                    ProductId = model.ProductId,
                    DateCreated = DateTime.UtcNow.ToLocalTime(),
                    StatusCode = StatusCode.Confirmed
            };
                
                _depositRepository.Insert(firstDeposit);
                _depositRepository.Save();
                return _mapper.Map<DepositModel>(firstDeposit);
            }

            return _mapper.Map<DepositModel>(deposits);
        }

        public DepositModel CustomerLastConfirmedTransaction(DepositModel model)
        {
            var deposits = _depositRepository.GetAll()
                .OrderByDescending(x => x.DateCreated)
                .FirstOrDefault(x => x.CustomerId == model.CustomerId && x.StatusCode == StatusCode.Confirmed);

            return _mapper.Map<DepositModel>(deposits);
        }

        public void Dispose()
        {
            _depositRepository?.Dispose();
        }    
    }
}