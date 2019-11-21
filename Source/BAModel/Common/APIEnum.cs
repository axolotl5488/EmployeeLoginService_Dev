using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAModel.Common
{
    public static class APIEnum
    {
        public enum MasterLeavetype
        {
            CasualLeave = 1,
            DutyLeave = 2,
            SickLeave = 3,
            LeaveWithoutPay = 4,
        }

        public enum Leavetype
        {
            FullLeave = 1,
            FirstHalfLeave = 2,
            SecondHalfLeave = 3,
        }

        public enum LeaveApprovedStatus
        {
            Pending = 1,
            Sanctioned = 2,
            Canceled = 3,
            Reverted = 4,
            Rejected = 5
        }

        public enum Punch_InType
        {
            InSide = 0,
            OurSide = 1,
        }

        public enum Punch_OutType
        {
            InSide = 0,
            OurSide = 1,
        }

    }
}
