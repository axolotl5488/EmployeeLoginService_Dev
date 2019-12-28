export class AppModel {
    activeTab: string;
}

//ng build --prod --base-href /


export class ResultStatus {
    status: boolean;

    message: string;
}

export class Dropdown_Model {
  id: number;
   name: string;
    }



export class AppCommonResponse {
  result: ResultStatus;

  public constructor() {
    this.result = new ResultStatus();
  }
}
