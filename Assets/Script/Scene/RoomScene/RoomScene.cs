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

    }

    public void onPlayerSitDown(RoomPlayerData p )
    {

    }
    public void onRecivedPlayerCards( RoomPlayerData p )
    {

    }
    public void onPlayerNetStateChanged( int playerIdx , bool isOnline )
    {

    }

    public void onPlayerChatMsg( int playerIdx , eChatMsgType type , string strContent )
    {

    }
    public void onPlayerStandUp( int idx )
    {

    }
    public void onDistributedCards()
    {

    }
    public void onPlayerActMo( int idx , int card )
    {

    }
    public void onPlayerActChu( int idx , int card )
    {

    }
    public void showActOptsAboutOtherCard( JSONArray vActs )
    {

    }
    public void onPlayerActChi( int idx , int card , int withA , int withB, int invokeIdx )
    {

    }
    public void onPlayerActPeng( int idx , int Card, int invokeIdx )
    {

    }
    public void onPlayerActMingGang( int idx , int Card, int invokeIdx, int newCard )
    {

    }
    public void onPlayerActAnGang( int idx , int card , int NewCard )
    {

    }

    public void onPlayerActBuGang( int idx , int card , int NewCard )
    {

    }
    public void onPlayerActHu( int idx, int card , int invokerIdx )
    {

    }
    public void showActOptsWhenRecivedCards( JSONArray vActs )
    {

    }

    public void onGameStart()
    {

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
    }  
}
