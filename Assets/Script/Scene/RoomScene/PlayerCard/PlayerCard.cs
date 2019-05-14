using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerCard : MonoBehaviour
{
    public CardChu mChu = null ;
    public CarHoldMing mHoldMing = null ;
    public Transform mHoldAnTransform = null ;
    ICardHoldAn mHoldAn = null ;
    UnityAction<int> mReqChuPaiCallBack = null ;
    public void makeSelfPlayerCard( UnityAction<int> pReqChuCallBack )
    {
        CardHoldAnSelf self = this.mHoldAnTransform.gameObject.GetComponent<CardHoldAnSelf>();
        if ( self == null )
        {
            self = this.mHoldAnTransform.gameObject.AddComponent<CardHoldAnSelf>();
        }
        self.enabled = true ;
        self.mChuPaiCallBack = this.selfDoChu;
        this.mHoldAn = self ;
        this.mReqChuPaiCallBack = pReqChuCallBack ;
        self.mMJFactory = this.mChu.mMJFactory;
        
        var ot = this.mHoldAnTransform.GetComponent<CardHoldAnOther>();
        if ( null != ot )
        {
            ot.clear();
            ot.enabled = false ;
            ot.mMJFactory = null;
        }
    }

    public void makeOtherPlayerCard()
    {
        CardHoldAnOther other = this.mHoldAnTransform.gameObject.GetComponent<CardHoldAnOther>();
        if ( other == null )
        {
            other = this.mHoldAnTransform.gameObject.AddComponent<CardHoldAnOther>();
        }
        other.mMJFactory = this.mChu.mMJFactory;
        other.enabled = true ;
        this.mHoldAn = other ;

        this.mReqChuPaiCallBack = null ;
        var self = this.mHoldAnTransform.GetComponent<CardHoldAnSelf>();
        if ( self != null )
        {
            self.clear();
            self.enabled = false ;
            self.mChuPaiCallBack = null ;
            self.mMJFactory = null;
        }
    }

    public void makeReplayPlayerCard()
    {
        Debug.LogError("not implement replay hold an");
    }
    public void clear()
    {
        this.mChu.clear();
        this.mHoldMing.clear();
        this.mHoldAn.clear();
        this.adjustHoldTransformPos();
    }

    public void refresh( List<int> vChu, List<PlayerActedCard> vMingHold , List<int> vHoldAn, int holdCnt )
    {
        this.mChu.refresh(vChu) ;
        this.mHoldAn.refresh(vHoldAn,holdCnt) ;
        this.mHoldMing.refresh(vMingHold) ;
        this.adjustHoldTransformPos();
    }
    void adjustHoldTransformPos()
    {
        var pos = this.mHoldAnTransform.localPosition ;
        pos.x = this.mHoldMing.transform.localPosition.x + this.mHoldMing.getHoldMingSize();
        this.mHoldAnTransform.localPosition = pos ;
    }
    public void onDistribute( List<int> vCards , int nCnt )
    {
        this.mHoldAn.onDistribute(vCards,nCnt);
    }
    public void onMo( int nCard , Vector3 ptWorldPosFromWall )
    {
        this.mHoldAn.onMo(nCard,ptWorldPosFromWall) ;
        this.mHoldAn.onWaitChu();
    }

    public void selfDoChu( int nCard , Vector3 ptHoldWorldPos )
    {
        this.mChu.addChu(nCard,ptHoldWorldPos) ;
        if ( this.mReqChuPaiCallBack != null )
        {
            this.mReqChuPaiCallBack(nCard);
        }
    }

    public void waitChu()
    {
        this.mHoldAn.onWaitChu();
    }
    public Vector3 onChu( int nCard )
    {
        var ptworPos = this.mHoldAn.doChu(nCard);
        return this.mChu.addChu(nCard,ptworPos) ;
    }

    public void chuCardBeRemoved( int nCard )
    {
        this.mChu.removeLastChu(nCard);
    }

    public void onPeng( int nCard , eArrowDirect eDir )
    {
        this.mHoldAn.removeCard(nCard,2);
        this.mHoldMing.addPeng(nCard,eDir) ;
        this.adjustHoldTransformPos() ;
        this.mHoldAn.onWaitChu();
    }

    public void onMingGang( int nCard , eArrowDirect eDir, int newCard, Vector3 newCardWallPos )
    {
        this.mHoldAn.removeCard(nCard,3);
        this.mHoldMing.addMingGang(nCard,eDir) ;
        this.onMo(newCard,newCardWallPos) ;
        this.adjustHoldTransformPos() ;
    }

    public void onAnGang( int nCard,int newCard, Vector3 newCardWallPos )
    {
        this.mHoldAn.removeCard(nCard,4);
        this.mHoldMing.addAnGang(nCard) ;
        this.onMo(newCard,newCardWallPos) ;
        this.adjustHoldTransformPos();
    }

    public void onBuGang( int nCard,int newCard, Vector3 newCardWallPos )
    {
        this.mHoldAn.removeCard(nCard,1);
        this.mHoldMing.addBuGang(nCard) ;
        this.onMo(newCard,newCardWallPos) ;
        this.adjustHoldTransformPos();
    }

    public void onHu( int nCard )
    {
        this.mHoldAn.onHu(nCard);
    }

    public void onEat( int targetCard , int withA , int withB )
    {
        this.mHoldAn.removeCard(withA,1);
        this.mHoldAn.removeCard(withB,1);
        this.mHoldMing.addEat(withA,withB,targetCard ); 
        this.mHoldAn.onWaitChu();
    }

    public void showCards( List<int> holdCards )
    {
        this.mHoldAn.showCards(holdCards);
    }

    void Start()
    {
        
    }
}
