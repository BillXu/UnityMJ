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
    }

    public DawoerRoomOpts( JSONObject jsOpts )
    {
        mOpts = jsOpts ;
    }

    public int round 
    { 
        get
        {
            if ( this.mOpts["round"] != null )
            {
                this.mOpts["round"] = 8 ;
                Debug.LogError("round key is null");
            }
            return (int)this.mOpts["round"].Number ; 
        } 

        set
        {
            this.mOpts["round"] = value ;
        }
        
    }
    public int playerCnt 
    { 
        get
        {
            if ( this.mOpts["seatCnt"] != null )
            {
                this.mOpts["seatCnt"] = 8 ;
                Debug.LogError("seatCnt key is null");
            }
            return (int)this.mOpts["seatCnt"].Number ; 
        } 

        set
        {
            this.mOpts["seatCnt"] = value ;
        }
    }
    public int payType
    { 
        get
        {
            if ( this.mOpts["payType"] != null )
            {
                this.mOpts["payType"] = 8 ;
                Debug.LogError("payType key is null");
            }
            return (int)this.mOpts["payType"].Number ; 
        } 

        set
        {
            this.mOpts["payType"] = value ;
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
            return (int)mOpts["pao"].Number ;
        }
        set
        {
            mOpts["pao"] = value ;
        }
    }

    public int limitFen
    {
        get 
        {
            return (int)mOpts["limitFen"].Number ;
        }
        set
        {
            mOpts["limitFen"] = value ;
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
