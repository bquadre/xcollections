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
    public class BranchService : IBranchService, IDisposable
    {
        private readonly IRepository<Branch> _branchRepository;
        private readonly IBranchValidation _branchValidation;
        private readonly IGenerator _generator;
        private readonly IMapper _mapper;
      
        public BranchService(IRepository<Branch> branchRepository,
              IBranchValidation branchValidation,
              IGenerator generator,
              IMapper mapper)
        {
            _branchRepository = branchRepository;
            _branchValidation = branchValidation;
            _generator = generator;
            _mapper = mapper;        
        }

        public Response<BranchModel> Create(BranchModel model)
        {
            try
            {
                var validationResult = _branchValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<BranchModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                   model.IsActive =true;
                   model.BranchCode = this.GenerateBranchCode();
                    _branchRepository.Insert(_mapper.Map<Branch>(model));
                    _branchRepository.Save();

                return new Response<BranchModel>
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

            return new Response<BranchModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<BranchModel> Update(BranchModel model)
        {
            try
            {
                var validationResult = _branchValidation.ValidateUpdate(model);
                if (!validationResult.IsValid)
                    return new Response<BranchModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                _branchRepository.Update(_mapper.Map<Branch>(model));
                _branchRepository.Save();

                return new Response<BranchModel>
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

            return new Response<BranchModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<BranchModel> Delete(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    return new Response<BranchModel>
                    {
                        ResultType = ResultType.ValidationError
                    };

                }
                var branch = _branchRepository.GetById(id);
                branch.IsDeleted = true;
                _branchRepository.Update(branch);
                _branchRepository.Save();

                return new Response<BranchModel>
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

            return new Response<BranchModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<BranchModel> List()
        { 
           var result = _branchRepository.GetAll();
                return result?.ProjectTo<BranchModel>();
        }
  
        public BranchModel Get(string id)
        {
             var result = _branchRepository.GetById(id);
                return result == null ? null : _mapper.Map<BranchModel>(result);
        }
            
        public void Dispose()
        {
            _branchRepository?.Dispose();
        }

        private string GenerateBranchCode()
        {
            var number = string.Empty;

            for (int i = 0; i < 1000; i++)
            {
                number = _generator.RandomNumber(100, 999);
                var check = _branchRepository.GetAll()
                    .Any(x => x.BranchCode.Equals(number));
                if (!check)
                    break;
            }

            return number;
        }
    }
}