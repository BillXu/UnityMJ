using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public class DawoerRoomOpts : IRoomOpts
{
    JSONObject mOpts = null ;
    // Start is called before the first frame update 
    public DawoerRoomOpts()
    {
        mOpts = new JSONObject();
        this.mOpts["circle"] = 0 ;
        this.gameType = eGameType.eGame_DaWoEr;
    }

    public DawoerRoomOpts( JSONObject jsOpts )
    {
        mOpts = jsOpts ;
    }

    public eGameType gameType
    {
        get
        {
            return (eGameType)mOpts["gameType"].Number;
        }
        set
        {
            mOpts["gameType"] = (int)value ;
        }
    }
    public int round 
    { 
        get
        {
            if ( this.mOpts["level"] == null )
            {
                this.mOpts["level"] = 0 ;
                Debug.LogError("round key is null");
            }
            return (int)this.mOpts["level"].Number ; 
        } 

        set
        {
            this.mOpts["level"] = value == 8 ? 0 : 1 ;
        }
        
    }
    public int seatCnt 
    { 
        get
        {
            if ( this.mOpts["seatCnt"] != null )
            {
                this.mOpts["seatCnt"] = 4 ;
                Debug.LogError("seatCnt key is null");
            }
            return (int)this.mOpts["seatCnt"].Number ; 
        } 

        set
        {
            //this.mOpts["seatCnt"] = value ;
            Debug.LogWarning("temp test set seat = 2");
            this.mOpts["seatCnt"] = 2 ;
        }
    }
    public ePayRoomCardType payType
    { 
        get
        {
            if ( this.mOpts["payType"] != null )
            {
                this.mOpts["payType"] = 8 ;
                Debug.LogError("payType key is null");
            }
            return (ePayRoomCardType)this.mOpts["payType"].Number ; 
        } 

        set
        {
            this.mOpts["payType"] = (int)value ;
        }
    }
    public int fee 
    { 
        get 
        {
            if ( this.round == 8 )
            {
                return 4 ;
            } 

            if ( 16 == this.round )
            {
                return 8 ;
            }
            Debug.LogError("unknown round type = " + this.round );
            return 1 ;
        }

        set
        {

        } 
    }
    public int paoType
    {
        get 
        {
            return (int)mOpts["dpOnePay"].Number ;
        }
        set
        {
            mOpts["dpOnePay"] = value == 1 ? 1 : 0;
        }
    }
    public int limitFen
    {
        get 
        {
            return (int)mOpts["guang"].Number ;
        }
        set
        {
            mOpts["guang"] = value ;
        }
    }
    public bool isRandSeat
    {
        get
        {
            return mOpts["isRandSeat"].Number == 1 ;
        }
        set
        {
            mOpts["isRandSeat"] = value ? 1 : 0 ;
        }
    }
    public bool isEnableIPAndGps
    {
        get
        {
            return mOpts["isEnableIPAndGps"].Number == 1 ;
        }
        set
        {
            mOpts["isEnableIPAndGps"] = value ? 1 : 0 ;
        }
    }
    public bool isForceGPS
    {
         get
        {
            return mOpts["isForceGPS"].Number == 1 ;
        }
        set
        {
            mOpts["isForceGPS"] = value ? 1 : 0 ;
        }       
    }
    public void parseOpts( JSONObject jsOpts )
    {
        this.mOpts = jsOpts ;
    }
    public JSONObject toJsOpts()
    {
        return mOpts ;
    }
}
