using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening ;
public class CardHoldAnOther : MonoBehaviour,ICardHoldAn
{
    public MJFactory mMJFactory = null;
    public bool isShowingUnknown = true ;
    public MJCard mHuCard = null ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clear()
    {
        while ( this.transform.childCount > 0 )
        {
            var child = this.transform.GetChild(0).GetComponent<MJCard>();
            if ( child == null )
            {
                this.transform.GetChild(0).gameObject.SetActive(false);
                this.transform.GetChild(0).SetParent(null);
                continue;
            }

            if ( this.isShowingUnknown &&  ( null == this.mHuCard || child.transform != this.mHuCard.transform ) )
            {
                this.mMJFactory.recycleUnknownCard(child) ;
            }
            else
            {
                this.mMJFactory.recycleCard(child) ;
            }
        }

        this.isShowingUnknown = true ;
        this.mHuCard = null ;
    }
    public void refresh( List<int> vCards , int nCnt )
    {
        this.clear();
        for ( int i = 0 ; i < nCnt ; ++i )
        {
            var v = this.mMJFactory.getUnknownCard(this.transform) ;
            v.curState = MJCard.state.FACE_USER ;
            v.transform.localPosition = new Vector3( i * v.world_x_Size,0,0) ;
        }
    }
    public void onMo( int nCard, Vector3 ptWallCardWorldPos )
    {
        var newCard = this.mMJFactory.getUnknownCard(this.transform) ;
        newCard.curState = MJCard.state.FACE_USER ;
        newCard.transform.position = ptWallCardWorldPos ;
        var targetPos = new Vector3((this.transform.childCount - 1 )* newCard.world_x_Size + newCard.world_x_Size * 0.3f,0,0 );
        newCard.transform.DOLocalMove(targetPos,0.3f) ;
    }

    public void onHu( int nCard )
    {
        var newCard = this.mMJFactory.getCard(nCard,this.transform) ;
        newCard.curState = MJCard.state.FACE_UP ;
        newCard.transform.position = new Vector3((this.transform.childCount - 1 )* newCard.world_x_Size + newCard.world_x_Size * 0.3f,0,0 );
        this.mHuCard = newCard ;
    }
    public void onDistribute( List<int> vCards , int nCnt )
    {
        for ( int i = 0 ; i < nCnt ; ++i )
        {
            var v = this.mMJFactory.getUnknownCard(this.transform) ;
            v.curState = MJCard.state.FACE_USER ;
            v.transform.localPosition = new Vector3( (this.transform.childCount - 1) * v.world_x_Size,0,0) ;
        }
    }
    public Vector3 doChu( int nCard )
    {
        var v = this.transform.GetChild(this.transform.childCount - 1).GetComponent<MJCard>();
        var p = v.transform.position ;
        this.mMJFactory.recycleUnknownCard(v);
        return p ;
    }

    public void removeCard( int nCard, int cnt )
    {
        while ( cnt-- > 0 )
        {
            var v = this.transform.GetChild(this.transform.childCount - 1).GetComponent<MJCard>();
            this.mMJFactory.recycleUnknownCard(v);
        }
    }
    public void onWaitChu()
    {

    }
    public void rearrangeCard()
    {

    }
    public void showCards( List<int> vCards ) // when game end do shou cards ;
    {   
        int nHuCard = 0 ;
        if ( this.mHuCard != null )
        {
            nHuCard = this.mHuCard.cardNum ;
        }

        this.clear();
        this.isShowingUnknown = false ;
        vCards.Sort();
        for ( int i = 0 ; i < vCards.Count ; ++i )
        {
            var v = this.mMJFactory.getCard(vCards[i],this.transform) ;
            v.curState = MJCard.state.FACE_UP ;
            v.transform.localPosition = new Vector3( i * v.world_x_Size,0,0) ;
        }   
        
        onHu(nHuCard); 
    }

    public float getHoldAnXSize()
    {
        if ( this.transform.childCount == 0 )
        {
            return 0 ;
        }
        
        var last = this.transform.GetChild(this.transform.childCount - 1 );
        return last.localPosition.x + last.GetComponent<MJCard>().world_x_Size ;
    }
}
