import { ResultStatus } from '../AppModel/appmodel_Model'

export class GetUserList_Response {
    records: GetUserList_Detail[];
    result: ResultStatus;

    public constructor() {
        this.result = new ResultStatus();
        this.records = new Array<GetUserList_Detail>();
    }
}

export class GetUserList_Detail {
    id: number;
    datecreated: string;
    fullname: string;
    devicetype: string;
    userphotourl: string;
    companyid: number;
    totallatecommer: number;
    totalearlyout: number;
    companyname: string;
    isdeleted: boolean;
    phonenumber: string;
    officeshifttype: string;
}