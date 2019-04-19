using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public class PlayerBaseData : PlayerInfoData , IClientPlayerData
{
    public static string EVENT_RECIEVED_BASE_DATA  = "EVENT_RECIEVED_BASE_DATA" ;
    public static string EVENT_REFRESH_MONEY = "EVENT_REFRESH_MONEY" ;
    public int diamond 
    {
        get 
        {
            return (int)this.playerBrifeDataMsg["diamond"].Number; 
        }

        set 
        {
            this.playerBrifeDataMsg["diamond"] = value; 
        }
    }

    public int coin 
    {
        get 
        {
            return (int)this.playerBrifeDataMsg["coin"].Number; 
        }

        set 
        {
            this.playerBrifeDataMsg["coin"] = value; 
        }
    }

    public int stayInRoomID 
    {
        get 
        {
            if ( this.playerBrifeDataMsg.ContainsKey("stayRoomID") == false )
            {
                return 0 ;
            }
            return (int)this.playerBrifeDataMsg["stayRoomID"].Number; 
        }

        set 
        {
            this.playerBrifeDataMsg["stayRoomID"] = value; 
        }
    }

    public bool onMsg( eMsgType msgID , JSONObject jsMsg )
    {
        return false ;
    }   
    public void onPlayerLogout()
    {

    }
}
