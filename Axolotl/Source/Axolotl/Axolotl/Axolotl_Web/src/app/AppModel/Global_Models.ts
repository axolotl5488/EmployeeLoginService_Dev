import { ResultStatus, Dropdown_Model } from '../AppModel/appmodel_Model'

export class GetCompanyList_drp_response {
  records: Dropdown_Model[];
  result: ResultStatus;

  public constructor()
{
    this.result = new ResultStatus();
    this.records = new Array<Dropdown_Model>();
}
    }
