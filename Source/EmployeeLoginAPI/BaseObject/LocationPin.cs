using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class LocationPin
    {
        public string Lat { get; internal set; }
        public string Long { get; internal set; }
        public bool Status { get; internal set; }
        public string PinAddress { get; internal set; }

        public int LocationID { get; internal set; }

       

    }
}