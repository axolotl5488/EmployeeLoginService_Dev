using System;

namespace EmployeeLoginService.BaseObject
{
    public class HolidayDetail
    {
        public int HolidayId { get; set; }

        public string HolidayDate { get; set; }

        public string Description { get; set; }

        public DateTime TempDate { get; set; }

        public string CompanyName { get; internal set; }
    }
}