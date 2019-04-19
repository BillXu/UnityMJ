using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using System.Text.RegularExpressions ;
public class ClubEvent
{
    public int eventID = 0 ;
    public eClubEvent logEventType = eClubEvent.eClubEvent_Max;
    private int _time = 0 ;
    public JSONObject jsDetail = null ;
    public eEventState state = eEventState.eEventState_Max ;

    private string _eventString  = "" ;
    public string timeStr
    {
        get
        {
            return Utility.getTimeString(this._time);
        }
    }

    public int time
    {
        set
        {
            this._time = value ;
        }
    }

    public string eventString 
    {
        get
        {
            if ( string.IsNullOrEmpty(this._eventString) )
            {
                this.constructEventString();
            }
            return this._eventString ;
        }
    }

    public bool constructEventString()
    {
        string strContent = "" ;
        List<JSONValue> vUIDs = new List<JSONValue>() ;
        switch ( this.logEventType )
        {
            case eClubEvent.eClubEvent_ApplyJoin:
            {
                if ( this.state == eEventState.eEventState_WaitProcesse )
                {
                    strContent = this.placeholdID(this.jsDetail["uid"]) + "( ID:"+ this.jsDetail["uid"] + ")申请加入俱乐部.";
                }
                else
                {
                    strContent = "【<color=#eb6c1f>" + this.placeholdID(this.jsDetail["respUID"]) + ( (int)this.jsDetail["isAgree"].Number == 1 ? "</color>】同意了【<color=#eb6c1f>" : "</color>】拒绝了【<color=#eb6c1f>" ) + this.placeholdID(this.jsDetail["uid"]) + "</color>】加入俱乐部"; 
                    vUIDs.Add(this.jsDetail["respUID"]);
                }
                vUIDs.Add(this.jsDetail["uid"]);
            }
            break;
            case eClubEvent.eClubEvent_Kick:
            {
                strContent = "【<color=#eb6c1f>" + this.placeholdID(this.jsDetail["uid"]) + "</color>】被【<color=#eb6c1f>" + this.placeholdID(this.jsDetail["mgrUID"]) + "</color>】请出了俱乐部"; 
                vUIDs.Add(this.jsDetail["mgrUID"]);
                vUIDs.Add(this.jsDetail["uid"]);
            }
            break ;
            case eClubEvent.eClubEvent_Leave:
            {
                strContent = "【<color=#eb6c1f>" + this.placeholdID(this.jsDetail["uid"]) + "</color>】离开了俱乐部"; 
                vUIDs.Add(this.jsDetail["uid"]);
            }
            break;
            case eClubEvent.eClubEvent_UpdatePrivlige:
            {
                eClubPrivilige prori = (eClubPrivilige)this.jsDetail["privilige"].Number ;
                string[] vstr = new string[(int)eClubPrivilige.eClubPrivilige_Max] ;
                vstr[(int)eClubPrivilige.eClubPrivilige_Creator] = "创建者" ;
                vstr[(int)eClubPrivilige.eClubPrivilige_Forbid] = "禁止入局" ;
                vstr[(int)eClubPrivilige.eClubPrivilige_Manager] = "管理员" ;
                vstr[(int)eClubPrivilige.eClubPrivilige_Normal] = "普通玩家" ;
                if ( vstr.Length <= (int)prori )
                {
                    Debug.LogError("invalid privilage club = " + prori );
                    break ;
                }
                strContent = "【<color=#eb6c1f>" + this.placeholdID(this.jsDetail["actUID"]) + "</color>】将玩家【<color=#eb6c1f>" + this.placeholdID(this.jsDetail["uid"]) + "</color>】设置为" + vstr[(int)prori] ; 
                vUIDs.Add(this.jsDetail["uid"]);
                vUIDs.Add(this.jsDetail["actUID"]);
            }
            break;
            default:
                Debug.LogError( "unknown log event type = " + this.logEventType + " do not process" );
                strContent = "未知俱乐部事件type="+this.logEventType ;
                break;
        }

        if ( this.state == eEventState.eEventState_WaitProcesse ) // not color full 
        {
            this._eventString = strContent ;
        }
        else
        {
            this._eventString = "<color=#835B35>" + strContent + "</c>" ;
        }
        
        // use cacher fill name first 
        var players = PlayerInfoDataCacher.getInstance();
        foreach (var item in vUIDs)
        {
            this.onPlayerDataInfo(players.getPlayerInfoByID((int)item.Number)) ;
        } 
        return this.isFinishFillEventString() ;
    }

    private string placeholdID( JSONValue id )
    {
        return "_" + (int)id.Number + "_" ;
    }

    bool isFinishFillEventString()
    {
        var v = @"_[1-9][0-9]{1,12}_"; 
        return Regex.IsMatch(this._eventString,v);
    }

    public bool onPlayerDataInfo( PlayerInfoData data )
    {
        if ( data == null )
        {
            return false;
        }

        var replace = "_"+data.uid+"_" ;
        var old = this._eventString ;
        this._eventString = this._eventString.Replace(replace,data.name) ;
        return this._eventString.EndsWith(old) == false ;
    }
} ;

public class ClubDataEvent : ClubDataCompoent {

    private int nClientMaxEventID = 0 ;
    public List<ClubEvent> vEventLog = new List<ClubEvent>() ;
    public List<ClubEvent> vEvents = new List<ClubEvent>() ;

    public override void init( ClubData clubData, int type )
    {
        base.init(clubData,type);
        EventDispatcher.getInstance().registerEventHandle(PlayerInfoDataCacher.EVENT_RECIEVED_PLAYER_INFO_DATA,this.onEvent) ;
    }

    public override void fetchData( bool isforce )
    {
        if ( false == isforce && false == this.isDataOutOfDate() )
        {
            this.doInformDataRefreshed( false );
            return ;
        }

        var msg = new JSONObject() ;
        msg["clubID"] = this.clubID;
        msg["clientMaxEventID"] = this.nClientMaxEventID ;
        msg["state"] = (int)eEventState.eEventState_Processed ; 
        Network.getInstance().sendMsg(msg,eMsgType.MSG_CLUB_REQ_EVENTS,eMsgPort.ID_MSG_PORT_CLUB,this.clubID ) ;

        msg["clientMaxEventID"] = 0 ;  // do not cache wait process event ;
        msg["state"] = (int)eEventState.eEventState_WaitProcesse ;  // not process event ;
        Network.getInstance().sendMsg(msg,eMsgType.MSG_CLUB_REQ_EVENTS,eMsgPort.ID_MSG_PORT_CLUB,this.clubID ) ;
    }

    public override bool onMsg( eMsgType msgID, JSONObject msgData )
    {
        if ( eMsgType.MSG_CLUB_REQ_EVENTS == msgID )
        {
            var clubID = msgData["clubID"] ;
            if ( null == clubID )
            {
                Utility.showTip("eMsgType.MSG_CLUB_REQ_EVENTS lack of clubID key , please add");
                return true ;
            }

            if ( (int)clubID.Number != this.clubID )
            {
                return  false ;
            }

            var ret = (int)msgData["ret"].Number ;
            if ( ret != 0  )
            {
                Debug.LogWarning( "invalid priviliage req event club id = " + this.clubID );
                this.doInformDataRefreshed(true);
                return true ;
            }

            if ( msgData.ContainsKey( "vEvents" ) == false )
            {
                this.doInformDataRefreshed(true);
            }

            var vtmpEvents = msgData["vEvents"].Array;
            var pageIdx = msgData["pageIdx"].Number ;
            foreach( var item in vtmpEvents )
            {
                var eve = item.Obj;
                 int eveID = (int)eve["eventID"].Number ;
                 var type = (eClubEvent)eve["type"].Number ;
                 var state = (eEventState)eve["state"].Number ;
                 if ( eveID > this.nClientMaxEventID && state != eEventState.eEventState_WaitProcesse ) // only processed event , do incremental request 
                 {
                     this.nClientMaxEventID = eveID ;
                 }

                 var p = new ClubEvent();
                 p.logEventType = type ;
                 p.time = (int)eve["time"].Number ;
                 p.jsDetail = eve["detail"].Obj ;
                 p.eventID = eveID ;
                 p.state = state ;
                 p.constructEventString();
                 if ( state != eEventState.eEventState_WaitProcesse )
                 {
                     this.vEventLog.Add(p);
                     continue;
                 }
                 else
                 {
                     if ( 0 == pageIdx ) // we do not cache wait process event ;
                     {
                         this.vEvents.Clear();
                         pageIdx = -1 ; // to invoid next clearing ;
                     }
                     this.vEvents.Add(p);
                     continue ;
                 }   
            }
            if ( vtmpEvents.Length < 10 && vtmpEvents.Length > 0 )
            {
                 // check we do not want cache too many logs ;
                if ( this.vEventLog.Count > 50 )
                {
                    this.vEventLog.RemoveRange(0, this.vEventLog.Count - 50 ) ;
                }

                this.doInformDataRefreshed(true);
            }
             return true ;
        }
        return false ;
    }

    public bool onEvent( EventArg eve )
    {
        if ( PlayerInfoDataCacher.EVENT_RECIEVED_PLAYER_INFO_DATA == eve.type )
        {
            var t = (EventWithObject<PlayerInfoData>)eve;
            var playerData = t.argObject ;
            var hasNewFillEvent = false ;
            foreach (var item in this.vEvents )
            {
                if ( item.onPlayerDataInfo( playerData) )
                {
                    hasNewFillEvent = true ;
                }
            }  

            if ( hasNewFillEvent )
            {
                this.doInformDataRefreshed(false);
            }
            return false;
        }
        return false ;
    }

    public void doProcessedEvent( int eventID )
    {
        
    }

    public override void onDestroy()
    {
         EventDispatcher.getInstance().removeEventHandleByTarget(this);
    }

}

