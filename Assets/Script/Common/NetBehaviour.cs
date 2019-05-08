using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public class NetBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake() {
        this.registerEvent(Network.EVENT_MSG); 
        this.registerEvent(Network.EVENT_FAILED);
        this.registerEvent(Network.EVENT_RECONNECTED_FAILED);   
        this.registerEvent(Network.EVENT_OPEN);      
        this.registerEvent(Network.EVENT_DISCONNECTED);   
        this.registerEvent(Network.EVENT_RECONNECT);      
    }

    protected void registerEvent( string eventName )
    {
        EventDispatcher.getInstance().registerEventHandle(eventName,this.onEvent) ; 
    }
    private void OnDestroy() {
        EventDispatcher.getInstance().removeEventHandleByTarget(this);
    }
 

    protected virtual bool onEvent( EventArg arg )
    {
        if ( Network.EVENT_DISCONNECTED == arg.type )
        {
            this.onDisconnect();
        }
        else if ( Network.EVENT_MSG == arg.type )
        {
            var msg = (EventMsg)arg;
            return this.onMsg(msg.msgID,msg.msgData);
        }
        else if ( Network.EVENT_RECONNECT == arg.type )
        {
            this.onReconnect(true);
        }
        else if ( Network.EVENT_RECONNECTED_FAILED == arg.type )
        {
            this.onReconnect(false);
        }
        else if ( Network.EVENT_FAILED == arg.type )
        {
            this.onConnectResult(false) ;
        }
        else if ( Network.EVENT_OPEN == arg.type )
        {
            this.onConnectResult(true);
        }
        return false ;
    }

    public bool sendMsg( JSONObject jsMsg , eMsgType msgID, eMsgPort targetPort , int targetID , Network.IOneMsgCallback callBack = null )
    {
        return Network.getInstance().sendMsg(jsMsg,msgID,targetPort,targetID,callBack) ;
    }

    
    // on disconnect from svr
    protected virtual void onDisconnect()
    {

    }

    protected virtual bool onMsg( eMsgType msgID , JSONObject jsMsg )
    {
        return false ;
    }

    // logic reconnect result ;
    protected virtual void onReconnect( bool isSuccess )
    {

    }

    protected virtual void onConnectResult( bool isSucess )
    {

    }
}
