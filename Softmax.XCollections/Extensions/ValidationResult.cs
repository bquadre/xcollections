using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Softmax.XCollections.Extensions
{
    /// <summary>
    /// Class containing validation details
    /// </summary>
    public class ValidationResult
    {
        private List<string> errorMessages;

        /// <summary>
        /// Creates a successful validationresult
        /// </summary>
        public static ValidationResult Success
        {
            get
            {
                return new ValidationResult();
            }
        }

        /// <summary>
        /// A list of all the error messages
        /// </summary>
        public List<string> ErrorMessages
        {
            get
            {
                if (this.errorMessages == null)
                {
                    this.errorMessages = new List<string>();
                }

                return this.errorMessages;
            }
        }

        /// <summary>
        /// Gets the error messages as one string
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                if (this.errorMessages == null || this.errorMessages.Count == 0)
                {
                    return string.Empty;
                }

                var stringBuilder = new StringBuilder();

                for (var i = 0; i < this.errorMessages.Count; i++)
                {
                    if ((i + 1) < this.errorMessages.Count)
                    {
                        stringBuilder.AppendLine(this.errorMessages[i]);
                    }
                    else
                    {
                        stringBuilder.Append(this.errorMessages[i]);
                    }
                }

                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// If the validationresult contains error messages, IsValid is false
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.errorMessages == null || this.errorMessages.Count == 0;
            }
        }

        /// <summary>
        /// Creates a validationresult with an error message
        /// </summary>
        /// <param name="errorMessage">The error message to create with</param>
        /// <returns>The validationresult</returns>
        public static ValidationResult Failed(string errorMessage)
        {
            var validationResult = new ValidationResult();

            validationResult.ErrorMessages.Add(errorMessage);

            return validationResult;
        }

        /// <summary>
        /// Adds an error message to the list
        /// </summary>
        /// <param name="errorMessage">The error message to add</param>
        public void AddErrorMessage(string errorMessage)
        {
            this.ErrorMessages.Add(errorMessage);
        }
    }
}
