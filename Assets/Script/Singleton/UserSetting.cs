using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
public class UserSetting : SingetongBehaviour<UserSetting>
{
    JSONObject mJsContent = null;
    // Start is called before the first frame update
    public string localAccount
    {
        get
        {
            if ( mJsContent == null || mJsContent.ContainsKey("acc") == false )
            {
                return null ;
            }
            return mJsContent["acc"].Str ;
        }

        set
        {
            mJsContent["acc"] = value ;
        }
    }

    public string localPassword 
    {
        get
        {
            if ( mJsContent == null || mJsContent.ContainsKey("pwd") == false )
            {
                return null ;
            }
            return mJsContent["pwd"].Str ;
        }

        set
        {
            mJsContent["pwd"] = value ;
        }
    }
    
    
    
    
    private new void Awake() {
        base.Awake();
        var str = PlayerPrefs.GetString("userSetting");
        if ( str != null )
        {
            this.mJsContent = JSONObject.Parse(str);
            Debug.Log("parsed user setting " + ( this.mJsContent == null ? " result null" : " ok " ) );
        }
        else
        {
            Debug.LogWarning("user settting str is null just new one");
        }

        if ( this.mJsContent == null )
        {
            this.mJsContent = new JSONObject();
        }
    }

    public void clearUserSetting()
    {
        if ( this.mJsContent != null )
        {
            this.mJsContent.Clear();
        }
    }
    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log("UserSetting  OnApplicationPause");
        if ( pauseStatus )
        {
            this.doSave();
        }
    }

    private void OnApplicationQuit() {
        Debug.Log("UserSetting  OnApplicationQuit");
        this.doSave();
    }

    private void doSave()
    {
        var str = this.mJsContent.ToString();
        if ( str.Length < 10 )
        {
            // means empty almost
            PlayerPrefs.DeleteKey("userSetting") ;
        }
        else
        {
            PlayerPrefs.SetString("userSetting",str) ;
        }
    }
}
