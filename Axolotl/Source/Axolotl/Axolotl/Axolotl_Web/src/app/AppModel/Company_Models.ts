import { ResultStatus, AppCommonResponse } from '../AppModel/appmodel_Model'

export class GetCompany_Response {
  records: GetCompany_Detail[];
  result: ResultStatus;

  public constructor() {
    this.result = new ResultStatus();
    this.records = new Array<GetCompany_Detail>();
  }
}

export class GetCompany_Detail {
  id: number;
  totalusers: number;
  datecreated: string;
  name: string;
  mobile: string;
  address: string;
  state: string;
  city: string;
  zipcode: string;
  flexiblebufferminutes: number;
  noofweekoffdays: number;
  workinghoursinminutes: number;
  isdelete: boolean;
}



export class ManageCompany_Request {
  id: number;
  name: string;
  mobile: string;
  address: string;
  city: string;
  state: string;
  zipcode: string;
  flexiblebufferminutes: number;
  noofweekOffdays: number;
  workinghours: number;
  punchrangeinmeter: number;
}

export class ManageCompany_Response {
  result: ResultStatus;

  public constructor() {
    this.result = new ResultStatus();
  }
}

export class GetCompanyDetail_Request {
  id: number;
}

export class GetCompanyDetail_Response {
  record: ManageCompany_Request;

  result: ResultStatus;

  public constructor() {
    this.result = new ResultStatus();
    this.record = new ManageCompany_Request();
  }
}



export class GetCompanyLocationDetail_request {
  locationid: number;
}

export class GetCompanyLocationDetail_response {
  result: ResultStatus;

  record: ManageCompanyLocation_request;
  public constructor() {
    this.result = new ResultStatus();
    this.record = new ManageCompanyLocation_request();
  }
}
export class ManageCompanyLocation_request {
  companyid: number;
  id: number;
  name: string;
  address: string;
  state: string;
  city: string;
  zipcode: string;
  lat: number;
  lng: number;

}

export class ManageCompanyLocation_response {
  result: ResultStatus;

  public constructor() {
    this.result = new ResultStatus();
  }
}

export class GetCompanyLocaitonList_request {
  companyid: number;
}

export class GetCompanyLocaitonList_response {
  result: ResultStatus;

  records: GetCompanyLocaitonList_Detail[];
  public constructor() {
    this.result = new ResultStatus();
    this.records = new Array<GetCompanyLocaitonList_Detail>();
  }
}

export class GetCompanyLocaitonList_Detail {
  id: number;
  name: string;
  address: string;
  state: string;
  city: string;
  zipcode: string;
  lat: number;
  lng: number;
  isdeleted: boolean;
}
