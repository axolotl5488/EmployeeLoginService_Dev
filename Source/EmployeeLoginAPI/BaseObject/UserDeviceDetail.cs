using System;

namespace EmployeeLoginService.BaseObject
{
    public class UserDeviceDetail
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string DeviceId { get; set; }

        public string Description { get; set; }

        public string DeviceType { get; set; }

        public bool IsApproved { get; set; }

        public int UDId { get; set; }

        public int CompanyId { get; internal set; }

        public string CompanyName { get; internal set; }

        public string ApprovedBy { get; internal set; }

        public string OSVersion { get; internal set; }

        public string ApprovedDate { get; internal set; }

        public string DeviceName { get; internal set; }
        public string LocationName { get; internal set; }

        public string IPAddress { get; internal set; }

    }
}