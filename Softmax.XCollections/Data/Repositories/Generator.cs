using Softmax.XCollections.Data.Contracts;
using System;

namespace Softmax.XCollections.Data.Repositories
{
    public class Generator : IGenerator
    {
        public string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public string RandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max).ToString();
        }

        public string DateCodeString()
        {
                var date = DateTime.Now;
                var year = date.Year;
                var month = date.Month;
                var day = date.Day;

                var mDay = string.Empty;
                var mMonth = string.Empty;

                mMonth = month < 10 ? "0" + month : month.ToString();
                mDay = day < 10 ? "0" + day : day.ToString();
                return year + mMonth + mDay;
        }

        public string TempPassword(int num)
        {
            var password = GenerateGuid()
                .Substring(0, num);
           return "Pass" + password + "@1";
        }
    }
}
