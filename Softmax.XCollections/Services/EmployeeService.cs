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
    public class EmployeeService : IEmployeeService, IDisposable
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IEmployeeValidation _employeeValidation;
        private readonly IMapper _mapper;

        public EmployeeService(
            IRepository<Employee> employeeRepository,
            IEmployeeValidation employeeValidation,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _employeeValidation = employeeValidation;
            _mapper = mapper;
        }

        public Response<EmployeeModel> Create(EmployeeModel model)
        {
            try
            {
                var validationResult = _employeeValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<EmployeeModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                     //model.DateCreated = DateTime.UtcNow;
                    _employeeRepository.Insert(_mapper.Map<Employee>(model));
                    _employeeRepository.Save();

                return new Response<EmployeeModel>
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

            return new Response<EmployeeModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<EmployeeModel> Update(EmployeeModel model)
        {
            try
            {
                var validationResult = _employeeValidation.ValidateUpdate(model);
                if (!validationResult.IsValid)
                    return new Response<EmployeeModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

             
                _employeeRepository.Update(_mapper.Map<Employee>(model));
                _employeeRepository.Save();

                return new Response<EmployeeModel>
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

            return new Response<EmployeeModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<EmployeeModel> Delete(string id)
        {
            try
            {
                //var validationResult = this.employeeValidation.ValidateUpdate(model);
                //if (!validationResult.IsValid)
                //    return new Response<EmployeeModel>
                //    {
                //        Message = validationResult.ErrorMessage,
                //        ResultType = ResultType.ValidationError
                //    };

                //var entity = mapper.Map<EmployeeModel>(model);
                _employeeRepository.Delete(id);
                _employeeRepository.Save();

                return new Response<EmployeeModel>
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

            return new Response<EmployeeModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<EmployeeModel> List()
        {
            var result = _employeeRepository.GetAll();
            return result.ProjectTo<EmployeeModel>();
        }

        public EmployeeModel Get(string id)
        {
            var result = _employeeRepository.GetById(id);
            return _mapper.Map<EmployeeModel>(result);
        }

        public void Dispose()
        {
            _employeeRepository?.Dispose();
        }    
    }
}