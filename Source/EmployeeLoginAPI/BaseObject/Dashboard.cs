using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeLoginService.BaseObject
{
    public class Dashboard
    {
        public double Punchin { get; internal set; }
        public double OutsidePunchin { get; internal set; }
        public double LatePunchin { get; internal set; }
        public double EarlyPounchout { get; internal set; }
        public int PunchinCount { get; internal set; }
        public int OutsidePunchinCount { get; internal set; }
        public int LatePunchinCount { get; internal set; }
        public int EarlyPounchoutCount { get; internal set; }
        public double RepPunchin { get; internal set; }
        public double RepOutsidePunchin { get; internal set; }
        public double RepLatePunchin { get; internal set; }
        public double RepEarlyPounchout { get; internal set; }
        public double FinalRepPer { get; internal set; }

    }
}