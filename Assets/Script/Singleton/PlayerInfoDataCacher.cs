using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using System ;
public class PlayerInfoData {

    protected JSONObject playerBrifeDataMsg = null ;
    
    public void initByMsg( JSONObject js )
    {
        this.playerBrifeDataMsg = js ;
    }

    public int uid
    {
        get
        {
            return (int)this.playerBrifeDataMsg["uid"].Number;
        }
    }

    public string name
    {
        get 
        {
            return this.playerBrifeDataMsg["name"].Str ;
        }
        
        set 
        {
            this.playerBrifeDataMsg["name"] = value ;
        }
    }

    public string headUrl
    {
        get 
        {
            return this.playerBrifeDataMsg["headIcon"].Str;
        }

        set 
        {
            this.playerBrifeDataMsg["headIcon"] = value ;
        }
    }

    public eSex gender
    {
        get
        {
            return (eSex)this.playerBrifeDataMsg["sex"].Number ;
        }

        set 
        {
            this.playerBrifeDataMsg["sex"] = (int)value;
        }
    }

    public string ip
    {
        get
        {
            return this.playerBrifeDataMsg["ip"].Str ;
        }
       
        set 
        {
            this.playerBrifeDataMsg["ip"] = value ;
        }
    }

    public double GPS_J
    {
        get
        {
            return this.playerBrifeDataMsg["J"].Number ;
        }

        set 
        {
            this.playerBrifeDataMsg["J"] = value ;
        }
        
    }
    public double GPS_W
    {
        get
        {
            return this.playerBrifeDataMsg["W"].Number ;
        }
        
        set 
        {
            this.playerBrifeDataMsg["W"] = value ;
        }
    }

    public string address
    {
         get
        {
            return this.playerBrifeDataMsg["address"].Str ;
        }
        
        set 
        {
            this.playerBrifeDataMsg["address"] = value ;
        }       
    }
}

public class PlayerInfoDataCacher : SingletonBehaviour<PlayerInfoDataCacher>
{
    public static string EVENT_RECIEVED_PLAYER_INFO_DATA = "RECEIVED_player_info_data" ;

    protected Dictionary<int,PlayerInfoData> vPlayerInfos = new Dictionary<int,PlayerInfoData>();
    protected Dictionary<int,float> vRequestingUIDs = new Dictionary<int,float>();
    private JSONObject jsMsgProxy = new JSONObject();

    private void Start() {
        Invoke("retryRequesting",3); 
    }
    public PlayerInfoData getPlayerInfoByID( int uid, bool isForceReq = false )
    {
        if ( this.vPlayerInfos.ContainsKey(uid) == false || isForceReq )
        {
            if ( vRequestingUIDs.ContainsKey(uid) )
            {
                Debug.LogWarning( "already requesting palyer info data uid = " + uid );
                return null ;
            }

            this.vRequestingUIDs[uid] = Time.realtimeSinceStartup;
            doRequestData(uid);
        }

        if ( this.vPlayerInfos.ContainsKey(uid) )
        {
            return this.vPlayerInfos[uid] ;
        }
        return null ;
    }

    protected void retryRequesting()
    {
        Debug.Log( "start play data info retry " );
        foreach ( var pair in this.vRequestingUIDs )
        {
            if ( Time.realtimeSinceStartup - pair.Value > 3 )
            {
                Debug.Log( "retry requesting player info data uid = " + pair.Key );
                doRequestData(pair.Key);
                this.vRequestingUIDs[pair.Key] = Time.realtimeSinceStartup;
            }
        }
    }

    void doRequestData( int uid )
    {
        this.jsMsgProxy["nReqID"] = uid ;
        this.jsMsgProxy["isDetail"] = 1 ;
        var self = this ;
        Network.getInstance().sendMsg(jsMsgProxy,eMsgType.MSG_REQUEST_PLAYER_DATA,eMsgPort.ID_MSG_PORT_DATA,uid,( JSONObject jsmsg )=>{
            
            if ( jsmsg.ContainsKey("uid") == false )
            {
                Debug.LogError( "req player data info error uid = " + uid );
                return true;
            }

            int readUID = (int)jsmsg["uid"].Number ;
            if ( self.vPlayerInfos.ContainsKey( readUID ) == false || self.vPlayerInfos[readUID] == null )
            {
                self.vPlayerInfos[readUID] = new PlayerInfoData();
            }
            self.vPlayerInfos[readUID].initByMsg(jsmsg) ;
            EventDispatcher.getInstance().dispatch(PlayerInfoDataCacher.EVENT_RECIEVED_PLAYER_INFO_DATA,self.vPlayerInfos[readUID] );

            if ( this.vRequestingUIDs.ContainsKey(uid) == false )
            {
                Debug.LogError( "request player info data , but do not put it in requesting UID = " + readUID );
                return false ;
            }
            this.vRequestingUIDs.Remove(uid);
            return true ;
        } ) ;
    }

    private void OnDestroy() {
        Debug.LogWarning("INFO DESTROYED");
    }
}
