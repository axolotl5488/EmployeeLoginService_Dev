using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAModel.Model
{
    class AppModel
    {
    }

    public class AppCommonResponse
    {
        public ResultStatus result { get; set; }

        public AppCommonResponse()
        {
            result = new ResultStatus();
        }
    }

    public class ResultStatus
    {
        public bool status { get; set; }

        public string message { get; set; }
    }
    

    public class Dropdown_Model
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
