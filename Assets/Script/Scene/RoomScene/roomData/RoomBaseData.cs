using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public class RoomBaseData 
{
    JSONObject jsMsgInfo = null ;
    public DawoerRoomOpts opts = null ;
    public int diceValue 
    {
        get
        { 
            return (int)this.jsMsgInfo["dice"].Number ; 
        } 

        set
        {
            this.jsMsgInfo["dice"] = value;
        }
    }
    public int bankerIdx
    {
        get
        { 
            return (int)this.jsMsgInfo["bankIdx"].Number ; 
        } 

        set
        {
            this.jsMsgInfo["bankIdx"] = value;
        }
    }
    public int curActIdx
    {
        get
        { 
            return (int)this.jsMsgInfo["curActIdex"].Number ; 
        } 

        set
        {
            this.jsMsgInfo["curActIdex"] = value;
        }
    }
    public int initCardCnt
    {
        get
        {
            return (int)this.jsMsgInfo["initCards"].Number;
        }
    }
    public bool isRoomOpen
    {
        get
        { 
            return (int)this.jsMsgInfo["isOpen"].Number == 1 ; 
        } 

        set
        {
            this.jsMsgInfo["isOpen"] = value ? 1 : 0 ;
        }
    }
    public int curRound{ get { return this.totalRoundCnt - this.leftRound ;}}
    public int totalRoundCnt { get {return this.opts.round ;}}  
    public int lastChuIdx{ get ; set ;}
    public int otherCanActCard { get ; set ;}  // do not forget parse last chu card is here ;

    public int leftRound
    {
        get 
        {
            return (int)this.jsMsgInfo["leftCircle"].Number;
        }
        set
        {
            this.jsMsgInfo["leftCircle"] = value ;
        }
    }
    public int leftMJCnt
    { 
        get
        {
            return (int)this.jsMsgInfo["leftCards"].Number;
        } 

        set 
        {
            this.jsMsgInfo["leftCards"] = value ;
        }
    }

    public int seatCnt { get{ return this.opts.seatCnt ;}}

    public int roomID
    {
        get 
        {
            return (int)jsMsgInfo["roomID"].Number;
        }
    }
    
    public int wallCard8
    {
        get { return (int)jsMsgInfo["r8"].Number;}
        set { jsMsgInfo["r8"] = value ;} 
    } 

    public int wallCard16
    {
        get { return (int)jsMsgInfo["r16"].Number;}
        set { jsMsgInfo["r16"] = value ;} 
    } 
    public int applyDismissIdx { get ; set ;} = -1 ;
    public List<int> agreeDismissIdx { get ; set ;} = new List<int>() ;
    public int dimissRoomLeftTime { get ; set ;}
    public string rule{ get{ return this.opts.ruleDesc() ; }} 
    public int baseScore { get ; set ; } = 1 ;
    public bool isRoomOverd = false ;
    public eRoomState state 
    { 
        get 
        {
            return (eRoomState)jsMsgInfo["state"].Number; 
        }

        set 
        {
            jsMsgInfo["state"] = (int)value;
        } 
    }
    public void parseInfo( JSONObject jsInfo )
    {
        this.jsMsgInfo = jsInfo ;
        if ( null == this.opts )
        {
            this.opts = new DawoerRoomOpts( jsInfo["opts"].Obj );
        }
        else
        {
            this.opts.parseOpts( jsInfo["opts"].Obj );
        }

        if ( jsInfo.ContainsKey( "lastActInfo" ) && jsInfo["lastActInfo"] != null )
        {
            var obj = jsInfo["lastActInfo"].Obj;
            this.curActIdx = (int)obj["idx"].Number;
            if ( ((eMJActType)obj["type"].Number) == eMJActType.eMJAct_Chu )
            {
                this.otherCanActCard = (int)obj["card"].Number;
                this.lastChuIdx = this.curActIdx;
            }
        }

        if ( (int)jsInfo["isWaitingDismiss"].Number == 1 )
        {
            var AgreeIdx = jsInfo["agreeIdxs"].Array;
            this.agreeDismissIdx.Clear();
            foreach (var item in AgreeIdx )
            {
                this.agreeDismissIdx.Add((int)item.Number);
            }

            this.dimissRoomLeftTime = (int)jsInfo["leftWaitTime"].Number;
            this.applyDismissIdx = agreeDismissIdx[0];
        }
        else
        {
            this.applyDismissIdx = -1 ;
            this.agreeDismissIdx.Clear();
        }

        if ( this.state != eRoomState.eRoomSate_WaitReady )
        {
            this.isRoomOpen = true ;
        }
    }

    public void onGameWillStart( JSONObject jsMsg )
    {
        this.bankerIdx = (int)jsMsg["bankerIdx"].Number ;
        --this.leftRound;// = (int)jsMsg["leftCircle"].Number ;
        this.diceValue = (int)jsMsg["dice"].Number;
        this.wallCard16 = (int)jsMsg["r16"].Number;
        this.wallCard8 = (int)jsMsg["r8"].Number;
        this.curActIdx = this.bankerIdx ;
        this.leftMJCnt = this.initCardCnt ;
        this.isRoomOpen = true ;
        this.state = eRoomState.eRoomState_StartGame ;
    }
    public void onEndGame()
    {
        this.leftMJCnt = this.initCardCnt ;
        this.state = eRoomState.eRoomSate_WaitReady ;
    }
    public bool isWaitReadyState()
    {
        return this.state == eRoomState.eRoomSate_WaitReady ;
    }

    public bool isDuringGame()
    {
        return this.state != eRoomState.eRoomSate_WaitReady && this.state != eRoomState.eRoomState_GameEnd;
    }
    public bool isCanLeaveRoom()
    {
        return this.isRoomOpen == false ;
    }
    public bool isCanDismissRoom()
    {
        return this.isRoomOpen ;
    }
}
