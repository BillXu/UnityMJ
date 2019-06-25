using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
using UnityEngine.SceneManagement ;
public class RoomData : NetBehaviour
{
    // Start is called before the first frame update
    public IRoomDataDelegate mSceneDelegate ;
    public RoomBaseData mBaseData = new RoomBaseData();
    public ResultSingleData mSinglResultData = new ResultSingleData();
    public ResultTotalData mTotalResultData = new ResultTotalData();
    public List<RoomPlayerData> mPlayers = new List<RoomPlayerData>();
    private void Start() {
        mPlayers.Add(null);
        mPlayers.Add(null);
        mPlayers.Add(null);
        mPlayers.Add(null);
        this.registerEvent(PlayerInfoDataCacher.EVENT_RECIEVED_PLAYER_INFO_DATA);
        this.registerEvent(VoiceManager.EVENT_UPLOAD_FINISH);
        this.registerEvent(VoiceManager.EVENT_PLAY_BEGIN);
        this.registerEvent(VoiceManager.EVENT_PLAY_FINISH);
        if ( ClientPlayerData.getInstance() != null )
        {
            this.reqRoomInfo(ClientPlayerData.getInstance().getComponentData<PlayerBaseData>().stayInRoomID) ;
        }
    }
    protected override bool onEvent(EventArg arg)
    {
        if ( PlayerInfoDataCacher.EVENT_RECIEVED_PLAYER_INFO_DATA == arg.type )
        {
            var eventObj = (EventWithObject<PlayerInfoData>)arg ;
            var info = eventObj.argObject ;
            if ( this.mSceneDelegate != null )
            {
                this.mSceneDelegate.onRecivedPlayerBrifeData(info);
            }
            return false;    
        }

        if ( VoiceManager.EVENT_UPLOAD_FINISH == arg.type )
        {
            var eventObj = (EventWithObject<string>)arg ;
            this.sendPlayerChat(eChatMsgType.eChatMsg_Voice,eventObj.argObject );
            return true ;
        }

        if ( VoiceManager.EVENT_PLAY_BEGIN == arg.type )
        {
            var eventObj = (EventWithObject<int>)arg ;
            this.mSceneDelegate.onPlayerChatMsg(eventObj.argObject,eChatMsgType.eChatMsg_Voice,"begin") ;
            return true ;
        }

        if ( VoiceManager.EVENT_PLAY_FINISH == arg.type )
        {
            var eventObj = (EventWithObject<int>)arg ;
            this.mSceneDelegate.onPlayerChatMsg(eventObj.argObject,eChatMsgType.eChatMsg_Voice,"end") ;
            return true ;
        }

        return base.onEvent(arg);
    }
    protected override bool onMsg( eMsgType nMsgID , JSONObject msg )
    {
        if ( this.mBaseData.isRoomOverd )  // if room over we can not recieved any msg , user will tansfer scene ;
        {
            return false ;
        } 

        switch ( nMsgID )
        {
            case eMsgType.MSG_REQUEST_ROOM_INFO:
            {
                var isOk = msg.ContainsKey( "ret" ) && ((int)msg["ret"].Number == 0 );
                if ( isOk == false )
                {
                    Prompt.promptText( "房间已经不存在了！" );
                    SceneManager.LoadScene(GameConfig.getInstance().SCENE_NAME_MAIN) ;
                }
            }
            break;
            case eMsgType.MSG_ROOM_INFO:
            {
                this.mBaseData.parseInfo(msg) ;
                this.mSceneDelegate.onRecivedRoomInfo(this.mBaseData);
            }
            break ;
            case eMsgType.MSG_ROOM_PLAYER_INFO:
            {
                var vMsgPlayers = msg["players"].Array ;
                if ( vMsgPlayers != null )
                {
                    foreach ( var item in vMsgPlayers )
                    {
                        this.onRecievedPlayer(item.Obj,false);
                    }
                }

                this.mSceneDelegate.onRecivedAllPlayers(this.mPlayers);
                
                this.reqActList();
            }
            break ;
            case eMsgType.MSG_ROOM_PLAYER_EXCHANGE_SEAT:
            {
                List<RoomPlayerData> tmpPlayers = new List<RoomPlayerData>();
                tmpPlayers.AddRange(this.mPlayers);
                this.mPlayers.Clear();
                this.mPlayers.Add(null);
                this.mPlayers.Add(null);
                this.mPlayers.Add(null);
                this.mPlayers.Add(null);
                var vPlayers = msg["detail"].Array;
                foreach (var item in vPlayers )
                {
                    int idx = (int)item.Obj["idx"].Number;
                    int uid = (int)item.Obj["uid"].Number;
                    var tmp = tmpPlayers.Find(( RoomPlayerData p )=>{ return p.nUID == uid ;} );
                    if ( tmp == null )
                    {
                        Debug.LogError("why client do not have player uid = " + uid );
                        continue ;
                    }
                    this.mPlayers[idx] = tmp;
                    tmp.idx = idx ;
                }
                tmpPlayers.Clear();
                tmpPlayers = null ;
                this.mSceneDelegate.onExchangedSeat();
            }
            break;
            case eMsgType.MSG_ROOM_SIT_DOWN:
            {
                this.onRecievedPlayer(msg,true);
            }
            break ;
            case eMsgType.MSG_ROOM_STAND_UP:
            {
                var idx = (int)msg["idx"].Number ;
                var uid = (int)msg["uid"].Number ;
                if ( this.mPlayers[idx] == null || this.mPlayers[idx].nUID != uid )
                {
                    Debug.LogError( "idx and uid not match idx = " + idx + " uid = " + uid );
                    return true ;
                }
                this.mPlayers[idx].clear() ;
                this.mSceneDelegate.onPlayerStandUp(idx);
            }
            break ;
            case eMsgType.MSG_ROOM_PLAYER_READY:
            {
                var idx = (int)msg["idx"].Number ;
                if ( null == this.mPlayers[idx] )
                {
                    Debug.LogError( "idx player is null , how to set ready " + idx );
                    return true;
                } 
                this.mPlayers[idx].isReady = true ;
            }
            break ;
            case eMsgType.MSG_PLAYER_LEAVE_ROOM:
            {
                Debug.Log("leave room ret = " + msg["ret"].Number );   
                if ( (int)msg["ret"].Number == 0 )
                {
                    SceneManager.LoadScene(GameConfig.getInstance().SCENE_NAME_MAIN) ;
                }
            }
            break;
            case eMsgType.MSG_ROOM_DO_OPEN:
            {
                this.mBaseData.isRoomOpen = true ;
            }
            break ;
            default:
            return onMsgPart2(nMsgID,msg);
        } 
        return true ;
    }
    bool onMsgPart2( eMsgType nMsgID , JSONObject msg )
    {
        switch ( nMsgID )
        {
            case eMsgType.MSG_PLAYER_ACT:
            {
                var nret = (int)msg["ret"].Number;
                if ( nret != 0  )
                {
                    Debug.LogError( "act error nret = " + nret );
                    Prompt.promptText( "操作失败code " + nret );
                    
                    var selfPlayer = this.mPlayers[this.getSelfIdx()];
                    this.mSceneDelegate.onMJActError(selfPlayer); // do refresh self cards ;
                }
            }
            break ;
            case eMsgType.MSG_ROOM_ACT:
            {
                this.processRoomActMsg(msg);
            } 
            break ;
            case eMsgType.MSG_PLAYER_WAIT_ACT_ABOUT_OTHER_CARD:
            {
                this.mBaseData.otherCanActCard = (int)msg["cardNum"].Number;
                this.mSceneDelegate.showActOptsAboutOtherCard(msg["acts"].Array);
            }
            break;
            case eMsgType.MSG_ROOM_MQMJ_WAIT_ACT_AFTER_CP:
            case eMsgType.MSG_PLAYER_WAIT_ACT_AFTER_RECEIVED_CARD:
            {
                this.mSceneDelegate.showActOptsWhenRecivedCards(msg["acts"].Array) ;
            }
            break;
            case eMsgType.MSG_ROOM_MQMJ_PLAYER_HU:
            {
                var isZiMo = (int)msg["isZiMo"].Number == 1 ;
                var huCard = (int)msg["huCard"].Number ;
                if ( isZiMo )
                {
                    var huIdx = (int)(msg["detail"].Obj["huIdx"].Number) ;
                    this.mSceneDelegate.onPlayerActHu(huIdx,huCard,huIdx );
                }
                else
                {
                    var vHuPlayers = msg["detail"].Obj["huPlayers"].Array ;
                    foreach ( var v in vHuPlayers )
                    {
                        var idx = (int)v.Obj["idx"].Number ;
                        this.mSceneDelegate.onPlayerActHu(idx,huCard,this.mBaseData.lastChuIdx ) ;
                    }
                }
            }
            break;
            case eMsgType.MSG_ROOM_CFMJ_GAME_WILL_START:
            {
                this.willStartGame(msg);
            }
            break;
            case eMsgType.MSG_ROOM_MQMJ_GAME_START:
            {
                this.willStartGame(msg);
                this.startGame(msg);
            }
            break ;
            case eMsgType.MSG_ROOM_SCMJ_GAME_END:
            {
                this.mSinglResultData.parseResult(msg);
                this.mSceneDelegate.onGameEnd(this.mSinglResultData) ;
                this.endGame();
            }
            break ;
            case eMsgType.MSG_ROOM_GAME_OVER:
            {
                // this.pdlgDismiss.closeDlg();
                // this.pRoomData.isRoomOver = true ;
                // this.pdlgRoomOver.refresh(msg,this.pRoomData) ;
                // if ( false == this.pdlgSingleReuslt.node.active )
                // {
                //     this.pdlgRoomOver.showDlg();
                // }
                this.mTotalResultData.parseResult(msg);
                this.mSceneDelegate.onRoomOvered( this.mTotalResultData ) ;
                this.mBaseData.isRoomOverd = true ;
            }
            break ;
            case eMsgType.MSG_ROOM_APPLY_DISMISS_VIP_ROOM:
            {
                this.mBaseData.applyDismissIdx = (int)msg["applyerIdx"].Number;
                this.mBaseData.dimissRoomLeftTime = 300 ;
                if ( this.mBaseData.agreeDismissIdx == null )
                {
                    this.mBaseData.agreeDismissIdx = new List<int>();
                }
                this.mBaseData.agreeDismissIdx.Clear();
                this.mBaseData.agreeDismissIdx.Add(this.mBaseData.applyDismissIdx);
                this.mSceneDelegate.onApplyDismisRoom( this.mBaseData.applyDismissIdx );
            }
            break ;
            case eMsgType.MSG_ROOM_REPLY_DISSMISS_VIP_ROOM_APPLY:
            {
                this.mSceneDelegate.onReplayDismissRoom( (int)msg["idx"].Number,(int)msg["reply"].Number == 1 ) ;
            }
            break;
            case eMsgType.MSG_VIP_ROOM_DO_CLOSED:
            {
                this.mSceneDelegate.onRoomDoClosed( (int)msg["isDismiss"].Number == 1 );
            }
            break ;
            case eMsgType.MSG_ROOM_REFRESH_NET_STATE:
            {
                var idx = (int)msg["idx"].Number;
                var isOnline = msg["state"].Number == 0 ;
                var p = this.mPlayers[idx] ;
                if ( p == null || p.isEmpty() )
                {
                    Debug.LogError("refresh net state player is null or empty idx = " + idx );
                    break ;
                }
                p.isOnline = isOnline ;
                this.mSceneDelegate.onPlayerNetStateChanged(idx,isOnline) ;
            }
            break;
            case eMsgType.MSG_ROOM_CHAT_MSG:
            {
                var type = (eChatMsgType)msg["type"].Number ;
                var content = msg["content"].Str ;
                var idx = (int)msg["playerIdx"].Number ;
                if ( eChatMsgType.eChatMsg_Voice == type )
                {
                    VoiceManager.getInstance().playVoice(content,idx ) ;
                    break ;
                }
                this.mSceneDelegate.onPlayerChatMsg(idx,type,content) ;
            }
            break ;
            case eMsgType.MSG_ROOM_INTERACT_EMOJI:
            {
                var invokerIdx = (int)msg["invokerIdx"].Number ;
                var targetIdx = (int)msg["targetIdx"].Number ;
                var emojiIdx = (int)msg["emoji"].Number ;
                this.mSceneDelegate.onInteractEmoji(invokerIdx,targetIdx,emojiIdx) ;
            }
            break ;
        }
        return true ; 
    }
    void onRecievedPlayer( JSONObject jsInfo, bool informDelegate )
    {
        var idx = (int)jsInfo["idx"].Number;
        if ( this.mPlayers[idx] == null )
        {
            this.mPlayers[idx] = new RoomPlayerData();
            this.mPlayers[idx].clear();
            Debug.LogWarning("find a null pos idx = " + idx);
        } 

        if ( this.mPlayers[idx].isEmpty() == false )
        {
            Debug.LogError("why the same pos , have two player ?");
        }
        this.mPlayers[idx].parseBaseInfo(jsInfo);
        this.mPlayers[idx].isSelf = ClientPlayerData.getInstance().getSelfUID() == this.mPlayers[idx].nUID ;
        if ( informDelegate )
        {
            this.mSceneDelegate.onPlayerSitDown(this.mPlayers[idx]);
        }
    }
    void processRoomActMsg( JSONObject msg )
    {
        // svr : { idx : 0 , actType : 234, card : 23, gangCard : 12, eatWith : [22,33], huType : 23, fanShu : 23  }
        var svrIdx = (int)msg["idx"].Number ;
        eMJActType actType = (eMJActType)msg["actType"].Number;
        var targetCard = (int)msg["card"].Number ;
        var RoomPlayer = this.mPlayers[svrIdx];
        int invokerIdx = -1;
        if ( msg.ContainsKey("invokerIdx") )
        {
            invokerIdx = (int)msg["invokerIdx"].Number;
        }
        else
        {
            invokerIdx = this.mBaseData.lastChuIdx;
        }
        this.mBaseData.curActIdx = svrIdx ;
        switch ( actType )
        {
            case eMJActType.eMJAct_Mo:
            {
                RoomPlayer.onMo(targetCard);
                this.mBaseData.leftMJCnt -= 1 ;
                this.mSceneDelegate.onPlayerActMo(svrIdx,targetCard);
            }
            break ;
            case eMJActType.eMJAct_Chu:
            {
                this.mBaseData.lastChuIdx = svrIdx ;
                this.mBaseData.otherCanActCard = targetCard ;
                RoomPlayer.onChu(targetCard) ;
                this.mSceneDelegate.onPlayerActChu(svrIdx,targetCard);
            }
            break ;
            case eMJActType.eMJAct_Chi:
            {
                var with = msg["eatWith"].Array;
                var withA = (int)with[0].Number;
                var withB = (int)with[1].Number;
                RoomPlayer.onChi(targetCard,withA,withB) ;
                if ( invokerIdx == -1 )
                {
                    Debug.LogError("chi act do not have invoker idx key ");
                    invokerIdx = (svrIdx - 1 + this.mBaseData.seatCnt) % this.mBaseData.seatCnt ;
                }
                this.mPlayers[invokerIdx].removeChu(targetCard);
                this.mSceneDelegate.onPlayerActChi(svrIdx,targetCard,withA,withB, invokerIdx ) ;
            }
            break ;
            case eMJActType.eMJAct_Peng:
            {
                RoomPlayer.onPeng(targetCard,invokerIdx) ;
                if ( invokerIdx == -1 )
                {
                    Debug.LogError("peng act do not have invoker idx key ");
                    break;
                }
                this.mPlayers[invokerIdx].removeChu(targetCard);
                this.mSceneDelegate.onPlayerActPeng(svrIdx,targetCard,invokerIdx ) ;
            }
            break ;
            case eMJActType.eMJAct_AnGang:
            {
                RoomPlayer.onAnGang(targetCard,(int)msg["gangCard"].Number);
                this.mBaseData.leftMJCnt -= 1 ;
                this.mSceneDelegate.onPlayerActAnGang(svrIdx,targetCard,(int)msg["gangCard"].Number );
            }
            break;
            case eMJActType.eMJAct_BuGang_Done:
            case eMJActType.eMJAct_BuGang:
            {
                this.mBaseData.leftMJCnt -= 1 ;
                RoomPlayer.onBuGang(targetCard,(int)msg["gangCard"].Number) ;
                this.mSceneDelegate.onPlayerActBuGang(svrIdx,targetCard,(int)msg["gangCard"].Number );
            }
            break;
            case eMJActType.eMJAct_MingGang:
            {
                this.mBaseData.leftMJCnt -= 1 ;
                RoomPlayer.onMingGang(targetCard,(int)msg["gangCard"].Number,invokerIdx) ;
                if ( invokerIdx == -1 )
                {
                    Debug.LogError("mingGang act do not have invoker idx key ");
                    break;
                }
                this.mPlayers[invokerIdx].removeChu(targetCard);
                this.mSceneDelegate.onPlayerActMingGang(svrIdx,targetCard,invokerIdx,(int)msg["gangCard"].Number);
            }
            break;
            case eMJActType.eMJAct_Hu:
            {
                RoomPlayer.onHu(targetCard);
                if ( invokerIdx == -1 )
                {
                    Debug.LogError("mingGang act do not have invoker idx key ");
                    break;
                }

                if ( invokerIdx != svrIdx )
                {
                    this.mPlayers[invokerIdx].removeChu(targetCard);
                }
                this.mSceneDelegate.onPlayerActHu(svrIdx,targetCard,invokerIdx ) ;
            }
            break ;
            default:
            Debug.LogError( "unknown act type = " + actType );
            return ;
        }
    }
    void willStartGame( JSONObject jsMsg )
    {
        this.mBaseData.onGameWillStart(jsMsg);
        this.mSceneDelegate.onGameStart();
    }
    void startGame( JSONObject jsMsg )
    {
        foreach (var item in this.mPlayers )
        {
            if ( null == item || item.isEmpty() )
            {
                continue;
            }

            if ( item.isSelf )
            {
                var cards = jsMsg["cards"].Array ;
                item.onRecivedHoldCard(cards,cards.Length) ;
            }
            else
            {
                item.onRecivedHoldCard(null,this.mBaseData.bankerIdx == item.idx ? 14 : 13 );
            }
        }
        this.mSceneDelegate.onDistributedCards();
    }
    void endGame()
    {
        this.mBaseData.onEndGame();
        foreach (var item in this.mPlayers )
        {
            if ( null != item && item.isEmpty() == false )
            {
                item.onEndGame();
            }
        }
    }
    public void reqRoomInfo( int nRoomID )
    {
        var msgReqRoomInfo = new JSONObject();
        var port = Utility.getMsgPortByRoomID(nRoomID);
        Network.getInstance().sendMsg(msgReqRoomInfo,eMsgType.MSG_REQUEST_ROOM_INFO,port,nRoomID) ;
    }
    public int getSelfIdx()
    {
        foreach (var item in this.mPlayers )
        {
            if ( item != null && item.isEmpty() == false && item.isSelf )
            {
                return item.idx ;
            }
        }
        return -1 ;
    }

    public RoomPlayerData getPlayerDataByUID( int uid )
    {
        foreach (var item in this.mPlayers )
        {
            if ( item != null && item.isEmpty() == false && item.nUID == uid )
            {
                return item ;
            }
        }
        return null ;
    }
    void onPlayerChoseDoActAboutOtherCard( eMJActType act )
    {
        if ( act != eMJActType.eMJAct_Chi )
        {
            var msg = new JSONObject() ;
            msg["actType"] = (int)act ;
            msg["card"] = this.mBaseData.otherCanActCard ;
            this.sendRoomMsg(msg,eMsgType.MSG_PLAYER_ACT) ;
            return ;
        }

        // check if have eat option ;
        var player = this.mPlayers[this.getSelfIdx()];
        List<eEatType> vL = new List<eEatType>();
        player.getEatOpts(vL,this.mBaseData.otherCanActCard ) ;
        if ( vL.Count == 1 )
        {
            this.onPlayerChoseEatType(vL[0]);
        }
        else
        {
            // show chose eat type result ;
            this.mSceneDelegate.showEatOpts( vL,this.mBaseData.otherCanActCard );
        }
    }

    void onPlayerChoseActAboutRecievedCard( eMJActType act , int targetCard )
    {
        var player = this.mPlayers[this.getSelfIdx()];
        switch ( act )
        {
            case eMJActType.eMJAct_BuGang:
            case eMJActType.eMJAct_AnGang:
            {
                List<int> gangOpts = new List<int>();
                player.getGangOpts(gangOpts);
                if ( gangOpts.Count == 1 )
                {
                    this.onPlayerChosedGangCard(gangOpts[0]) ;
                }
                else
                {
                    // show chose gang card dlg ;
                    this.mSceneDelegate.showGangOpts(gangOpts);
                }
            }
            return;
            case eMJActType.eMJAct_Hu:
            case eMJActType.eMJAct_Pass:
            {
                targetCard = player.newMoChard ;
            }
            break;
            case eMJActType.eMJAct_Chu:
            {

            }
            break;
        }

        var msg = new JSONObject() ;
        msg["actType"] = (int)act ;
        msg["card"] = targetCard ;
        this.sendRoomMsg(msg,eMsgType.MSG_PLAYER_ACT) ;
    }

    public void onPlayerChosedAct( eMJActType act , int targetCard )
    {
        var player = this.mPlayers[this.getSelfIdx()];
        if ( act == eMJActType.eMJAct_Chu || player.vHoldCards.Count % 3 == 2 )
        {
            onPlayerChoseActAboutRecievedCard(act,targetCard);
        }
        else
        {
            onPlayerChoseDoActAboutOtherCard(act);
        }
    }
    public void onPlayerChosedGangCard( int cardForGang ) // must be anGang or bu Gang ;
    {
        var player = this.mPlayers[this.getSelfIdx()];

        int type = (int)eMJActType.eMJAct_AnGang;
        var info = player.getActedCardInfo(cardForGang);
        if ( info != null )
        {
            type = (int)eMJActType.eMJAct_BuGang;
        }
        var msg = new JSONObject() ;
        msg["actType"] = type;
        msg["card"] = cardForGang ;
        this.sendRoomMsg(msg,eMsgType.MSG_PLAYER_ACT) ;
    }

    public void onPlayerChoseEatType( eEatType type )
    {
        JSONArray v = new JSONArray();
        int nTargetCard = this.mBaseData.otherCanActCard ;
        switch ( type )
        {
            case eEatType.eEat_Left:
            {
                v.Add(nTargetCard + 1 );
                v.Add(nTargetCard + 2 );
            }
            break;
            case eEatType.eEat_Middle:
            {
                v.Add(nTargetCard - 1 );
                v.Add(nTargetCard + 1 );
            }
            break;
            case eEatType.eEat_Righ:
            {
                v.Add(nTargetCard - 1 );
                v.Add(nTargetCard - 2 );
            }
            break;
        }

        var msg = new JSONObject() ;
        msg["actType"] = (int)eMJActType.eMJAct_Chi ;
        msg["card"] = this.mBaseData.otherCanActCard ;
        msg["eatWith"] = v;
        this.sendRoomMsg(msg,eMsgType.MSG_PLAYER_ACT) ;
    }

    public bool sendRoomMsg( JSONObject jsMsg , eMsgType msgID )
    {
        jsMsg["dstRoomID"] = this.mBaseData.roomID;
        return sendMsg(jsMsg,msgID,Utility.getMsgPortByRoomID(this.mBaseData.roomID),this.mBaseData.roomID );
    }

    public void onPlayerClickedSitDown( int svrIdx )
    {
        var msg = new JSONObject() ;
        msg["idx"] = svrIdx ;
        this.sendRoomMsg(msg,eMsgType.MSG_PLAYER_SIT_DOWN) ;
    }

    public void onPlayerApplyLeave()
    {
        var msg = new JSONObject() ;
        this.sendRoomMsg(msg,eMsgType.MSG_PLAYER_LEAVE_ROOM) ;
    }

    public void onPlayerApplyDismissRoom()
    {
        // send msg ;
        var msg = new JSONObject() ;
        this.sendRoomMsg(msg,eMsgType.MSG_APPLY_DISMISS_VIP_ROOM) ;
    }

    public void onPlayerReplyDismiss( bool isAgree )
    {
        // send msg ;
        var msg = new JSONObject() ;
        msg["reply"] = isAgree ? 1 : 0 ;
        this.sendRoomMsg(msg,eMsgType.MSG_REPLY_DISSMISS_VIP_ROOM_APPLY) ;
    }

    public void onPlayerReady()
    {
        var msg = new JSONObject() ;
        this.sendRoomMsg(msg,eMsgType.MSG_PLAYER_SET_READY) ;
    }

    public void reqActList()
    {
        var msg = new JSONObject() ;
        this.sendRoomMsg(msg,eMsgType.MSG_REQ_ACT_LIST ) ;
    }
    
    protected override void onDisconnect()
    {
        Prompt.promptText("网络连接丢失，尝试重连");
    }
    protected override void onReconnect( bool isSuccess )
    {
        if ( isSuccess )
        {
            Prompt.promptText("网络重连成功！");
            this.reqRoomInfo(ClientPlayerData.getInstance().getComponentData<PlayerBaseData>().stayInRoomID) ;
        }
        else
        {
            SceneManager.LoadScene(GameConfig.getInstance().SCENE_NAME_LOGIN ) ;
        }
    }

    public int getAlreadyGangCnt()
    {
        int cnt = 0 ;
        foreach (var item in this.mPlayers )
        {
            if ( item == null || item.isEmpty() )
            {
                continue ;
            } 
            cnt += item.getGangCnt();
        }
        return cnt ;
    }

    public void sendPlayerChat( eChatMsgType type , string strContent )
    {
        if ( getSelfIdx() == -1 )
        {
            Prompt.promptText( "您没有坐下，不能发言。" );
            return ;
        }

        var msg = new JSONObject();
        msg["type"] = (int)type ;
        msg["content"] = strContent ;
        this.sendRoomMsg( msg,eMsgType.MSG_PLAYER_CHAT_MSG ) ;
    }

    public void sendPlayerInteractEmoji( int targetIdx , int emojiIdx )
    {
        if ( getSelfIdx() == -1 )
        {
            Prompt.promptText( "您没有坐下，不能发言。" );
            return ;
        }

        var msg = new JSONObject();
        msg["targetIdx"] = targetIdx ;
        msg["emoji"] = emojiIdx ;
        this.sendRoomMsg( msg,eMsgType.MSG_PLAYER_INTERACT_EMOJI ) ;
    }

    public void replayLastVoice( int playerUID )
    {
        VoiceManager.getInstance().replayCacheVoice(playerUID);
    }
}
