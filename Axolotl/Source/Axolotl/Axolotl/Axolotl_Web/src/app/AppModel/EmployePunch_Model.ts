import { ResultStatus } from '../AppModel/appmodel_Model'

export class EmployeePunchList_Response {
    records: EmployeePunchList_Detail[];
    result: ResultStatus;

    public EmployeePunchList_Response() {
        this.result = new ResultStatus();
        this.records = new Array<EmployeePunchList_Detail>();
    }
}

export class EmployeePunchList_Detail {
    id: number;
    date: string;
    userid: number;
    totaltasks: number;
    workinghours: string;
    username: string;
    clockintime: string;
    clockouttime: string;
    clockinlatitude: number;
    clockinlongitude: number;
    latecomer: boolean;
    earlyouter: boolean;
    latecomerreason: string;
    earlyouterreason: string;
    issystemclockout: boolean;
    clockoutlatitude: number;
    clockoutlongitude: number;
}
