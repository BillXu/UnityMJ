using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public class MainScene : MonoBehaviour
{
    // Start is called before the first frame update
    public DlgJoin mDlgJoin = null ;
    public DlgCreateRoom mDlgCreate = null ;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onJoin()
    {
        this.mDlgJoin.showDlg<string>(( DlgBase dlg , JSONObject js )=>{
            this.onJoinRoomDlgResult(int.Parse(this.mDlgJoin.resultNumber)) ;
        },null,null) ;
    }

    public void onCreate()
    {
        this.mDlgCreate.showDlg<string>(( DlgBase dlg, JSONObject js )=>{
            var uid = ClientPlayerData.getInstance().getSelfUID(); 
            js["uid"] = uid;
            var port = Utility.getMsgPortByGameType((eGameType)js["gameType"].Number);
            Network.getInstance().sendMsg(js,eMsgType.MSG_CREATE_ROOM,port,uid,( JSONObject msg )=>{
            var ret = (int)msg["ret"].Number ;
            if ( ret != 0  )
            {
                Prompt.promptText( "create room is failed = " + ret );
                return true;
            }
            var roomID = (int)msg["roomID"].Number ;
            Prompt.promptText( "create room success" );
            this.onJoinRoomDlgResult(roomID) ;
            return true ;
        });
        },null,null) ;
    }

    protected void onJoinRoomDlgResult( int nJoinRoomID )
    {
        Debug.Log( "onJoinRoomDlgResult " + nJoinRoomID );
        var msg = new JSONObject() ;
        msg["roomID"] = nJoinRoomID;
        msg["uid"] = ClientPlayerData.getInstance().getSelfUID();
        var port = Utility.getMsgPortByRoomID( nJoinRoomID ); 
        if ( eMsgPort.ID_MSG_PORT_ALL_SERVER <= port || port > eMsgPort.ID_MSG_PORT_MAX  )
        {
            Prompt.promptText( "房间不存在或已经解散 code" + 0 );
            return ;
        }

        Network.getInstance().sendMsg(msg,eMsgType.MSG_ENTER_ROOM,port,nJoinRoomID,( JSONObject jsmsg )=>
        {
            var ret = (int)jsmsg["ret"].Number ;
            if ( ret != 0 )
            {
                Prompt.promptText( "房间不存在或已经解散 code" + ret );
                return true;
            }
            Debug.Log( "set join room id = " + nJoinRoomID );
            ClientPlayerData.getInstance().getComponentData<PlayerBaseData>().stayInRoomID = nJoinRoomID;
            this.mDlgJoin.closeDlg();
            //cc.director.loadScene(SceneName.Scene_Room ) ;
            return true ;
        } );
    }
}
