using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAModel.Common;
using BAModel.Model;
using DataModel;
using Newtonsoft.Json;
using System.Data.Entity;
using static BAModel.Common.Common;

namespace BAModel.Service
{
    public static class GlobalService
    {
        public static GetCompanyList_drp_response GetCompanyList_drp()
        {
            DateTime starttime = DateTime.UtcNow;
            DateTime endtime = DateTime.UtcNow;
            GetCompanyList_drp_response response = new GetCompanyList_drp_response();
            try
            {
                AxolotlEntities db = new AxolotlEntities();
                List<Company> records = db.Companies.Where(x => x.IsDelete == false).OrderBy(x => x.Name).ToList();
                foreach(Company obj in records)
                {
                    Dropdown_Model map = new Dropdown_Model();
                    map.id = obj.ID;
                    map.name = obj.Name;

                    response.records.Add(map);
                }

                response.result.message = "";
                response.result.status = true;
            }
            catch (Exception ex)
            {
                response.result.message = "something went wrong!";
                response.result.status = false;
            }


            return response;
        }
    }
}
