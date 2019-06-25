using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerPlayers : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> mSeatButtons ;
    public List<GameObject> mReadyIcons ;
    public List<RoomPlayerItem> mPlayers ;
    public List<EffectPlayerActResult> mEffectActResults;
    public Transform mBankIcon = null ;
    public PlayerInteractEmoji mInteractEmoji = null ;
    
    List<Vector3> mInitPosForButtons = new List<Vector3>();
    List<Vector3> mInitPosForReadyIcons = new List<Vector3>();
    List<Vector3> mInitPlayerPos = new List<Vector3>() ;
    List<Vector3> mInitEffectPos = new List<Vector3>() ;
    Vector3 mBankIconLocalPosPerPlayer = new Vector3(-24.7f,33.3f,0);
    List<Vector3> mBankIconPosInUse = new List<Vector3>() ;
    public RoomScene mScene ;

    public int bankIdx
    {
        set
        {
            if ( value < 0 || value >= 4 )
            {
                this.mBankIcon.gameObject.SetActive(false);
                return ;
            }
            this.mBankIcon.gameObject.SetActive(true);
            this.mBankIcon.position = this.mBankIconPosInUse[value] ;
        }
    }
    void Start()
    {
        if ( this.mInitPosForButtons.Count == 0 )
        {
            for (int i = 0; i < 4; i++)
            {
                this.mInitPosForButtons.Add(this.mSeatButtons[i].transform.localPosition);
                this.mInitPosForReadyIcons.Add(this.mReadyIcons[i].transform.localPosition );
                this.mInitPlayerPos.Add( this.mPlayers[i].transform.localPosition );
                this.mInitEffectPos.Add( this.mEffectActResults[i].transform.localPosition );
                this.mBankIconPosInUse.Add( this.mPlayers[i].transform.TransformPoint(this.mBankIconLocalPosPerPlayer) ) ;
                
                var p = new PlayerInfoItem.ClickInfoItemEvent();
                p.AddListener(this.onClickPlayerPhoto);
                this.mPlayers[i].setClickCallBack(p);
            }
        }   
    }

    public void onBtnSeat_0()
    {
        this.onClickSeat(0);
    }

    public void onBtnSeat_1()
    {
        this.onClickSeat(1);
    }

    public void onBtnSeat_2()
    {
        this.onClickSeat(2);
    }

    public void onBtnSeat_3()
    {
        this.onClickSeat(3);
    }

    void onClickSeat( int idx )
    {
        this.mReadyIcons[idx].SetActive(true);
        this.mSeatButtons[idx].SetActive(false);
        this.mScene.mRoomData.onPlayerClickedSitDown( idx );
    }

    public void playActResultEffect( int idx , eMJActType act )
    {
        this.mEffectActResults[idx].playActEffect(act) ;
    }
    public void onPlayerReady( int idx )
    {
        this.mReadyIcons[idx].SetActive(true);
    }

    public void onPlayerSitDown( RoomPlayerData dt )
    {
        var data = this.mScene.mRoomData;
        this.mReadyIcons[dt.idx].SetActive( dt.isReady && data.mBaseData.isWaitReadyState() );
        this.mSeatButtons[dt.idx].SetActive( false );
        this.mPlayers[dt.idx].gameObject.SetActive( true );
        this.mPlayers[dt.idx].refresh(dt.nUID,dt.nChips);

        if ( dt.isSelf )
        {
            var selfIdx = dt.idx;
            var players = data.mPlayers ;
            for ( int i = 0 ; i < 4 ; ++i )
            {
                var p = players[i];
                var isnullp = p == null || p.isEmpty();
                this.mSeatButtons[i].SetActive( selfIdx < 0 && isnullp );
            }
            adjustPos(selfIdx);
        }
    }

    public void onPlayerStandUp( int idx )
    {
        this.mReadyIcons[idx].SetActive( false );
        this.mPlayers[idx].gameObject.SetActive( false );

        var data = this.mScene.mRoomData;
        var selfIdx = data.getSelfIdx();

        var players = data.mPlayers ;
        for ( int i = 0 ; i < 4 ; ++i )
        {
            var p = players[i];
            var isnullp = p == null || p.isEmpty();
            this.mSeatButtons[i].SetActive( selfIdx < 0 && isnullp );
        }
    }
    public void refresh( RoomData data )
    {
        var selfIdx = data.getSelfIdx();
        this.adjustPos( selfIdx < 0 ? 0 : selfIdx );

        var players = data.mPlayers ;
        for ( int i = 0 ; i < 4 ; ++i )
        {
            var p = players[i];
            var isnullp = p == null || p.isEmpty();

            this.mReadyIcons[i].SetActive( data.mBaseData.isWaitReadyState() && isnullp == false && p.isReady );
            this.mSeatButtons[i].SetActive( selfIdx < 0 && isnullp );
            
            this.mPlayers[i].gameObject.SetActive( isnullp == false );
            if ( false == isnullp )
            {
                this.mPlayers[i].refresh(p.nUID,p.nChips) ;
            }
        }
        this.bankIdx = data.mBaseData.bankerIdx ;
    }

    void adjustPos( int selfIdx )
    {
        for ( int i = 0; i < 4; i++ )
        {
            var idx = ( selfIdx + i ) % 4 ;
            this.mReadyIcons[idx].transform.localPosition = this.mInitPosForReadyIcons[i];
            this.mSeatButtons[idx].transform.localPosition = this.mInitPosForButtons[i] ;
            this.mPlayers[idx].transform.localPosition = this.mInitPlayerPos[i];
            this.mEffectActResults[idx].transform.localPosition = this.mInitEffectPos[i] ;
            this.mBankIconPosInUse[idx] = this.mPlayers[idx].transform.TransformPoint(this.mBankIconLocalPosPerPlayer) ;
        }
    }
    public void onStartGame()
    {
        for ( int i = 0 ; i < 4 ; ++i )
        {
            this.mReadyIcons[i].SetActive( false );
        }
        this.bankIdx = this.mScene.mRoomData.mBaseData.bankerIdx ;
    }

    public void onPlayerChatMsg( int playerIdx , eChatMsgType type , string strContent )
    {
        this.mPlayers[playerIdx].onPlayerChatMsg(type,strContent);
    }

    public void onClickPlayerPhoto( int nPlayerID )
    {
        this.mScene.showDlgPlayerInfo(nPlayerID);
    }

    public void onShowInteractEmoji( int invokeIdx , int targetIdx , int emojiIdx )
    {
        this.mInteractEmoji.playInteractEmoji(this.mPlayers[invokeIdx].transform.position,this.mPlayers[targetIdx].transform.position,emojiIdx) ;
    }

    // public 
    public int eidx = 5 ;
    public void test()
    {
        this.onShowInteractEmoji(1,0,eidx);
    }
}
