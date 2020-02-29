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
  IsCompanyhasAdmin: boolean;
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



export class MangeCompantHolidays_request {
  id: number;

  companyid: number;

  date: string;

  Name: string;

  description: string;

}

export class GetCompanyHolidayDetail_request {
  id: number;
}


export class GetCompanyHolidayDetail_response {
  record: MangeCompantHolidays_request;

  result: ResultStatus;

  public GetCompanyHolidayDetail_response() {
    this.result = new ResultStatus();
    this.record = new MangeCompantHolidays_request();
  }
}

export class GetCompanyHolidayList_request {
  companyid: number;
}

export class GetCompanyHolidayList_response {
  result: ResultStatus;

  records: GetCompanyHolidayList_yeardetail[];

  public GetCompanyHolidayList_response() {
    this.result = new ResultStatus();
    this.records = new Array<GetCompanyHolidayList_yeardetail>();
  }
}
export class GetCompanyHolidayList_yeardetail {
  year: string;

  records: GetCompanyHolidayList_detail[];

  public GetCompanyHolidayList_yeardetail() {
    this.records = new Array<GetCompanyHolidayList_detail>();
  }
}

export class GetCompanyHolidayList_detail {
  id: number;
  companyid: number;
  companyname: string;
  date: string;
  Name: string;
  description: string;
  isactive: boolean;
  year: string;
}


export class GetCompanyRolesList_request {
  companyid: number;
}

export class GetCompanyRolesList_response {
  records: GetCompanyRolesList_detail[];
  result: ResultStatus;

  public GetCompanyRolesList_response() {
    this.result = new ResultStatus();
    this.records = new Array<GetCompanyRolesList_detail>();
  }
}

export class GetCompanyRolesList_detail {
  id: number;
  lastmodified: number;
  companyid: number;
  totalmembers: number;
  name: number;
  description: number;
  isactive: number;
}

export class ManageCompanyRoles_request {
  id: number;
  companyid: number;
  name: number;
  description: number;
}

export class GetCompanyRoleDetail_request {
  id: number;
}

export class GetCompanyRoleDetail_response {
  record: ManageCompanyRoles_request;

  result: ResultStatus;

  public GetCompanyRoleDetail_response() {
    this.result = new ResultStatus();
    this.record = new ManageCompanyRoles_request();
  }
}

export class GetCompanyRolesPermissionList_request {
  companyid: number;
}

export class GetCompanyRolesPermissionList_response {
  records: GetCompanyRolesPermissionList_Role_detail[];

  result: ResultStatus;

  public GetCompanyRolesPermissionList_response() {
    this.result = new ResultStatus();
    this.records = new Array<GetCompanyRolesPermissionList_Role_detail>();
  }
}

export class GetCompanyRolesPermissionList_Role_detail {
  id: number;
  companyid: number;
  name: string;

  items: GetCompanyRolesPermissionList_RolePermission_detail[];

  public GetCompanyRolesPermissionList_Role_detail() {
    this.items = new Array<GetCompanyRolesPermissionList_RolePermission_detail>();
  }
}

export class GetCompanyRolesPermissionList_RolePermission_detail {
  id: number;
  lastmodified: string;
  screenname: string;
  screenid: number;
  companyid: number;
  companyroleid: number;

  isactive: boolean;
}
export class ManageCompanyRolesPermission_request {
  companyid: number;
  companyroleid: number;
  companyrolepermissionid: number;
  screenid: number;
}


export class ManageTeamList_request {
  companyid: number;
}

export class ManageTeamList_response {
  records: ManageTeamList_detail[];
  reportingusers: ManageTeamList_reporting_person_detail[];
  roles: ManageTeamList_reporting_role_detail[];
  result: ResultStatus;

  ManageTeamList_response() {
    this.result = new ResultStatus();
    this.records = new Array<ManageTeamList_detail>();
    this.reportingusers = new Array<ManageTeamList_reporting_person_detail>();
    this.roles = new Array<ManageTeamList_reporting_role_detail>();
  }
}

export class ManageTeamList_detail {
  userid: number;
  name: string;
  roleid: number;
  rolename: string;
  totalmember: number;
  reportingperson?: number;
  reportingpersonname: string;

}

export class ManageTeamList_reporting_person_detail {
  userid: number;
  name: string;
}

export class ManageTeamList_reporting_role_detail {
  roleid: number;
  name: string;
}

export class ManageTeam_request {
  userid: number;
  roleid: number;
  reportingpersonid?: number;
}
