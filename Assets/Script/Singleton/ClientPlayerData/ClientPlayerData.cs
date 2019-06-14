using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public interface IClientPlayerData
{
    bool onMsg( eMsgType msgID , JSONObject jsMsg );
    void onPlayerLogout();
}
public class ClientPlayerData : SingletonBehaviour<ClientPlayerData>
{
    Dictionary<string,IClientPlayerData> vPlayerDatas = new Dictionary<string,IClientPlayerData>();
    private new void Awake() {
        base.Awake();
        this.vPlayerDatas[typeof(ClubDataMgr).Name] = new ClubDataMgr();
        this.vPlayerDatas[typeof(PlayerBaseData).Name] = new PlayerBaseData();
        this.vPlayerDatas[typeof(PlayerRecorder).Name] = new PlayerRecorder();
        EventDispatcher.getInstance().registerEventHandle(Network.EVENT_MSG,this.onMsgEvent);
        EventDispatcher.getInstance().registerEventHandle(GPSManager.EVENT_GPS_RESULT,this.onGPSResultEvent );
    }

    bool onMsgEvent( EventArg e )
    {
        var emsg = (EventMsg)e;
        return onMsg(emsg.msgID,emsg.msgData) ;
    }

    bool onGPSResultEvent( EventArg e )
    {
        var js = ((EventWithObject<JSONObject>)e).argObject ;
        var code = (int)js["code"].Number;
        if ( code != 0 )
        {
            Prompt.promptText("获取位置信息失败code = " + code );
            return true ;
        }

        var baseData = getComponentData<PlayerBaseData>();
        baseData.GPS_J = js["longitude"].Number ;
        baseData.GPS_W = js["latitude"].Number ;
        // tell svr ;
        var jsMsg = new JSONObject();
        jsMsg["J"] = baseData.GPS_J ;
        jsMsg["W"] = baseData.GPS_W ;
        Network.getInstance().sendMsg(jsMsg,eMsgType.MSG_PLAYER_UPDATE_GPS,eMsgPort.ID_MSG_PORT_DATA,getSelfUID()) ;
        Debug.Log("get gps location = ");
        return true ;
    }

    bool onMsg( eMsgType msgID , JSONObject jsMsg )
    {
        switch(msgID)
        {
            case eMsgType.MSG_PLAYER_BASE_DATA:
            {
                var baseData = this.getComponentData<PlayerBaseData>();
                baseData.initByMsg(jsMsg);

                if ( jsMsg["clubs"] != null )
                {
                    getComponentData<ClubDataMgr>().init(jsMsg["clubs"].Array) ;
                }
                else
                {
                    getComponentData<ClubDataMgr>().init(null) ;
                }                
                this.getComponentData<PlayerRecorder>().init(baseData.uid);
                // dispatch event ;
                GPSManager.getInstance().requestGPS();
                EventDispatcher.getInstance().dispatch(PlayerBaseData.EVENT_RECIEVED_BASE_DATA);
            }
            break ;
            case eMsgType.MSG_PLAYER_REFRESH_MONEY:
            {
                this.getComponentData<PlayerBaseData>().coin = (int)jsMsg["coin"].Number ;
                this.getComponentData<PlayerBaseData>().diamond = (int)jsMsg["diamond"].Number ;
    
                EventDispatcher.getInstance().dispatch(PlayerBaseData.EVENT_REFRESH_MONEY);
            }
            break;
            default:
            {
                foreach (var item in this.vPlayerDatas )
                {
                    if ( item.Value.onMsg(msgID,jsMsg) )
                    {
                        return true ;
                    }
                }
            }
            return false ;
        }
        return false ;
    }
    public T getComponentData<T>() where T : class , IClientPlayerData
    {
        var t = typeof(T);
        if ( this.vPlayerDatas.ContainsKey( t.Name ) == false )
        {
            Debug.LogError("compoent is null : " + t.Name);
            return null ;
        }

        return this.vPlayerDatas[t.Name] as T;
    }
    public int getSelfUID()
    {
        return this.getComponentData<PlayerBaseData>().uid ;
    }
}
