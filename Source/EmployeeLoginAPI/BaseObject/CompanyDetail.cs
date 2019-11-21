namespace EmployeeLoginService.BaseObject
{
    public class CompanyDetail
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string IsActive { get; set; }

        public bool Active { get; set; }

        public string DefaultFrom { get; internal set; }

        public string DefaultTo { get; internal set; }

        public string LunchFrom { get; internal set; }

        public string LunchTo { get; internal set; }

        public string TeaFrom { get; internal set; }

        public string TeaTo { get; internal set; }

        public int BreakCategoryId { get; internal set; }

        public string BreakCategoryName { get; internal set; }

        public int TotalBreakCount { get; internal set; }

        public string TotalBreakCountWord { get; internal set; }

        public string Weekoff { get; internal set; }

        public int UserLimit { get; internal set; }
    }
}