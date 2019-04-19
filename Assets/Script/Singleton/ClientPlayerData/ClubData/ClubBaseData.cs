using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public class ClubBaseData : ClubDataCompoent
{
    JSONObject _dataJs = null ;

    public override void fetchData( bool isForce )
    {
        if ( isForce == false && this.isDataOutOfDate() == false )
        {
            this.doInformDataRefreshed(false);
            return ;
        }

        JSONObject js = new JSONObject() ;
        js["clubID"] = this.clubID;
        Network.getInstance().sendMsg(js,eMsgType.MSG_CLUB_REQ_INFO,eMsgPort.ID_MSG_PORT_CLUB,this.clubID);
    }

    public override bool onMsg( eMsgType msgID  , JSONObject msgData )
    {
        if ( eMsgType.MSG_CLUB_REQ_INFO == msgID )
        {
            int clubID = (int)msgData["clubID"].Number ;
            if ( clubID != this.clubID )
            {
                return false ;
            }

            this._dataJs = msgData;
            this.doInformDataRefreshed(true);
            return true ;
        }
        return false ;
    }

    // svr : {  inviteCnt : 23 ,  mgrs : [23,23,52], state : 0, curCnt : 23, capacity : 23 , maxEventID : 23 ,opts : {} }
    public JSONObject clubOpts
    {
        get
        {
            return this._dataJs["opts"].Obj ;
        }
        
        set 
        {
            this._dataJs["opts"] = value ;
        }
    }

    public string notice
    {
        get
        {
            return this._dataJs["notice"].Str ;
        }
        
        set 
        {
            this._dataJs["notice"] = value ;
        }
    }

    public string name
    {
        get
        {
            return this._dataJs["name"].Str ;
        }
        
        set 
        {
            this._dataJs["name"] = value ;
        }
    }

    public int creatorUID
    {
        get
        {
            return (int)this._dataJs["creator"].Number ;
        }
        
        set 
        {
            this._dataJs["creator"] = value ;
        }
    }
      
    public int diamond
    {
        get
        {
            return (int)this._dataJs["diamond"].Number ;
        }
    }
 
    public int state
    {
        get
        {
            return (int)this._dataJs["state"].Number ;
        }

        set 
        {
            this._dataJs["state"] = value ;
        }
    }

    public bool isStoped
    {
        get
        {
            return this.state == 1 ; ;
        }

        set
        {
            this.state = value ? 1 : 0 ;
        }
    }
 
    public int capacity
    {
        get
        {
            return (int)this._dataJs["capacity"].Number ;
        }
    }
   
    public JSONObject jsCore
    {
        get 
        {
            return this._dataJs;
        }
    }


    // get maxEventID () : number
    // {
    //     return this._dataJs["maxEventID"] ;
    // }
}
