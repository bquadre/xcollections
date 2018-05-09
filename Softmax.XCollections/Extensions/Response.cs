using Microsoft.AspNetCore.Identity;
using Softmax.XCollections.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Extensions
{
    /// <summary>
    /// Respnse used for returning a result object
    /// </summary>
    /// <typeparam name="T">The result object to return</typeparam>
    public class Response<T> : Response
        where T : class
    {
        /// <summary>
        /// Indicates if the response contains a result
        /// </summary>
        public override bool HasResult
        {
            get
            {
                return this.Result != null;
            }
        }

        /// <summary>
        /// The result of the response
        /// </summary>
        public new T Result
        {
            get
            {
                return base.Result as T;
            }

            set
            {
                base.Result = value;
            }
        }

        /// <summary>
        /// Creates a successful response with a given result object
        /// </summary>
        /// <param name="result">The result object to return with the response</param>
        /// <returns>The response object</returns>
        public static Response<T> Success(T result)
        {
            var response = new Response<T> { ResultType = ResultType.Success, Result = result };

            return response;
        }

        /// <summary>
        /// Creates a failed result. It takes no result object
        /// </summary>
        /// <param name="errorMessage">The error message returned with the response</param>
        /// <returns>The created response object</returns>
        public static new Response<T> Failed(string errorMessage)
        {
            var response = new Response<T> { ResultType = ResultType.Error, Message = errorMessage };

            return response;
        }

        /// <summary>
        /// Creates a warning result. The warning result is successful, but might have issues that should be addressed or logged
        /// </summary>
        /// <param name="warningMessage">The warning returned with the response</param>
        /// <param name="result">The result object</param>
        /// <returns>The created response object</returns>
        public static Response<T> Warning(string warningMessage, T result)
        {
            var response = new Response<T>
            {
                ResultType = ResultType.Warning,
                Message = warningMessage,
                Result = result
            };

            return response;
        }

        /// <summary>
        /// Creates an empty result. The empty result is successful, but might have issues that should be addressed or logged
        /// </summary>
        /// <returns>The created response object</returns>
        public static Response<T> Empty()
        {
            var response = new Response<T> { ResultType = ResultType.Empty };

            return response;
        }
    }

    /// <summary>
    /// A simple response object with no returned object. Just indicates successful or failed requests
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class. 
        /// </summary>
        public Response()
        {
            this.ResultType = ResultType.Success;
        }

        /// <summary>
        /// Indicates if the response contains a result
        /// </summary>
        public virtual bool HasResult
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The result of the response
        /// </summary>
        public object Result { get; protected set; }

        /// <summary>
        /// Indicates if the response is successful or not. Warning or success result type indicate success
        /// </summary>
        public bool Successful
        {
            get
            {
                return this.ResultType == ResultType.Success || this.ResultType == ResultType.Warning;
            }
        }

        /// <summary>
        /// The result type
        /// </summary>
        public ResultType ResultType { get; set; }

        /// <summary>
        /// The message returned with the response
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Creates a successful response with a given result object
        /// </summary>
        /// <returns>The response object</returns>
        public static Response Success()
        {
            var response = new Response { ResultType = ResultType.Success };

            return response;
        }

        /// <summary>
        /// Creates a failed result. It takes no result object
        /// </summary>
        /// <param name="errorMessage">The error message returned with the response</param>
        /// <returns>The created response object</returns>
        public static Response Failed(string errorMessage)
        {
            var response = new Response { ResultType = ResultType.Error, Message = errorMessage };

            return response;
        }

        /// <summary>
        /// Creates a validation error response, indicating the input was invalid
        /// </summary>
        /// <param name="validationMessage">The validation message</param>
        /// <returns>The Response object</returns>
        public static Response ValidationError(string validationMessage)
        {
            var response = new Response { ResultType = ResultType.ValidationError, Message = validationMessage };

            return response;
        }

        /// <summary>
        /// Creates a warning result. The warning result is successful, but might have issues that should be addressed or logged
        /// </summary>
        /// <param name="warningMessage">The warning returned with the response</param>
        /// <returns>The created response object</returns>
        public static Response Warning(string warningMessage)
        {
            var response = new Response { ResultType = ResultType.Warning, Message = warningMessage };

            return response;
        }

        

        /// <summary>
        /// Creates a Response object from an Identityresult
        /// </summary>
        /// <param name="identityResult">The identityresult to get response from</param>
        /// <returns>The Response object</returns>
        public static Response FromIdentityResult(IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
            {
                return Success();
            }

            var errors = string.Empty;

            if (identityResult.Errors != null)
            {
                errors = string.Concat(identityResult.Errors);
            }

            return Failed(errors);
        }

        /// <summary>
        /// Creates an empty result. The empty result is successful, but might have issues that should be addressed or logged
        /// </summary>
        /// <returns>The created response object</returns>
        public static Response Empty()
        {
            var response = new Response { ResultType = ResultType.Empty };

            return response;
        }
    }
}
