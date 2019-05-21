using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerWaitReady : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> mSeatButtons ;
    public List<GameObject> mReadyIcons ;
    
    List<Vector3> mInitPosForButtons = new List<Vector3>();
    List<Vector3> mInitPosForReadyIcons = new List<Vector3>();
    public RoomScene mScene ;
    void Start()
    {
        if ( this.mInitPosForButtons.Count == 0 )
        {
            for (int i = 0; i < 4; i++)
            {
                this.mInitPosForButtons.Add(this.mSeatButtons[i].transform.localPosition);
                this.mInitPosForReadyIcons.Add(this.mReadyIcons[i].transform.localPosition );
            }
        }        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void onPlayerReady( int idx )
    {
        this.mReadyIcons[idx].SetActive(true);
    }

    public void refresh( RoomData data )
    {
        var selfIdx = data.getSelfIdx();
        if ( selfIdx >= 0 ) // add just pos ;
        {
            for ( int i = 0; i < 4; i++ )
            {
                var idx = ( selfIdx + i ) % 4 ;
                this.mReadyIcons[idx].transform.localPosition = this.mInitPosForReadyIcons[i];
                this.mSeatButtons[idx].transform.localPosition = this.mInitPosForButtons[i] ;
            }
        }

        var players = data.mPlayers ;
        for ( int i = 0 ; i < 4 ; ++i )
        {
            this.mReadyIcons[i].SetActive( players[i] != null && players[i].isEmpty() == false && players[i].isReady );
            this.mSeatButtons[i].SetActive( selfIdx < 0 && ( players[i] == null || players[i].isEmpty()) );
        }
    }

    public void hide()
    {
        for ( int i = 0 ; i < 4 ; ++i )
        {
            this.mReadyIcons[i].SetActive(false);
            this.mSeatButtons[i].SetActive(false);
        }
    }
}
