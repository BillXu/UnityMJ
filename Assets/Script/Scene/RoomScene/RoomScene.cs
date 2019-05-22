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
    private void Awake() {
        mRoomData.mSceneDelegate = this ;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onRecivedRoomInfo( RoomBaseData info )
    {
        this.mLayerCard.clear();
        this.mLayerInfo.refresh(this.mRoomData.mBaseData);
    }

    public void onRecivedAllPlayers( List<RoomPlayerData> vPlayers )
    {
        var info = this.mRoomData.mBaseData ;
        var selfIdx = this.mRoomData.getSelfIdx() ;

        this.mLayerCard.selfIdx = selfIdx < 0 ? 0 : selfIdx;
        this.mLayerCard.refreshWall(info.diceValue,info.bankerIdx,info.leftMJCnt,info.initCardCnt);

        this.mLayerPlayers.refresh(this.mRoomData);
    }

    public void onPlayerSitDown(RoomPlayerData p )
    {
        // maybe self sitdown ;
        if ( p.nUID == ClientPlayerData.getInstance().getSelfUID() )
        {
            this.mLayerCard.selfIdx = p.idx ;
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
    }
    public void onPlayerActChu( int idx , int card )
    {
        this.mLayerCard.onChu(idx,card);
    }
    public void showActOptsAboutOtherCard( JSONArray vActs )
    {

    }
    public void onPlayerActChi( int idx , int card , int withA , int withB, int invokeIdx )
    {
        this.mLayerCard.onEat(idx,card,withA,withB ) ;
    }
    public void onPlayerActPeng( int idx , int Card, int invokeIdx )
    {
        this.mLayerCard.onPeng(idx,Card,invokeIdx) ;
    }
    public void onPlayerActMingGang( int idx , int Card, int invokeIdx, int newCard )
    {
        this.mLayerCard.onMingGang(idx,Card,invokeIdx,newCard ) ;
    }
    public void onPlayerActAnGang( int idx , int card , int NewCard )
    {
        this.mLayerCard.onAnGang(idx,card,NewCard ) ;
    }

    public void onPlayerActBuGang( int idx , int card , int NewCard )
    {
        this.mLayerCard.onBuGang( idx,card,NewCard );
    }
    public void onPlayerActHu( int idx, int card , int invokerIdx )
    {
        this.mLayerCard.onHu(idx,card,invokerIdx ) ;
    }
    public void showActOptsWhenRecivedCards( JSONArray vActs )
    {

    }

    public void onGameStart()
    {
        this.mLayerPlayers.onStartGame();
    }
    public void onGameEnd( JSONObject jsResult )
    {

    }
    public void onRoomOvered( JSONObject jsResult )
    {

    }
    public void onApplyDismisRoom( int idx )
    {

    }
    public void onReplayDismissRoom( int idx , bool isAgree )
    {

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
        this.mRoomData.onPlayerChoseActAboutRecievedCard(eMJActType.eMJAct_Chu,nCard ) ;
    }  

    public void showEatOpts( List<eEatType> vEatOpts )
    {

    }
    public void showGangOpts( List<int> vGangOpts )
    {

    }

    public int svrIdxToClientIdx( int svrIdx ) // used by UI layer;
    {
        int selfIdx = this.mRoomData.getSelfIdx();
        if ( selfIdx < 0 )
        {
            selfIdx = 0 ;
        }

        return (int)Mathf.Abs( svrIdx - selfIdx ) ;
    }

    public int clientIdxToSvrIdx( int clientIdx ) // used by UI layer;
    {
        int selfIdx = this.mRoomData.getSelfIdx();
        if ( selfIdx < 0 )
        {
            selfIdx = 0 ;
        }

        return ( clientIdx + selfIdx ) % 4 ;
    }
}
