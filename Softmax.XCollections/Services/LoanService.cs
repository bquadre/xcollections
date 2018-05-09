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

namespace Softmax.XCollections.Services
{
    public class LoanService : ILoanService, IDisposable
    {
        private readonly IRepository<Loan> _loanRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly ILoanValidation _loanValidation;
        private readonly IGenerator _generator;
        private readonly IMapper _mapper;

        public LoanService(
              IRepository<Loan> loanRepository,
              IRepository<Product> productRepository,
              ILoanValidation loanValidation,
              IGenerator generator,
              IMapper mapper)
        {
            _loanRepository = loanRepository;
            _productRepository = productRepository;
            _loanValidation = loanValidation;
            _generator = generator;
            _mapper = mapper;
        }

        public Response<LoanModel> Create(LoanModel model)
        {
            try
            {
                var validationResult = _loanValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<LoanModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                var loanProduct = this.GetLoanProduct(model.ProductId);
                var lastTransaction = this.CustomerLastTransaction(model);

                model.Interest = (model.Amount * loanProduct.Rate) / 100;
                model.DateCreated = DateTime.UtcNow.ToLocalTime();
               //// model.ProductId 
               // if (lastTransaction != null && lastTransaction.StatusCode == StatusCode.Pending || lastTransaction != null && lastTransaction.StatusCode != StatusCode.Declined && lastTransaction.StatusCode != StatusCode.Settled)
               // {
               //     return new Response<LoanModel>
               //     {
               //         Result = model,
               //         ResultType = ResultType.PendingTransaction
               //     };

               // }
               
               _loanRepository.Insert(_mapper.Map<Loan>(model));
               _loanRepository.Save();

                return new Response<LoanModel>
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

            return new Response<LoanModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<LoanModel> Update(LoanModel model)
        {
            try
            {
                var validationResult = _loanValidation.ValidateUpdate(model);
                if (!validationResult.IsValid)
                    return new Response<LoanModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                var updatingObj = _loanRepository.GetById(model.LoanId);
                updatingObj.StatusCode = model.StatusCode;
                updatingObj.DueDate = model.DueDate;
                updatingObj.DateApproved = model.DateApproved;
                _loanRepository.Update(updatingObj);
                _loanRepository.Save();

                return new Response<LoanModel>
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

            return new Response<LoanModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<LoanModel> Delete(string id)
        {
            try
            {
                //var validationResult = this.loanValidation.ValidateUpdate(model);
                //if (!validationResult.IsValid)
                //    return new Response<CustomerModel>
                //    {
                //        Message = validationResult.ErrorMessage,
                //        ResultType = ResultType.ValidationError
                //    };

                //var entity = mapper.Map<LoanModel>(model);
                _loanRepository.Delete(id);
                _loanRepository.Save();

                return new Response<LoanModel>
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

            return new Response<LoanModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<LoanModel> List()
        {
            var result = _loanRepository.GetAll();
            return result?.ProjectTo<LoanModel>();
        }

        public LoanModel Get(string id)
        {
            var result = _loanRepository.GetById(id);
            return result == null ? null : _mapper.Map<LoanModel>(result);
        }

        private LoanModel CustomerLastTransaction(LoanModel model)
        {
            var loans  = _loanRepository.GetAll()
                .OrderByDescending(x => x.DateCreated)
                .FirstOrDefault(x => x.CustomerId == model.CustomerId);

            return loans == null ? null : _mapper.Map<LoanModel>(loans);
        }

        private Product GetLoanProduct(string productId)
        {
            return _productRepository.GetById(productId);
        }
        
        public void Dispose()
        {
            _loanRepository?.Dispose();
        }
    }
}