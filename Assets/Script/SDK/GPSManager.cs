using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;

public class GPSManager : SingletonBehaviour<GPSManager>
{
    public static string EVENT_GPS_RESULT = "EVENT_GPS_RESULT" ; // { code : 2 , longitude : 23 , latitude : 2 , address : "" }
    
    static string SDK_GPS_REQUEST_GPSINFO = "SDK_GPS_REQUEST_GPSINFO" ; // { isNeedAddress : 1 } 
    static string SDK_GPS_CACULATE_DISTANCE = "SDK_GPS_CACULATE_DISTANCE" ; // { A_longitude : 23, A_latitude : 23 ,B_longitude : 23, B_latitude : 23 }
    // longitude = J (jing du )
    public void requestGPS( bool isNeedAddress = false )
    {
        var jsArg = new JSONObject() ;
        jsArg["isNeedAddress"] = isNeedAddress ? 1 : 0 ;
        SDKHelp.sendRequestToPlatform(GPSManager.SDK_GPS_REQUEST_GPSINFO,jsArg) ;
    }

    public int caculateDistance( double A_longitude , double A_latitude,double B_longitude, double B_latitude )
    {
        var jsArg = new JSONObject();
        jsArg["A_longitude"] = A_longitude;
        jsArg["A_latitude"] = A_latitude;

        jsArg["B_longitude"] = B_longitude;
        jsArg["B_latitude"] = B_latitude;
        return SDKHelp.sendRequestToPlatform(GPSManager.SDK_GPS_CACULATE_DISTANCE,jsArg) ;
    }
}
