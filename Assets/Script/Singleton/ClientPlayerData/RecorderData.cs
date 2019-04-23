using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Boomlagoon.JSON; 
using System ;
public class PlayerOffsetItem   {
    public int uid  = 0 ;
    public int offset = 0 ;
}

public class IRecorderEntry
{
    public List<PlayerOffsetItem> vOffsets = new List<PlayerOffsetItem>();
    public string strTime = "";
}

public class RecorderSinglRoundEntry : IRecorderEntry
{
    public int replayID = 0 ;
    public void parseData( JSONObject jsData )
    {
        this.strTime = jsData["time"].Str ;
        this.replayID = (int)jsData["replayID"].Number ;
        if ( jsData.ContainsKey("resultDetail") == false )
        {
            return ;
        }

        var vOffset = jsData["resultDetail"].Array ;
        for ( int idx = 0 ; idx < vOffset.Length ; ++idx )
        {
            var of = new PlayerOffsetItem() ;
            this.vOffsets.Add(of);
            JSONObject offset = vOffset[idx].Obj ;
            of.uid = (int)offset["uid"].Number ;
            of.offset = (int)offset["offset"].Number ;
        }
    }
}

public class RecorderRoomEntry : IRecorderEntry
{
    public delegate void singleResultCallBack( RecorderRoomEntry data );
    public int roomID = 0 ;
    public JSONObject opts = null ;
    public int sieralNum = 0 ; 
    public List<RecorderSinglRoundEntry> vSingleRoundRecorders = new List<RecorderSinglRoundEntry>();

    singleResultCallBack lpCallBack = null;
    public void parseData( JSONObject jsData )
    {
        this.roomID = (int)jsData["roomID"].Number ;
        this.opts = jsData["opts"].Obj;
        this.sieralNum = (int)jsData["sieralNum"].Number;
        var t = jsData["time"].Number ;
        this.strTime = Utility.getTimeString((int)t);
        
        var vOffset = jsData["offsets"].Array ;
        foreach ( var item in vOffset )
        {
            var offset = item.Obj ;
            var of = new PlayerOffsetItem();
            of.uid = (int)offset["userUID"].Number ;
            of.offset = (int)offset["offset"].Number ;
            this.vOffsets.Add(of);
        }
    }

    // string getLocalTimeString( double timeStamp )
    // {
    //     DateTime dtStart = new DateTime(1970, 1, 1);
    //     dtStart = dtStart.ToLocalTime();
    //     long lTime = ((long)timeStamp * 10000000);
    //     TimeSpan toNow = new TimeSpan(lTime);
    //     DateTime targetDt = dtStart.Add(toNow);
    //     return targetDt.ToString();
    // }

    public void fetchSingleRoundRecorders( singleResultCallBack pResultCallBack )
    {
        if ( this.vSingleRoundRecorders.Count  > 0 )
        {
            pResultCallBack(this);
            return ;
        }

        if ( null != this.lpCallBack )
        {
            Debug.LogWarning("already requesting , please wait");
            return ;
        }
        this.lpCallBack = pResultCallBack;
        ClientPlayerData.getInstance().StartCoroutine("doReqSingleRoundRecorder");
    }

    IEnumerator doReqSingleRoundRecorder()
    {
        var url = "http://api.youhoox.com/cfmj.new.serial.php?serial="+ this.sieralNum ;
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
 
        if( www.isNetworkError || www.isHttpError ) {
            Debug.Log(www.error);
            if ( null != this.lpCallBack )
            {
                this.lpCallBack(null);
            }
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            string s = "{ \"a\" : " + www.downloadHandler.text + "}" ;
            var js = JSONObject.Parse(s);
            if ( null == js )
            {
                Debug.LogError("pase singe recorder error");
                if ( null != this.lpCallBack )
                {
                    this.lpCallBack(null);
                }
                else
                {
                    var array = js["a"].Array;
                    foreach ( var v in array )
                    {
                        var pItem = new RecorderSinglRoundEntry();
                        pItem.parseData(v.Obj);
                        this.vSingleRoundRecorders.Add(pItem);
                    }
                }
            }
        }
        this.lpCallBack = null ;  // must clear callBack ;
    }
}

public class RecorderData
{
    public delegate void DataCallBack( RecorderData p, bool isCacher ) ;
    List<RecorderRoomEntry> vRecorder = new List<RecorderRoomEntry>();

    private int nOwnerID = 0 ;
    private bool isOwnerClub = false ;

    private int nCacherMaxSerialNum = 0 ;
    private float nLastFetchDataTime = 0 ;
    DataCallBack lpCallBack = null ;
    public void init( int ownerID, bool isClub = false )
    {
        this.nOwnerID = ownerID ;
        this.isOwnerClub = isClub ;
        this.nCacherMaxSerialNum = 0 ;
        this.nLastFetchDataTime = 0 ;
    }

    public void fetchData( DataCallBack pResultCallBack )
    {
        var now = Time.realtimeSinceStartup ;
        if ( now - this.nLastFetchDataTime < 30 ) // at least, half minite  will refresh data 
        {
            pResultCallBack(this,true) ;
            return ;
        }

        if ( null != this.lpCallBack )
        {
            Debug.LogWarning("already request room recorder , wait a while");
            return ;
        }
        this.lpCallBack = pResultCallBack ;
        ClientPlayerData.getInstance().StartCoroutine("doReqRecorder");
    }

    IEnumerator doReqRecorder()
    {
        var url = "" ;
        if ( this.isOwnerClub )
        {
            url = "http://api.youhoox.com/cfmj.new.club.php?club_id="+this.nOwnerID ;
        }
        else
        {
            url = "http://api.youhoox.com/cfmj.new.uid.php?uid=" + this.nOwnerID + "&self=false";
        }
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
 
        if( www.isNetworkError || www.isHttpError ) {
            Debug.Log(www.error);
            if ( null != this.lpCallBack )
            {
                this.lpCallBack(null,false);
            }
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            string s = "{ \"a\" : " + www.downloadHandler.text + "}" ;
            var js = JSONObject.Parse(s);
            if ( null == js )
            {
                Debug.LogError("pase singe recorder error");
                if ( null != this.lpCallBack )
                {
                    this.lpCallBack(null,false);
                }
                else
                {
                    var array = js["a"].Array;
                    this.parseRecorder(array);
                    this.lpCallBack(this,false);
                    this.nLastFetchDataTime = Time.realtimeSinceStartup;
                }
            }
        }
        this.lpCallBack = null ;  // must clear callBack ;
    }

    public void onPlayerClearLogout()
    {
        this.vRecorder.Clear() ;
        this.nCacherMaxSerialNum = 0 ;
        this.nLastFetchDataTime = 0 ;
        this.nOwnerID = 0 ;
        this.isOwnerClub = false ;
    }

    protected void parseRecorder( JSONArray js )
    {
        var maxHoldRecordCnt = GameConfig.getInstance().MAX_RECORDER_CACHER ;
        var resultCnt = js.Length + this.vRecorder.Count ;
        if ( resultCnt > maxHoldRecordCnt )
        {
            var eraseCnt = ( resultCnt - maxHoldRecordCnt ) ;
            eraseCnt = Math.Min(this.vRecorder.Count , eraseCnt);
            while ( eraseCnt-- <= 0 )
            {
                this.vRecorder.Pop();
            }
        }

        foreach ( var e in js )
        {
            var v = e.Obj;
            var sieal = (int)v["sieralNum"].Number;
            if ( sieal <= this.nCacherMaxSerialNum )
            {
                continue ;
            }

            if ( sieal > this.nCacherMaxSerialNum  )
            {
                this.nCacherMaxSerialNum = sieal ;
            }

            var p = new RecorderRoomEntry();
            p.parseData(v);
            this.vRecorder.Add(p);
        }

        this.vRecorder.Sort((RecorderRoomEntry a , RecorderRoomEntry b )=>{
            if ( a.sieralNum < b.sieralNum )
            {
                return 1 ;
            }
            else if ( a.sieralNum > b.sieralNum )
            {
                return -1 ;
            }
            return 0 ;
        });
    }
}

public class PlayerRecorder : IClientPlayerData
{
    public RecorderData selfClientData  = null ;
    public void init( int nselfUID )
    {
        if ( null == this.selfClientData )
        {
            this.selfClientData = new RecorderData();
        }
        this.selfClientData.init(nselfUID,false) ;
    }
    public bool onMsg( eMsgType msgID , JSONObject jsMsg )
    {
        return false ;
    }

    public void fetchData( RecorderData.DataCallBack pResultCallBack )
    {
        if ( this.selfClientData == null )
        {
            Debug.LogError("client recorder data is null ? forget init ? ");
            return  ;
        }
        this.selfClientData.fetchData(pResultCallBack);
    }
    public void onPlayerLogout()
    {
        if ( null != this.selfClientData )
        {
            this.selfClientData.onPlayerClearLogout();
        }
    }
}