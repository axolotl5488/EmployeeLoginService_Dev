using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAModel.Model;

namespace BAModel.Model
{
    class GlobalModels
    {
    }

    public class GetCompanyList_drp_response
    {
        public List<Dropdown_Model> records { get; set; }
        public ResultStatus result { get; set; }

        public GetCompanyList_drp_response()
        {
            result = new ResultStatus();
            records = new List<Dropdown_Model>();
        }
    }
}
