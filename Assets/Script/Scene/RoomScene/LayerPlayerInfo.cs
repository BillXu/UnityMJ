using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerPlayerInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public List<RoomPlayerItem> mPlayers ;
    List<Vector3> mInitPlayerPos = new List<Vector3>() ;
    void Start()
    {
        if ( this.mInitPlayerPos.Count == 0 )
        {
            for ( int i = 0; i < this.mPlayers.Count; i++ )
            {
                this.mInitPlayerPos.Add( this.mPlayers[i].transform.localPosition );
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onPlayerSitDown( RoomPlayerData dt )
    {
        if ( dt.isSelf )
        {
            this.adjustPosBySelfIdx(dt.idx);
        }
        this.mPlayers[dt.idx].refresh(dt.nUID,dt.nChips) ;
    }

    public void onPlayerStandUp( int idx )
    {
        this.mPlayers[idx].gameObject.SetActive(false);
    }

    public void refresh( RoomData dt )
    {
        var selfIdx = dt.getSelfIdx();
        if ( selfIdx < 0 )
        {
            selfIdx = 0 ;
        }

        this.adjustPosBySelfIdx(selfIdx);

        for (int i = 0; i < dt.mPlayers.Count; i++)
        {
            var p = dt.mPlayers[i] ;
            if ( p == null || p.isEmpty() )
            {
                this.mPlayers[i].gameObject.SetActive(false);
                continue;
            }
            this.mPlayers[i].gameObject.SetActive(true);
            this.mPlayers[i].refresh(p.nUID,p.nChips) ;
        }
    }

    void adjustPosBySelfIdx( int selfIdx )
    {
        for ( int i = 0; i < 4; i++ )
        {
            var idx = ( selfIdx + i ) % 4 ;
            this.mPlayers[i].transform.localPosition = this.mInitPlayerPos[i];
        }
    }
}
