using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;

public class PlatformUtility : SingletonBehaviour<PlatformUtility>
{
    static string EVENT_UTILITY_CLIPBOARD_CONTENT = "EVENT_UTILITY_CLIPBOARD"; // { content : "this is text" }
    static string EVENT_UTILITY_BATTERY_LEVEL_RESULT = "EVENT_UTILITY_BATTERY_LEVEL_RESULT" ; // { rate : 0.5 }

    static string SDK_UTILITY_READ_CLIPBOARD = "SDK_UTILITY_READ_CLIPBOARD" ;  
    static string SDK_UTILITY_SET_CLIPBOARD = "SDK_UTILITY_SET_CLIPBOARD" ;  // { content : "text" }
    static string SDK_UTILITY_READ_BATTERY_LEVEL = "SDK_UTILITY_READ_BATTERY_LEVEL" ;  

    public int readBatteryLevel()
    {
        return SDKHelp.sendRequestToPlatform(PlatformUtility.SDK_UTILITY_READ_BATTERY_LEVEL,null);
    }

    public void asyncReadClipBoard()
    {
        SDKHelp.sendRequestToPlatform(PlatformUtility.SDK_UTILITY_READ_CLIPBOARD,null);
    }

    public void setClipBoard( string content )
    {
        var js = new JSONObject();
        js["content"] = content ;
        SDKHelp.sendRequestToPlatform(PlatformUtility.SDK_UTILITY_SET_CLIPBOARD,js );
    }
}
