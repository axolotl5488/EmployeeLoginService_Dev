using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAModel.Common
{
   public static class Config
    {
        public static long GetTimeStamp(DateTime datetime)
        {
            return new DateTimeOffset(datetime).ToUnixTimeSeconds();
        }
    }
}
