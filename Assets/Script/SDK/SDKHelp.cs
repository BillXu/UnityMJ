using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;

public class SDKHelp : SingletonBehaviour<SDKHelp>
{
    // Start is called before the first frame update
 

    public void sdkSendEvent( string strResult )
    {
        var js = JSONObject.Parse(strResult);
        var eventID = js["eventID"].Str;
        EventDispatcher.getInstance().dispatch(eventID,js) ;
        Debug.Log( "大师的立即发货给sdkSendEvent" + eventID + " detail :" + strResult );
    }


    public static int sendRequestToPlatform( string eventID , JSONObject jsDetail )
    {
    

#if UNITY_EDITOR  
        //platform = "unity编辑模式";  
#elif UNITY_XBOX360  
       //platform="XBOX360平台";  
#elif UNITY_IPHONE  
       //platform="IPHONE平台";  
#elif UNITY_ANDROID  
       {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
            int ret = mainActivity.Call<int>("onRecievedUnityRequest", eventID,jsDetail == null ? "{ a : 0 }" : jsDetail.ToString() );
            if ( -99999999 == ret )
            {
                Debug.LogError("request not be processed by native = " + eventID );
            }
            return ret ;
       }
#elif UNITY_STANDALONE_OSX  
       //platform="OSX平台(mac)";  
#elif UNITY_STANDALONE_WIN  
       //platform="Windows平台"; 
#endif 
      return 0 ;
    }
}
