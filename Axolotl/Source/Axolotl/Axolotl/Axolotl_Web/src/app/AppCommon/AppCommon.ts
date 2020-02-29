import '../../assets/scripts/notify.min.js';

import * as $ from 'jquery';

export class AppCommon {

    public static token: string = "";
    public static activetab: string = "";

    // Local
    public static TokenURL: string = "https://localhost:44324/Token";
    public static APIURL: string = "https://localhost:44324/API/WebAPI/";
    public static APIImageURL: string = "https://localhost:44324/Images/";

    // Server
    // ng build --prod --base-href /axolotl_web/
    //public static TokenURL: string = "http://nktpl.co.uk/";
   //public static APIURL: string = "http://nktpl.co.uk/API/WebAPI/";
    //public static APIImageURL: string = "http://nktpl.co.uk/Images/";

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



