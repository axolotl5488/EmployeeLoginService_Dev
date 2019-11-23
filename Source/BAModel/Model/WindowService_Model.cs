
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAModel.Model
{
    class WindowService_Model
    {
    }

    public class DailyUserReport_Model
    {
        public int TotalIn { get; set; }
        public int TotalOut { get; set; }
        public int TotalInType { get; set; }
        public int TotalInLocation { get; set; }
        public int TotalLateIn { get; set; }
        public int TotalOutType { get; set; }
        public int TotalOutLocation { get; set; }
        public int TotalEarlyOut { get; set; }
        public int TotalSystemOut { get; set; }

        public List<DailyUserReport_Detail_Model> records { get; set; }

        public DailyUserReport_Model()
        {
            records = new List<DailyUserReport_Detail_Model>();
        }
    }

    public class DailyUserReport_Detail_Model
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public PunchIn record { get; set; }
        public DailyUserReport_Detail_Model()
        {
            record = new PunchIn();
        }
    }
}
