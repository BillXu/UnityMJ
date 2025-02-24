﻿using System.Collections;
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
    public void makeSelfPlayerCard( UnityAction<int> pReqChuCallBack, int selfIdx )
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

        //var roateX = GameObject.Find("Main Camera").transform.localRotation.eulerAngles.x - 4 ;
        self.transform.localEulerAngles = new Vector3(42,0,0) ;
        var pos = self.transform.localPosition;
        switch ( selfIdx )
        {
            case 0 :
            {
                pos.y = 76.9f;
                pos.z = -86f;
            }
            break;
            case 1:
            {
                pos.y = 74.7f;
                pos.z = -87.3f;
            }
            break;
            case 2 :
            {
                pos.y = 75.4f;
                pos.z = -86.9f;
            }
            break ;
            case 3 :
            {
                pos.y = 75.2f;
                pos.z = -86.9f;
            }
            break ;
        }
        self.transform.localPosition = pos;

        var posMing = this.mHoldMing.transform.localPosition;
        posMing.z = pos.z;
        posMing.y = pos.y;
        this.mHoldMing.transform.localPosition = posMing;
    }

    public void makeOtherPlayerCard( eArrowDirect ePosDir , int selfIdx )
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

        other.transform.localEulerAngles = Vector3.zero ;
        var pos = other.transform.localPosition;
        pos.y = 1.4f;
        switch ( selfIdx )
        {
            case 0 :
            {
                switch ( ePosDir )
                {
                    case eArrowDirect.eDirect_Left:
                    case eArrowDirect.eDirect_Righ:
                    {
                        pos.z = -38f;
                    }
                    break;
                    case eArrowDirect.eDirect_Opposite:
                    {
                        pos.z = -34.8f;
                    }
                    break ;
                }
                
            }
            break;
            case 1:
            {
                switch ( ePosDir )
                {
                    case eArrowDirect.eDirect_Left:
                    {
                        pos.z = -35.7f;
                    }
                    break;
                    case eArrowDirect.eDirect_Opposite:
                    {
                        pos.z = -35.5f;
                    }
                    break ;
                    case eArrowDirect.eDirect_Righ:
                    {
                        pos.z = -36.9f;
                    }
                    break ;
                }
            }
            break;
            case 2 :
            {
                switch ( ePosDir )
                {
                    case eArrowDirect.eDirect_Righ:
                    case eArrowDirect.eDirect_Left:
                    {
                        pos.z = -38.1f;
                    }
                    break;
                    case eArrowDirect.eDirect_Opposite:
                    {
                        pos.z = -33.4f;
                    }
                    break ;
                }
            }
            break ;
            case 3 :
            {
                switch ( ePosDir )
                {
                    case eArrowDirect.eDirect_Opposite:
                    {
                        pos.z = -35.5f;
                    }
                    break ;
                    case eArrowDirect.eDirect_Left:
                    case eArrowDirect.eDirect_Righ:
                    {
                        pos.z = -36.7f;
                    }
                    break ;
                }
            }
            break ;
        }
        other.transform.localPosition = pos;
        
        var posMing = this.mHoldMing.transform.localPosition;
        posMing.z = pos.z;
        posMing.y = pos.y ;
        this.mHoldMing.transform.localPosition = posMing;
    }

    public void makeReplayPlayerCard()
    {
        Debug.LogError("not implement replay hold an");
    }
    public void clear()
    {
        this.mChu.clear();
        this.mHoldMing.clear();
        if ( this.mHoldAn != null )
        {
            this.mHoldAn.clear();
        }
        this.adjustHoldTransformPos();
    }

    public void refresh( List<int> vChu, List<PlayerActedCard> vMingHold , List<int> vHoldAn, int holdCnt )
    {
        this.mChu.refresh(vChu) ;
        this.mHoldAn.refresh(vHoldAn,holdCnt) ;
        this.mHoldMing.refresh(vMingHold) ;
        this.adjustHoldTransformPos();
        if ( holdCnt % 3 == 2 )
        {
            this.mHoldAn.onWaitChu();
        }
    }
    void adjustHoldTransformPos()
    {
        var tSize = this.mHoldMing.getHoldMingSize() + (this.mHoldAn != null ? this.mHoldAn.getHoldAnXSize() : 0) ;
        var pos = this.mHoldMing.transform.localPosition ;
        pos.x = -tSize * 0.5f ;
        Debug.Log("adjustHoldTransformPos size = " + tSize );
        this.mHoldMing.transform.localPosition = pos;
        var posAn = this.mHoldAnTransform.localPosition;
        posAn.x = pos.x + this.mHoldMing.getHoldMingSize();
        this.mHoldAnTransform.localPosition = posAn ;
    }
    public void onDistribute( List<int> vCards , int nCnt )
    {
        this.mHoldAn.onDistribute(vCards,nCnt);
        adjustHoldTransformPos();
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
        this.adjustHoldTransformPos();
    }

    public void showCards( List<int> holdCards )
    {
        this.mHoldAn.showCards(holdCards);
    }

    void Start()
    {
        
    }
}
