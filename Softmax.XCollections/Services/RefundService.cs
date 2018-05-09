using System;
using System.Collections.Generic;
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
    public class RefundService : IRefundService, IDisposable
    {
        private readonly IGenerator _generator;
        private readonly IRepository<Loan> _loanRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<Refund> _refundRepository;
        private readonly IRefundValidation _refundValidation;

        public RefundService(
            IRepository<Loan> loanRepository,
            IRepository<Refund> refundRepository,
            IRefundValidation refundValidation,
            IGenerator generator,
            IMapper mapper
        )
        {
            _loanRepository = loanRepository;
            _refundRepository = refundRepository;
            _refundValidation = refundValidation;
            _generator = generator;
            _mapper = mapper;
        }

        public void Dispose()
        {
            _refundRepository?.Dispose();
        }


        public Response<RefundModel> Create(RefundModel model)
        {
            try
            {
                var validationResult = _refundValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                {
                    return new Response<RefundModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };
                }

                var lastTransaction = CustomerLastTransaction(model);//check for pending transaction
                var lastConfirmedTransaction = CustomerLastConfirmedTransaction(model);//integrity check
                
                if (lastTransaction.StatusCode == StatusCode.Pending)
                {
                    return new Response<RefundModel>
                    {
                        Result = model,
                        ResultType = ResultType.PendingTransaction
                    };
                }

                if (lastConfirmedTransaction != null && model.Amount > lastConfirmedTransaction.Balance)
                {
                    return new Response<RefundModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.DataIntegrity
                    };
                }

               
                model.RefundId = _generator.GenerateGuid();
                model.Reference = _generator.RandomNumber(1000000, 9999999);
                model.DateCreated = DateTime.UtcNow.ToLocalTime();
                model.Balance = lastConfirmedTransaction.Balance;


                _refundRepository.Insert(_mapper.Map<Refund>(model));
                _refundRepository.Save();

                return new Response<RefundModel>
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

            return new Response<RefundModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<RefundModel> Update(RefundModel model)
        {
            try
            {
                var validationResult = _refundValidation.ValidateUpdate(model);
                if (!validationResult.IsValid)
                    return new Response<RefundModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                var existingModel = _refundRepository.GetById(model.RefundId);
                if (existingModel != null)
                {
                    existingModel.Balance = model.Balance;
                    existingModel.StatusCode = model.StatusCode;
                }

                _refundRepository.Update(existingModel);
                _refundRepository.Save();

                return new Response<RefundModel>
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

            return new Response<RefundModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<RefundModel> Delete(string id)
        {
            try
            {
              
                _refundRepository.Delete(id);
                _refundRepository.Save();

                return new Response<RefundModel>
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

            return new Response<RefundModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<RefundModel> List()
        {
            var result = _refundRepository.GetAll();
            return result?.ProjectTo<RefundModel>();
        }

        public RefundModel Get(string id)
        {
            var result = _refundRepository.GetById(id);
            return result == null ? null : _mapper.Map<RefundModel>(result);
        }

        public RefundModel CustomerLastTransaction(RefundModel model)
        {
            var loan = _loanRepository.GetById(model.LoanId);
            if (loan == null)
            {
                return null;
            }

            var refund = _refundRepository.GetAll()
                .OrderByDescending(x => x.DateCreated)
                .FirstOrDefault(x => x.CustomerId == model.CustomerId && x.LoanId == model.LoanId);

            if (refund == null)
            {
                var firstRefund = new Refund
                {
                    Amount = 0,
                    Balance = loan.Amount + loan.Interest,
                    CustomerId = model.CustomerId,
                    EmployeeId = model.EmployeeId,
                    RefundId = _generator.GenerateGuid(),
                    Reference = _generator.RandomNumber(1111111, 9999999),
                    LoanId = model.LoanId,
                    DateCreated = DateTime.UtcNow.ToLocalTime(),
                    StatusCode = StatusCode.Confirmed
                };

                _refundRepository.Insert(firstRefund);
                _refundRepository.Save();

                //manual mapping got the job done
                var refundModel = new RefundModel()
                {
                    StatusCode = firstRefund.StatusCode
                };

                return refundModel;
            }

            //manual mapping got the job done
            var newModel = new RefundModel()
            {
                StatusCode = refund.StatusCode
            };
            return newModel;
        }

        public RefundModel CustomerLastConfirmedTransaction(RefundModel model)
        {
            var refund = _refundRepository.GetAll()
                .OrderByDescending(x => x.DateCreated)
                .FirstOrDefault(x => x.CustomerId == model.CustomerId && x.LoanId == model.LoanId && x.StatusCode == StatusCode.Confirmed);
            if(refund == null)
            {
                return null;
            }

            var newModel = new RefundModel()
            {
                Amount = refund.Amount,
                Balance = refund.Balance,
            };

            return newModel;
        }
    }
}