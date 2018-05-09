using AutoMapper;
using Softmax.XCollections.Data.Contracts;
using Softmax.XCollections.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Softmax.XCollections.Models;
using Softmax.XCollections.Extensions;
using Softmax.XCollections.Data.Enums;
using Softmax.XCollections.Data.Contracts.Services;
using Softmax.XCollections.Data.Contracts.Validations;

namespace Softmax.XCollections.Services
{
    public class ProductService : IProductService, IDisposable
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IProductValidation _productValidation;
        private readonly IGenerator _generator;
        private readonly IMapper _mapper;
      

        public ProductService(IRepository<Product> productRepository,
              IProductValidation productValidation,
              IGenerator generator,
              IMapper mapper)
        {
            _productRepository = productRepository;
            _productValidation = productValidation;
            _generator = generator;
            _mapper = mapper;
        }

        public Response<ProductModel> Create(ProductModel model)
        {
            try
            {
                var validationResult = _productValidation.ValidateCreate(model);
                if (!validationResult.IsValid)
                    return new Response<ProductModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };
                    model.IsActive = true;
                    _productRepository.Insert(_mapper.Map<Product>(model));
                    _productRepository.Save();

                return new Response<ProductModel>
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

            return new Response<ProductModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<ProductModel> Update(ProductModel model)
        {
            try
            {
                var validationResult = _productValidation.ValidateUpdate(model);
                if (!validationResult.IsValid)
                    return new Response<ProductModel>
                    {
                        Message = validationResult.ErrorMessage,
                        ResultType = ResultType.ValidationError
                    };

                _productRepository.Update(_mapper.Map<Product>(model));
                _productRepository.Save();
               
                return new Response<ProductModel>
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

            return new Response<ProductModel>
            {
                ResultType = ResultType.Error
            };
        }

        public Response<ProductModel> Delete(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    return new Response<ProductModel>
                    {
                        ResultType = ResultType.ValidationError
                    };

                }
                var product = _productRepository.GetById(id);
                product.IsDeleted = true;
                product.IsActive = false;
                _productRepository.Update(product);
                _productRepository.Save();

                return new Response<ProductModel>
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

            return new Response<ProductModel>
            {
                ResultType = ResultType.Error
            };
        }

        public IQueryable<ProductModel> List()
        {
            var result = _productRepository.GetAll();
            return result?.ProjectTo<ProductModel>();
        }

        public ProductModel Get(string id)
        {
            var result = _productRepository.GetById(id);
            return _mapper.Map<ProductModel>(result);

        }

        public void Dispose()
        {
            _productRepository?.Dispose();
        }    
    }
}