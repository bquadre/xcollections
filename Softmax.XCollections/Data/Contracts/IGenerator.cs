using Softmax.XCollections.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Data.Contracts
{
    public interface IGenerator
    {
        /// <summary>
        /// Generates guid.
        /// </summary>
        /// <returns> Returns generated unique guid </returns>
       string GenerateGuid();

        /// <summary>
        /// Generates random numbers.
        /// </summary>
        /// <returns> Returns generated unique guid </returns>
        string RandomNumber(int min, int max);


        string DateCodeString();

        string TempPassword(int num);
    }

}
