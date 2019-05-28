using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public class RoomScene : MonoBehaviour, IRoomDataDelegate
{
    // Start is called before the first frame update
    public RoomData mRoomData = null ;
    public LayerCards mLayerCard = null ;
    public SeatOrientation mSeatOriention = null;
    public CountDownTimer mDeskTimer = null;
    public LayerRoomInfo mLayerInfo = null ;
    public LayerPlayers mLayerPlayers = null ;
    public LayerDlg mLayerDlg = null ;
    private void Awake() {
        mRoomData.mSceneDelegate = this ;
    }
    void Start()
    {
        this.mLayerCard.selfIdx = 0 ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onRecivedRoomInfo( RoomBaseData info )
    {
        this.mLayerCard.clear();
        this.mLayerInfo.refresh(this.mRoomData.mBaseData);
        this.mSeatOriention.setCurActIdx(info.curActIdx) ;
        this.mDeskTimer.resetTime();
    }

    public void onRecivedAllPlayers( List<RoomPlayerData> vPlayers )
    {
        var info = this.mRoomData.mBaseData ;
        var selfIdx = this.mRoomData.getSelfIdx() ;

        this.mLayerCard.selfIdx = selfIdx < 0 ? 0 : selfIdx;
        this.mDeskTimer.selfIdx = selfIdx < 0 ? 0 : selfIdx;
        this.mLayerCard.refreshWall(info.diceValue,info.bankerIdx,info.leftMJCnt,info.initCardCnt);

        this.mLayerPlayers.refresh(this.mRoomData);
        
    }

    public void onPlayerSitDown(RoomPlayerData p )
    {
        // maybe self sitdown ;
        if ( p.nUID == ClientPlayerData.getInstance().getSelfUID() )
        {
            this.mLayerCard.selfIdx = p.idx ;
            this.mDeskTimer.selfIdx = p.idx ;
        }
        this.mLayerPlayers.onPlayerSitDown(p);
    }
    public void onMJActError( RoomPlayerData p )
    {
        this.mLayerCard.refreshPlayerCards( p.idx,p.vChuCards,p.vActedCards,p.vHoldCards,p.vHoldCards.Count ) ;
    }
    public void onPlayerNetStateChanged( int playerIdx , bool isOnline )
    {

    }

    public void onPlayerChatMsg( int playerIdx , eChatMsgType type , string strContent )
    {

    }
    public void onPlayerStandUp( int idx )
    {
        this.mLayerPlayers.onPlayerStandUp(idx);
    }

    public void onPlayerReady( int idx )
    {
        this.mLayerPlayers.onPlayerReady(idx);
    }
    public void onDistributedCards()
    {
        this.mLayerCard.onDistribute(this.mRoomData);
    }
    public void onPlayerActMo( int idx , int card )
    {
        this.mLayerCard.onMo(idx,card);
        this.mLayerInfo.leftMJCnt = this.mRoomData.mBaseData.leftMJCnt ;
        this.mDeskTimer.resetTime();
        this.mSeatOriention.setCurActIdx(idx) ;
    }
    public void onPlayerActChu( int idx , int card )
    {
        this.mLayerCard.onChu(idx,card);
    }
    public void showActOptsAboutOtherCard( JSONArray vActs )
    {
        this.mLayerDlg.showDlgAct(vActs);
    }
    public void onPlayerActChi( int idx , int card , int withA , int withB, int invokeIdx )
    {
        this.mLayerCard.onEat(idx,card,withA,withB ) ;
        this.mDeskTimer.resetTime();
        this.mSeatOriention.setCurActIdx(idx) ;
        this.mLayerPlayers.playActResultEffect(idx,eMJActType.eMJAct_Chi );
    }
    public void onPlayerActPeng( int idx , int Card, int invokeIdx )
    {
        this.mLayerCard.onPeng(idx,Card,invokeIdx) ;
        this.mDeskTimer.resetTime();
        this.mSeatOriention.setCurActIdx(idx) ;
        this.mLayerPlayers.playActResultEffect(idx,eMJActType.eMJAct_Peng );
    }
    public void onPlayerActMingGang( int idx , int Card, int invokeIdx, int newCard )
    {
        this.mLayerCard.onMingGang(idx,Card,invokeIdx,newCard ) ;
        this.mLayerInfo.leftMJCnt = this.mRoomData.mBaseData.leftMJCnt ;
        this.mDeskTimer.resetTime();
        this.mSeatOriention.setCurActIdx(idx) ;
        this.mLayerPlayers.playActResultEffect(idx,eMJActType.eMJAct_MingGang );
    }
    public void onPlayerActAnGang( int idx , int card , int NewCard )
    {
        this.mLayerCard.onAnGang(idx,card,NewCard ) ;
        this.mLayerInfo.leftMJCnt = this.mRoomData.mBaseData.leftMJCnt ;
        this.mDeskTimer.resetTime();
        this.mSeatOriention.setCurActIdx(idx) ;
        this.mLayerPlayers.playActResultEffect(idx,eMJActType.eMJAct_AnGang );
    }

    public void onPlayerActBuGang( int idx , int card , int NewCard )
    {
        this.mLayerCard.onBuGang( idx,card,NewCard );
        this.mLayerInfo.leftMJCnt = this.mRoomData.mBaseData.leftMJCnt ;
        this.mDeskTimer.resetTime();
        this.mSeatOriention.setCurActIdx(idx) ;
        this.mLayerPlayers.playActResultEffect(idx,eMJActType.eMJAct_BuGang );
    }
    public void onPlayerActHu( int idx, int card , int invokerIdx )
    {
        this.mLayerCard.onHu(idx,card,invokerIdx ) ;
        this.mLayerPlayers.playActResultEffect(idx,eMJActType.eMJAct_Hu );
    }
    public void showActOptsWhenRecivedCards( JSONArray vActs )
    {
        this.mLayerDlg.showDlgAct(vActs);
    }

    public void onGameStart()
    {
        this.mLayerPlayers.onStartGame();
        this.mLayerInfo.leftMJCnt = this.mRoomData.mBaseData.leftMJCnt ;
        this.mDeskTimer.resetTime();
        this.mSeatOriention.setCurActIdx(this.mRoomData.mBaseData.bankerIdx) ;
    }
    public void onGameEnd( ResultSingleData data )
    {
        this.mLayerDlg.showDlgResultSingle(data);

        foreach ( var item in  data.mResults )
        {
            if ( item == null || item.isEmpty() )
            {
                continue ;
            }
            this.mLayerCard.showHoldCards(item.mIdx,item.mAnHoldCards );
        }
    }
    public void onRoomOvered( JSONObject jsResult )
    {

    }
    public void onApplyDismisRoom( int idx )
    {
        this.mLayerDlg.showDlgDismiss(this.mRoomData) ;
    }
    public void onReplayDismissRoom( int idx , bool isAgree )
    {
        this.mLayerDlg.onReplayDismiss(idx,isAgree);
    }
    public void onRoomDoClosed()
    {

    }

    public void onRecivedPlayerBrifeData( PlayerInfoData infoData )
    {

    } 

    public void requestChu( int nCard )
    {
        Debug.LogWarning("let room data send msg to chu pai");
        this.mRoomData.onPlayerChosedAct(eMJActType.eMJAct_Chu,nCard ) ;
    }  

    public void showEatOpts( List<eEatType> vEatOpts , int ntargetCard )
    {
        this.mLayerDlg.showEatOpts(vEatOpts,ntargetCard);
    }
    public void showGangOpts( List<int> vGangOpts )
    {
        this.mLayerDlg.showGangOpts(vGangOpts);
    }
}
