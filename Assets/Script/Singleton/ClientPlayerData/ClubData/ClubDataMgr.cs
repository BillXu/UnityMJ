using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public interface ClubDataMgrDelegate
{
    void onNewClub( ClubData club ) ;
    void onLeaveClub( ClubData club ) ;
}

public class ClubDataMgr : IClientPlayerData {

    List<ClubData> vClubs = new List<ClubData>() ;
    public ClubDataMgrDelegate pDelegate = null ;

    public void init( JSONArray vjsClubIDs ) 
    {
        // construct club datas ;
        if ( null != vjsClubIDs )
        {
            foreach ( var v in vjsClubIDs )
            {
                var pClub = new ClubData() ;
                pClub.init((int)v.Number,this) ;
                this.vClubs.Add( pClub ) ;
            }
        }
    }
 
    public void setDelegate( ClubDataMgrDelegate pd )
    {
        this.pDelegate = pd ;
    }

    public bool onMsg( eMsgType msgID , JSONObject msg )
    {
        foreach ( var v in this.vClubs )
        {
            if ( v.onMsg(msgID,msg) )
            {
                return true ;
            }
        }
        return false ;
    }

    public void onPlayerLogout() 
    {
        foreach ( var v in this.vClubs )
        {
            v.onDestry();
        }
        this.vClubs.Clear();
        this.setDelegate(null);
    }

    // onClubDataRefreshed( club : ClubData, refreshedCompoent : IClubDataComponent )
    // {
    //     if ( this.pDelegate )
    //     {
    //         this.pDelegate.onClubDataRefresh(club,refreshedCompoent);
    //     }
    // }

    public ClubData getClubByID( int clubID )
    {
        foreach ( var v in this.vClubs )
        {
            if ( v.getClubID() == clubID )
            {
                return v ;
            }
        }
        return null ;
    }

    public void deleteClub( int clubID )
    {
        for ( int idx = 0 ; idx < this.vClubs.Count ; ++idx )
        {
            if ( this.vClubs[idx].getClubID() == clubID )
            {
                if ( this.pDelegate != null )
                {
                    this.pDelegate.onLeaveClub(this.vClubs[idx]) ;
                }
                this.vClubs[idx].onDestry();
                this.vClubs.RemoveAt(idx) ;
                return ;
            }
        }

        Debug.LogError( "client data do not have clubID = " + clubID );
    }

    public void addClub( int clubID )
    {
        var p = getClubByID(clubID);
        if ( null != p )
        {
            Debug.LogError("why duplicate add club id = " + clubID );
            return ;
        }

        var c = new ClubData() ;
        c.init(clubID,this) ;
        this.vClubs.Add(c);
        if ( null != this.pDelegate )
        {
            this.pDelegate.onNewClub(c) ;
        }
    }
}

