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
  taskrecords: PunchTask_Model[];
  public constructor() {
    this.taskrecords = new Array<PunchTask_Model>();
  }
}

export class PunchTask_Model {
  taskid: number;
  Task: string;
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
