using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public class RoomBaseData 
{
    JSONObject jsMsgInfo = null ;
    public DawoerRoomOpts opts = null ;
    public int diceValue{ get ;set; } = 1 ;
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
    public int otherCanActCard { get ; set ;}

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
    }

    public void onGameWillStart( JSONObject jsMsg )
    {
        this.bankerIdx = (int)jsMsg["bankerIdx"].Number ;
        this.leftRound = (int)jsMsg["leftCircle"].Number ;
        this.curActIdx = this.bankerIdx ;
        this.leftMJCnt = this.initCardCnt ;
    }
    public void onEndGame()
    {
        this.leftMJCnt = this.initCardCnt ;
    }
}
