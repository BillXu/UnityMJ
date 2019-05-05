using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;
using UnityEngine ;
public class EventMsg : EventArg
{
    public eMsgType msgID ;
    public JSONObject msgData ; 
}

public enum eNetworkState
{
    eNet_Uninit,
    eNet_Connecting,
    eNet_Connected,
    eNet_Disconnect,
    eNet_Max,
}
public class Network : SingletonBehaviour<Network>,WebSocketUnityDelegate
{
    public delegate bool IOneMsgCallback( JSONObject jsMsg ) ;
    public static string EVENT_OPEN = "open"; // 首次建立网络链接成功，如果非首次，系统会直接逻辑重连，而不会发出次事件。
    public static string EVENT_FAILED = "failed"; // 建立网络链接失败
    public static string EVENT_MSG = "msg";  // 收到网络消息
    public static string EVENT_DISCONNECTED = "close"; // 网络断开
    public static string EVENT_RECONNECT = "reconnect"; // 逻辑重连成功
    public static string EVENT_RECONNECTED_FAILED = "reconnectFailed" ;  // 逻辑重连失败，网络链接是成功的。逻辑需要重新登录。

    static int TIME_HEAT_BEAT = 3 ; 
    static string MSG_ID = "msgID" ;

    protected WebSocketUnity mWebSocket = null;
    protected  List<KeyValuePair<eMsgType,IOneMsgCallback>> vMsgCallBack = new List<KeyValuePair<eMsgType,IOneMsgCallback>>();  
    protected bool isRecievedHeatBet = false ;

    protected JSONObject mjsMsgProxy = new JSONObject();
    protected EventMsg mEventMsg = new EventMsg();
    // when client side close connect , or reconnect error , onClose always be invoked , so we don't want this result ,
    // we only error one time is ok ;  
    protected eNetworkState mNetState = eNetworkState.eNet_Uninit;
    protected bool isStartHeadBet = false ;
    protected bool isStartReconnect = false ;
    public int SessionID { get ; set ;} = 0 ;

    private string defaultIP = "" ;
    private string backUpIP = null ;
    protected bool isUseBackUpIP = false ;
    protected string mDstIP
    { 
        get
        {
            if ( this.isUseBackUpIP && this.backUpIP != null && this.backUpIP.Length > 0 )
            {
                return this.backUpIP ;
            }
            return defaultIP ;
        } 
    }
    private bool isSetedUp
    {
        get 
        {
            return string.IsNullOrEmpty(this.defaultIP) == false ;
        }
    }

    // can ony invoke in init method , only invoke one time , connect other ip ,please use function : tryNewDstIP()
    public void setUpAndConnect( string dstIPPort,string backUpIPPort = null ) 
    {
       this.defaultIP = "ws://" + dstIPPort ;
       if ( backUpIPPort != null )
       {
            this.backUpIP = "ws://" + backUpIPPort ;
       }
       
       Debug.Log( "direct connect to svr" );
       doConnect(); 
    }

    public bool sendMsg( JSONObject jsMsg , eMsgType msgID, eMsgPort targetPort , int targetID , IOneMsgCallback callBack = null )
    {
        if ( eNetworkState.eNet_Connected != this.mNetState )
        {
            Debug.LogError( "socket is not open , can not send msgid = " + msgID + " state = " + this.mNetState );
            return false;
        }
        jsMsg[Network.MSG_ID] = (int)msgID ;

        mjsMsgProxy["cSysIdentifer"] = (int)targetPort ;
        mjsMsgProxy["nTargetID"] = targetID ;
        mjsMsgProxy["JS"] = jsMsg.ToString().Replace("\"","\\\"");
        this.mWebSocket.Send(mjsMsgProxy.ToString()) ;

        Debug.Log( "send msg : " + mjsMsgProxy.ToString() );
        if ( callBack != null ) // reg call back ;
        {
            var p = new KeyValuePair<eMsgType,IOneMsgCallback>(msgID,callBack);
            this.vMsgCallBack.Add(p) ; 
        }
        return true;
    }
    protected void doConnect()
    {
        if ( eNetworkState.eNet_Connecting == this.mNetState  )
        {
            Debug.LogWarning("already connecting , so do not connect again");
            return ;
        }

        if ( eNetworkState.eNet_Connected == this.mNetState )
        {
            Debug.Log("network is connected , do not connect again");
            return ;
        }

        if ( this.mWebSocket != null )
        {
            this.mWebSocket.Close();
            this.mWebSocket = null ;
        }

        Debug.Log( "do connecting..." );
        this.mNetState = eNetworkState.eNet_Connecting;
        this.mWebSocket = new WebSocketUnity(this.mDstIP,this) ;
        this.mWebSocket.Open();
#if UNITY_EDITOR
        Debug.Log("connecting ip = " + this.mDstIP);
#endif
    }

    protected void close()
    {
        if ( eNetworkState.eNet_Connected != this.mNetState )
        {
            Debug.LogWarning("network is not connected , why closed it ? curState = " + this.mNetState );
            return ;    
        }

        Debug.Log( "client self colse" );
        this.mWebSocket.Close();
        this.OnWebSocketUnityClose("client close");
    }
 
    protected IEnumerator doSendHeatBet()
    {
        // send heat bet ;
        if ( eNetworkState.eNet_Connected != this.mNetState )
        {
            Debug.LogWarning( "socket is not open , can not send heat bet " );
            yield break;
        }

        byte[] h = { (byte)'H'} ;
        this.mWebSocket.Send(h);
        //Debug.Log("do send heat beat ");
        this.isRecievedHeatBet = false ;
        yield return new WaitForSeconds (Network.TIME_HEAT_BEAT);

        if ( this.isRecievedHeatBet == false )
        {
            if ( eNetworkState.eNet_Connected != this.mNetState )
            {
                Debug.Log("already known disconnect so need not notify close");
                yield break;
            }

            // do disconnected ; heat beat time out 
            Debug.Log( "heat bet out , invoke on close event" );
            this.close();
            yield break;
        }
        this.StartCoroutine(this.doSendHeatBet());
    }

    protected void tryReconnect()
    {
       Debug.Log(" invoke doTryReconnect");
       if ( eNetworkState.eNet_Connected == this.mNetState )
       {
           return ;
       }

       doConnect();
    }

    // websocket delegate
    // Event when the connection has been opened
    private void Update() {
        if ( this.isStartHeadBet )
        {
            this.StartCoroutine(this.doSendHeatBet());
            this.isStartHeadBet = false ;
        }

        if ( this.isStartReconnect )
        {
            this.isStartReconnect = false ;
            this.Invoke("tryReconnect",2.5f);
        }
    }

    private bool processInternalMsg( eMsgType msgID , JSONObject jsm )
    {
        switch ( msgID )
        {
            case eMsgType.MSG_VERIFY_CLIENT:
            {
                if ( (int)jsm["nRet"].Number != 0 )
                {
                    Debug.LogError("can not verify this client ret :" + jsm["nRet"] );
                    EventDispatcher.getInstance().dispatch(Network.EVENT_FAILED);
                    return true;
                }
                Debug.Log("verifyed session id = " + jsm["nSessionID"] + " ret =" + jsm["nRet"] );
                // decide if need reconnect 
                if ( this.SessionID == 0 ) // we need not reconnect 
                {
                    this.SessionID = (int)jsm["nSessionID"].Number;
                    EventDispatcher.getInstance().dispatch(Network.EVENT_OPEN);
                    return true;
                }

                Debug.Log("go on do reconnect session id = " + this.SessionID );
                // we need do reconnect 
                JSONObject jsRec = new JSONObject();
                jsRec["nSessionID"] = this.SessionID;
                this.sendMsg(jsRec,eMsgType.MSG_RECONNECT,eMsgPort.ID_MSG_PORT_GATE,0) ;
                break ;
            }
            case eMsgType.MSG_RECONNECT:
            {
                int ret = (int)jsm["nRet"].Number;
                this.SessionID = (int)jsm["sessionID"].Number ;
                var ev = Network.EVENT_RECONNECT ;
                if ( 0 != ret ) // reconnect ok 
                {
                    ev = Network.EVENT_RECONNECTED_FAILED ;
                }
                EventDispatcher.getInstance().dispatch(ev);
                Debug.Log("reconnect result = " + ev );
                break;
            }
            default:
            return false ;
        }
        return true ;
    }
	public void OnWebSocketUnityOpen(string sender)
    {
        Debug.Log(" web socket on open " );
        this.mNetState = eNetworkState.eNet_Connected ;
        this.isStartHeadBet = true ;
        this.isUseBackUpIP = false ;  // in order to promise next trying to connect , begin from defaultIP. 

        // verify client ;
        JSONObject jsMsg = new JSONObject() ;
        var self = this ;
        this.sendMsg(jsMsg,eMsgType.MSG_VERIFY_CLIENT,eMsgPort.ID_MSG_PORT_GATE,0 ) ;      
    }
	
	// Event when the connection has been closed
	public void OnWebSocketUnityClose(string reason)
    {
        Debug.Log( "OnWebSocketUnityClose: " + reason );
        this.onNetworkFailed();
    }
	
	// Event when the websocket receive a message
	public void OnWebSocketUnityReceiveMessage(string message)
    {
        //cc.log(" on msg " + ev.data );
        if ( message == "H" )
        {
            //cc.log(" do read heat bet on msg " + ev.data );
            this.isRecievedHeatBet = true ;
            return ;
        }

        Debug.Log( "on msg : " + message );
        var msg = JSONObject.Parse(message) ;
        if ( msg == null )
        {
            Debug.LogError("can not pase set msg : " + message );
            return ;
        }

        eMsgType nMsgID = (eMsgType)(msg[Network.MSG_ID].Number);
        if ( this.processInternalMsg(nMsgID,msg) )
        {
            return ;
        }
        // check call back 
        List<KeyValuePair<eMsgType,IOneMsgCallback>> vWillDeleter = null ;
        foreach (var item in this.vMsgCallBack )
        {
            if ( item.Key != nMsgID )
            {
                continue ;
            }

            if ( null == vWillDeleter )
            {
               vWillDeleter = new List<KeyValuePair<eMsgType,IOneMsgCallback>>();
            }

            vWillDeleter.Add(item);
            if ( item.Value(msg) )
            {
                return ;
            }
        } 
        // do remove ;
        foreach (var delItem in vWillDeleter )
        {
            this.vMsgCallBack.Remove(delItem);
        }
        vWillDeleter = null ;
        //console.log("dispath msg id " + msg );
        /// dispatch event ;
        this.mEventMsg.type = Network.EVENT_MSG ;
        this.mEventMsg.msgID = nMsgID ;
        this.mEventMsg.msgData = msg ;
        EventDispatcher.getInstance().dispatch(mEventMsg);
    }
	
	// Event when the websocket receive data (on mobile : ios and android)
	// you need to decode it and call after the same callback than PC
	public void OnWebSocketUnityReceiveDataOnMobile(string base64EncodedData)
    {
        Debug.LogWarning("shoul not recived this msg : ");
    }
	
	// Event when the websocket receive data
	public void OnWebSocketUnityReceiveData(byte[] data)
    {
        Debug.LogWarning("shoul not recived this msg : ");
    }
	
	// Event when an error occurs
	public void OnWebSocketUnityError(string error)
    {
        Debug.LogWarning("OnWebSocketUnityError: " + error );
        this.onNetworkFailed();
    }

    void onNetworkFailed()
    {
        if ( eNetworkState.eNet_Connected != this.mNetState && eNetworkState.eNet_Connecting != this.mNetState )
        {
            Debug.Log( "onNetworkFailed.. curState : " + this.mNetState );
            return ;
        }

        if ( eNetworkState.eNet_Connecting == this.mNetState )  // if connected fail , try another ip , swiching between defaultIP and backupIP
        {
            this.isUseBackUpIP = !this.isUseBackUpIP ;
        }

        EventDispatcher.getInstance().dispatch( eNetworkState.eNet_Connected == this.mNetState ? Network.EVENT_DISCONNECTED : Network.EVENT_FAILED ) ;
        this.mNetState = eNetworkState.eNet_Disconnect ;

        Debug.Log( "delay try reconected" );
        // delay invoker reconnect
        this.isStartReconnect = true ;
        //this.Invoke("tryReconnect",3);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if ( pauseStatus )
        {
            Debug.LogWarning("network app background , close socket  pause = true");
            StopAllCoroutines();
            this.mNetState = eNetworkState.eNet_Uninit ;  // assign befor close , to forbid websocketError or websocketClose logic from excuting ;
            if ( null != this.mWebSocket )
            {
                this.mWebSocket.Close();
            }
            this.mWebSocket = null ;
            EventDispatcher.getInstance().dispatch( Network.EVENT_DISCONNECTED ) ;
        }
        else
        {
            if ( this.isSetedUp == false )
            {
                Debug.LogWarning("net work not set up , so do nothing");
                return ;
            }
            Debug.LogWarning("network app reactive agian , do connnect  pause = false");
            this.isUseBackUpIP = false ;
            this.doConnect();
        }
    }
}
