import { ResultStatus, AppCommonResponse } from '../AppModel/appmodel_Model'




export class EmployeeleaveList_Response {
    records: EmployeeleaveList_Detail[];
    result: ResultStatus;

    public EmployeeleaveList_Response() {
        this.result = new ResultStatus();
        this.records = new Array<EmployeeleaveList_Detail>();
    }
}

export class EmployeeleaveList_Detail {
    id: number;
    companyname: string;
    companyid: number;
    fromdate: string;
    todate: string;
    userid: string;
    username: string;
    leavetypeid: number;
    leavetype: string;
    daytypeid: number;
    daytype: string;
    leavestatusid: number;
    leavestatus: string;
    ispaidleave: boolean;
    userremarkd: string;
    approvalremarks: string;
    totaldays: number;

}


export class EmployeeleaveList_request {
    companyid: number;
    userid: number;
}
