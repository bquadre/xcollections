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
    public class CustomerService : ICustomerService, IDisposable
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly ICustomerValidation _customerValidation;
        private readonly IMapper _mapper;

        public CustomerService(
            IRepository<Customer> customerRepository,
            ICustomerValidation customerValidation,
            IMapper mapper)
        {
            _customerRepository = customerRepository;
            _customerValidation = customerValidation;
            _mapper = mapper;
        }


        public Response<CustomerModel> Create(CustomerModel model)
        {
            try
            {
                var validationResult = _customerValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<CustomerModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                //model.DateCreated = DateTime.UtcNow;
                _customerRepository.Insert(_mapper.Map<Customer>(model));
                _customerRepository.Save();

                return new Response<CustomerModel>
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

            return new Response<CustomerModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<CustomerModel> Update(CustomerModel model)
        {
            try
            {
                var validationResult = _customerValidation.ValidateUpdate(model);
                if (!validationResult.IsValid)
                    return new Response<CustomerModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                _customerRepository.Update(_mapper.Map<Customer>(model));
                _customerRepository.Save();

                return new Response<CustomerModel>
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

            return new Response<CustomerModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<CustomerModel> Delete(string id)
        {
            try
            {
                //var validationResult = this.customerValidation.ValidateUpdate(model);
                //if (!validationResult.IsValid)
                //    return new Response<CustomerModel>
                //    {
                //        Message = validationResult.ErrorMessage,
                //        ResultType = ResultType.ValidationError
                //    };

                //var entity = mapper.Map<CustomerModel>(model);
                _customerRepository.Delete(id);
                _customerRepository.Save();

                return new Response<CustomerModel>
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

            return new Response<CustomerModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<CustomerModel> List()
        {
            var result = _customerRepository.GetAll();
            return result?.ProjectTo<CustomerModel>();
        }

        public CustomerModel Get(string id)
        {
            var result = _customerRepository.GetById(id);
            return result == null ? null : _mapper.Map<CustomerModel>(result);
        }
        public void Dispose()
        {
            _customerRepository?.Dispose();
        }
    }
}