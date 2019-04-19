using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
public class ClubData : MonoBehaviour
{
    private int _ClubID = 0 ;
    private ClubDataMgr _PlayerClubs = null ;
    private Dictionary<eClubDataComponent,ClubDataCompoent> vClubDataComponents  = new Dictionary<eClubDataComponent,ClubDataCompoent>() ;

    public void init( int clubID, ClubDataMgr clubsMgr )
    {
        this._PlayerClubs = clubsMgr ;
        this._ClubID = clubID ;
        this.vClubDataComponents[eClubDataComponent.eClub_BaseData] = new ClubBaseData();
        this.vClubDataComponents[eClubDataComponent.eClub_Events] = new ClubDataEvent();
        this.vClubDataComponents[eClubDataComponent.eClub_Members] = new ClubDataMembers();
        this.vClubDataComponents[eClubDataComponent.eClub_Recorders] = new ClubRecorderData();
        this.vClubDataComponents[eClubDataComponent.eClub_Rooms] = new ClubDataRooms();
        foreach (var item in this.vClubDataComponents )
        {
            item.Value.init(this,(int)item.Key) ;
        } 
        this.vClubDataComponents[eClubDataComponent.eClub_BaseData].fetchData(false);
    }

    // onDataRefreshed( data : IClubDataComponent )
    // {
    //     this._PlayerClubs.onClubDataRefreshed(this,data);
    // }

    // fetchData( type : eClubDataComponent , isForce : boolean )
    // {
    //     let p = this.vClubDataComponents[type] ;
    //     if ( p == null )
    //     {
    //         console.error( "fetch data component is null , type = " + type );
    //         return ;
    //     }
    //     p.fetchData( isForce ) ;
    // }
    public bool onMsg( eMsgType msgID, JSONObject msgData )
    {
        foreach (var item in this.vClubDataComponents )
        {
            if ( item.Value.onMsg(msgID,msgData) )
            {
                return true ;
            }
        }
        return false ;
    }
    public ClubDataEvent getClubEvents()
    {
        return (ClubDataEvent)this.vClubDataComponents[eClubDataComponent.eClub_Events] ;
    }

    public ClubDataMembers getClubMembers()
    {
        return (ClubDataMembers)this.vClubDataComponents[eClubDataComponent.eClub_Members] ;
    }

    public void onDestry()
    {
        foreach (var item in this.vClubDataComponents )
        {
            item.Value.onDestroy();
        } 
        this.vClubDataComponents.Clear() ;
    }

    public int getClubID()
    {
        return 0 ;
    }

    public ClubBaseData getClubBaseData()
    {
        return (ClubBaseData)this.vClubDataComponents[eClubDataComponent.eClub_BaseData] ; 
    }
}
