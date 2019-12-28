import '../../assets/scripts/notify.min.js';

import * as $ from 'jquery';

export class AppCommon {

    // Local
    //public static APIURL: string = "https://localhost:44324/API/WebAPI/";

    // Server
    // ng build --prod --base-href /axolotl_web/
   public static APIURL: string = "http://nktpl.co.uk/API/WebAPI/";

    static SuccessNotify(message: string): void {
        $.notify(message, 'success');
    }

    static DangerNotify(message: string): void {
        $.notify(message, 'danger');
    }

    static WarningNotify(message: string): void {
        $.notify(message, 'warning');
    }

    static InfoNotify(message: string): void {
        $.notify(message, 'info');
    }

}



