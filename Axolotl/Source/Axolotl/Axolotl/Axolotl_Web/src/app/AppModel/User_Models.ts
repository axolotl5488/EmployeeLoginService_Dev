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

export class SignUp_request {
  id: number;
  firstname: string;
  lastname: string;
  username: string;
  phonenumber: string;
  password: string;
  companyid: number;
  Shifttype: number;
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

