using Softmax.XCollections.Data.Contracts;
using System;
using System.Linq;
using Softmax.XCollections.Extensions;
using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Data.Contracts.Services;
using Softmax.XCollections.Data.Contracts.Validations;
using Softmax.XCollections.Models;

namespace Softmax.XCollections.Services
{
    public class WithdrawalService : IWithdrawalService, IDisposable
    {
        private readonly IRepository<WithdrawalModel> withdrawalRepository;
        private readonly IWithdrawalValidation withdrawalValidation;

        public WithdrawalService(
              IRepository<WithdrawalModel> withdrawalRepository,
              IWithdrawalValidation withdrawalValidation
               )
        {
            this.withdrawalRepository = withdrawalRepository;
            this.withdrawalValidation = withdrawalValidation;
        }

         
        public Response<WithdrawalModel> Create(WithdrawalModel model)
        {
            try
            {
                var validationResult = this.withdrawalValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<WithdrawalModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                    this.withdrawalRepository.Insert(model);
                    this.withdrawalRepository.Save();

                return new Response<WithdrawalModel>
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

            return new Response<WithdrawalModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<WithdrawalModel> Update(WithdrawalModel model)
        {
            try
            {
                var validationResult = this.withdrawalValidation.ValidateUpdate(model);
                if (!validationResult.IsValid)
                    return new Response<WithdrawalModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                    this.withdrawalRepository.Update(model);
                    this.withdrawalRepository.Save();
               
                return new Response<WithdrawalModel>
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

            return new Response<WithdrawalModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<WithdrawalModel> Delete(string id)
        {
            try
            {
                //var validationResult = this.withdrawalValidation.ValidateUpdate(model);
                //if (!validationResult.IsValid)
                //    return new Response<CustomerModel>
                //    {
                //        Message = validationResult.ErrorMessage,
                //        ResultType = ResultType.ValidationError
                //    };

                //var entity = mapper.Map<LoanRequest>(model);
                this.withdrawalRepository.Delete(id);
                this.withdrawalRepository.Save();

                return new Response<WithdrawalModel>
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

            return new Response<WithdrawalModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<IQueryable<WithdrawalModel>> List(string search)
        {
            IQueryable<WithdrawalModel> request;
            try
            {

                if (string.IsNullOrEmpty(search))
                {
                    request = this.withdrawalRepository.GetAll();

                }
                else
                {
                    request = this.withdrawalRepository.GetAll();

                }

            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throw;
                return new Response<IQueryable<WithdrawalModel>>() { ResultType = ResultType.Error };
            }

            return new Response<IQueryable<WithdrawalModel>>() { ResultType = ResultType.Success, Result = request };
        }

        public Response<WithdrawalModel> Get(string id)
        {
            try
            {
                var request = this.withdrawalRepository.GetById(id);
                
                return new Response<WithdrawalModel>() { ResultType = ResultType.Success, Result = request };
            }
            catch (Exception ex)
            {
                //online error log
                var err = ex.Message;
            }

            return new Response<WithdrawalModel>() { ResultType = ResultType.Error };
        }

        public void Dispose()
        {
            withdrawalRepository?.Dispose();
        }    
    }
}