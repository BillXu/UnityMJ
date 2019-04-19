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
        this.vPlayerDatas[typeof(RecorderData).Name] = new PlayerRecorder();
        EventDispatcher.getInstance().registerEventHandle(Network.EVENT_MSG,this.onMsgEvent);
    }

    bool onMsgEvent( EventArg e )
    {
        var emsg = (EventMsg)e;
        return onMsg(emsg.msgID,emsg.msgData) ;
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
            return null ;
        }

        return this.vPlayerDatas[t.Name] as T;
    }
    public int getSelfUID()
    {
        return this.getComponentData<PlayerBaseData>().uid ;
    }
}
