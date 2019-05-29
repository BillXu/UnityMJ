using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public interface IRoomDataDelegate
{
    void onRecivedRoomInfo( RoomBaseData info );
    void onPlayerSitDown(RoomPlayerData p );
    void onRecivedAllPlayers(List<RoomPlayerData> vPlayers ) ;
    void onMJActError( RoomPlayerData p );
    void onPlayerNetStateChanged( int playerIdx , bool isOnline );
    void onPlayerChatMsg( int playerIdx , eChatMsgType type , string strContent );
    void onPlayerStandUp( int idx );
    void onPlayerReady( int idx );
    void onDistributedCards();
    void onPlayerActMo( int idx , int card );
    void onPlayerActChu( int idx , int card );
    void showActOptsAboutOtherCard( JSONArray vActs );
    void onPlayerActChi( int idx , int card , int withA , int withB, int invokeIdx );
    void onPlayerActPeng( int idx , int Card, int invokeIdx );
    void onPlayerActMingGang( int idx , int Card, int invokeIdx, int newCard );
    void onPlayerActAnGang( int idx , int card , int NewCard );
    void onPlayerActBuGang( int idx , int card , int NewCard );
    void onPlayerActHu( int idx, int card , int invokerIdx );
    void showActOptsWhenRecivedCards( JSONArray vActs );

    void onGameStart();
    void onGameEnd( ResultSingleData data );
    void onRoomOvered( ResultTotalData data );
    void onApplyDismisRoom( int idx );
    void onReplayDismissRoom( int idx , bool isAgree );
    void onRoomDoClosed();

    void onRecivedPlayerBrifeData( PlayerInfoData infoData );
    void showEatOpts( List<eEatType> vEatOpts , int ntargetCard );
    void showGangOpts( List<int> vGangOpts );
}
