using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public interface ClubDataDelegate
{
    void onRefresh( ClubDataCompoent data );
    void onInformUpdate( ClubDataCompoent data,string evnt , JSONObject jsArg = null );
    void onResopneResult( ClubDataCompoent data, int ret , string strDesc , JSONObject jsArg = null );
}
public abstract class ClubDataCompoent
{
    private  float nLastFetchDataTime = 0 ;
    private ClubData _ClubData = null ;
    private int _type = 0 ;

    public int clubID
    {
        get
        {
            return this.getClub().getClubID(); 
        }
    }
    protected ClubDataDelegate mDelegate
    {
        get ; set ;
    }
    public ClubData getClub() { return this._ClubData ;}

    public virtual void init( ClubData clubData, int type )
    {
        this._ClubData = clubData ;
        this.nLastFetchDataTime = 0 ;
        this._type = type ;
    }

    public int getType()
    {
        return this._type;
    }

    abstract public void fetchData( bool isforce );
    
    public void doInformDataRefreshed( bool isResetInterval )
    {
        if ( this.mDelegate != null )
        {
            this.mDelegate.onRefresh(this);
        }

        if ( isResetInterval )
        {
            this.nLastFetchDataTime = Time.realtimeSinceStartup;
        }
    }

    protected bool isDataOutOfDate()  // by seconds 
    {
        return (Time.realtimeSinceStartup - this.nLastFetchDataTime ) > 15 ; // 15 seconds , refresh rate
    }

    public virtual bool onMsg( eMsgType msgID , JSONObject msgData ) 
    {
        return false ;
    }

    public virtual void onDestroy()
    {
         
    }
}
