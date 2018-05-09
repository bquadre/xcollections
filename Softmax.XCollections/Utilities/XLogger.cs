using Softmax.XCollections.Data.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Softmax.XCollections.Utilities
{
    public class XLogger : IXLogger
    {

        public void xLog(string message)
        {
            var filePath = Path.Combine(
               Directory.GetCurrentDirectory(), "wwwroot/docs",
               "xlog.txt");
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.WriteLine(DateTime.Now.ToLongDateString() + " " +message);
                streamWriter.Close();
            }

        }
    }
}
