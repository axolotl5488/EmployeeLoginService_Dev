import { ResultStatus, AppCommonResponse } from '../AppModel/appmodel_Model'

export class EmployeePunchList_Request {
  companyid: number;
  userid: number;
}
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
  taskrecords: PunchTask_Model[];
  callrecords: GetEmployeeCallList_detail_Webportal[];
  public constructor() {
    this.taskrecords = new Array<PunchTask_Model>();
    this.callrecords = new Array<GetEmployeeCallList_detail_Webportal>();
  }
}

export class PunchTask_Model {
  taskid: number;
  Task: string;
  TaskStatus: string;
  TaskStatusID: number;
}


export class GetEmployeePunchDetailWeb_request {
  punchid: number;
}

export class GetEmployeePunchDetailWeb_response {
  result: ResultStatus;

  record: EmployeePunchList_Detail;

  public constructor() {
    this.result = new ResultStatus();
    this.record = new EmployeePunchList_Detail();
  }
}


export class GetEmployeeCallList_detail_Webportal {
  id: number;
  companyid: number;
  punchid: number;
  start_lat: number;
  start_lng: number;
  title: string;
  callfor: string;
  remarks: string;
  start_datetime_timestamp: string;
  _end_lat?: number;
  _end_lng?: number;
  _end_datetime_timestamp?: string;
}
