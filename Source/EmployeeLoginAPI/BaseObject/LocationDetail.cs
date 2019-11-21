using System.Collections.Generic;

namespace EmployeeLoginService.BaseObject
{
    public class LocationDetail
    {
        //public int BreakCategoryId { get; internal set; }

       // public string BreakCategoryName { get; internal set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public bool IsChecked { get; internal set; }

        public double Latitude { get; set; }

        public string LocationAddress { get; set; }

        public string LocationImage { get; set; }

        public int LocationId { get; set; }

        public double Longitude { get; set; }

        //public string LunchFrom { get; internal set; }

        //public string LunchTo { get; internal set; }

        public string PlaceName { get; set; }

        //public string TeaFrom { get; internal set; }

        //public string TeaTo { get; internal set; }

       // public int TotalBreakCount { get; internal set; }

       // public string TotalBreakCountWord { get; internal set; }

        public List<LocationPin> Lp { get; internal set; }
    }
}