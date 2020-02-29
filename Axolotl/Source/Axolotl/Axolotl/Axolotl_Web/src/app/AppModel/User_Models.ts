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
  rolename: string;
  reportingperson: string;
  totalmember: number;
}

export class SignUp_request {
  id: number;
  firstname: string;
  lastname: string;
  username: string;
  phonenumber: string;
  password: string;
  companyid: number;
  Shifttype: number;
  companyroleid: number;
  haveteam: boolean;
}

export class SignUp_response {
  result: ResultStatus;

  constructor() {
    this.result = new ResultStatus();
  }
}


export class GetUserDetail_Request {
  id: number;
}

export class GetUserDetail_Response {
  record: SignUp_request;
  result: ResultStatus;

  public constructor() {
    this.result = new ResultStatus();
    this.record = new SignUp_request();
  }
}

export class ReportingPerson_request {
  userid: number;
  companyid: number;
}

export class GetEmployeeWeekOffs_request {
  employeeid: number;
}

export class GetEmployeeWeekOffs_response {
  result: ResultStatus;
  records: GetEmployeeWeekOffs_weekdetail[];
  constructor() {
    this.result = new ResultStatus();
    this.records = new Array<GetEmployeeWeekOffs_weekdetail>();
  }
}

export class GetEmployeeWeekOffs_weekdetail {
  week: string;
  records: GetEmployeeWeekOffs_detail[];

  public constructor() {
    this.records = new Array<GetEmployeeWeekOffs_detail>();
  }
}

export class GetEmployeeWeekOffs_detail {
  weekno: number;
  week: string;
  day: string;
  id: number;
  isadd: boolean;
  employeeid: number;
  companyid: number;
  companyweekoffdays: number;
}
