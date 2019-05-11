using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening ;
public class LayerCards : MonoBehaviour
{
    public List<CardWall> mWalls ;
    public List<PlayerCard> mPlayerCards ;
    public RoomScene mScene ;
    bool isRemoveHuCardFromDianPao = false ;
    int mCurWallIdx = 0 ;
    public int selfIdx
    {
        set
        {
            for ( int idx = 0 ; idx < 4 ; ++idx )
            {
                if ( idx == value )
                {
                    this.mPlayerCards[idx].makeSelfPlayerCard( this.mScene.requestChu ) ;
                }
                else
                {
                    this.mPlayerCards[idx].makeOtherPlayerCard();
                }
            }
        }
    }

    public bool isReplay
    {
        set
        {
            for ( int idx = 0 ; idx < 4 ; ++idx )
            {
                this.mPlayerCards[idx].makeReplayPlayerCard();
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clear()
    {
        foreach (var item in this.mWalls )
        {
            item.clear();
        }

        foreach (var itemc in this.mPlayerCards )
        {
            itemc.clear();
        }
        this.isRemoveHuCardFromDianPao = false ;
    }

    public void shuffle( int mjCnt )
    {
        foreach ( var item in this.mWalls )
        {
            item.refresh(mjCnt/4,mjCnt/4,0);
        }

        var wallParendNode = this.mWalls[0].transform.parent;
        var pos = wallParendNode.localPosition ;
        pos.y -= this.mWalls[0].wallHeight ;
        wallParendNode.localPosition = pos ;
        wallParendNode.DOLocalMoveY(pos.y + this.mWalls[0].wallHeight,1 ) ;
    }

    public void refresh( RoomData data )
    {
        this.selfIdx = data.getSelfIdx();
        this.clear();
        this.refreshWall(data.mBaseData.diceValue,data.mBaseData.bankerIdx,data.mBaseData.leftMJCnt,data.mBaseData.initCardCnt);
        foreach (var item in data.mPlayers )
        {
            if ( item == null || item.isEmpty() )
            {
                continue ;
            }
            this.refreshPlayerCards(item.idx,item.vChuCards,item.vActedCards,item.vHoldCards,item.vHoldCards.Count );
        }
    }
    void refreshWall( int nDiceValue , int bankerIdx , int nLeftMJCnt, int mjCntPerWall )
    {
        int startWallIdx = bankerIdx + nDiceValue - 1 ;
        startWallIdx = startWallIdx % 4 ;
        int startWallLeftFront = ( nDiceValue % 6 + 1 ) * 2 ;
        
        int notEmtpyWallCnt = ( nLeftMJCnt + mjCntPerWall - 1 ) / mjCntPerWall ;
        this.mCurWallIdx = ( startWallIdx - notEmtpyWallCnt + 4 ) % 4;
        int curWallLeftCnt = nLeftMJCnt % mjCntPerWall ;

        for ( int idx = startWallIdx ; idx < (startWallIdx + 4) ; ++idx )
        {
            int frontCnt = ( startWallIdx == idx % 4) ? startWallLeftFront : 0 ;
            int nBackCnt = 0 ;
            if ( idx > (this.mCurWallIdx + 4) )
            {
                nBackCnt = mjCntPerWall ;
            }
            else if ( idx == (this.mCurWallIdx + 4) )
            {
                nBackCnt = curWallLeftCnt ;
            }
            else
            {
                nBackCnt = 0 ;
            }

            this.mWalls[idx%4].refresh(mjCntPerWall,frontCnt,nBackCnt);
        }
    }

    void refreshPlayerCards( int playerIdx , List<int> vChu , List<PlayerActedCard> vMing , List<int> vHoldAn, int holdAnCnt )
    {
        this.mPlayerCards[playerIdx].refresh(vChu,vMing,vHoldAn,holdAnCnt);
    }

    public void onDistribute( RoomData data )
    {
        int startWallIdx = data.mBaseData.bankerIdx + data.mBaseData.diceValue - 1 ;
        startWallIdx = startWallIdx % 4 ;
        int startWallLeftFront = ( data.mBaseData.diceValue % 6 + 1 ) * 2 ;
        this.mCurWallIdx = startWallIdx;
        this.mWalls[this.mCurWallIdx].mFrontWallCnt = startWallLeftFront ;

        foreach (var item in data.mPlayers )
        {
            if ( item == null || item.isEmpty() )
            {
                continue ;
            }
            this.mPlayerCards[item.idx].onDistribute(item.vHoldCards,item.vHoldCards.Count);
            // decrease wall
            for ( int i = 0 ; i < item.vHoldCards.Count ; ++i )
            {
                this.featchCardFromWall();
            }
        }
    }

    public void waitPlayerChu( int playerIdx )
    {
        this.mPlayerCards[playerIdx].waitChu();
    }

    public void onMo( int playerIdx , int card )
    {
        this.mPlayerCards[playerIdx].onMo(card,this.featchCardFromWall()) ;
    }

    public void onChu( int playerIdx , int card )
    {
        var pt = this.mPlayerCards[playerIdx].onChu(card);
        // show arrow pos ;
    }   

    public void onPeng( int playerIdx , int card , int invokerIdx )
    {
        this.mPlayerCards[playerIdx].onPeng(card,this.getDirection(playerIdx,invokerIdx) );
        this.mPlayerCards[invokerIdx].chuCardBeRemoved(card);
        // hide arrow ;
    }

    public void onMingGang( int playerIdx , int card , int invokerIdx , int NewCard )
    {
        this.mPlayerCards[playerIdx].onMingGang( card,this.getDirection(playerIdx,invokerIdx),NewCard,this.featchCardFromWall() );
        this.mPlayerCards[invokerIdx].chuCardBeRemoved(card);
        // hide arrow ;
    }

    public void onBuGang( int playerIdx , int card , int NewCard )
    {
        this.mPlayerCards[playerIdx].onBuGang( card,NewCard,this.featchCardFromWall() );
    } 

    public void onAnGang( int playerIdx , int card , int NewCard )
    {
        this.mPlayerCards[playerIdx].onAnGang(card,NewCard,this.featchCardFromWall() );
    }

    public void onHu( int playerIdx , int card , int invokerIdx )
    {
        this.mPlayerCards[playerIdx].onHu(card);
        if ( playerIdx != invokerIdx && this.isRemoveHuCardFromDianPao == false )
        {
            this.mPlayerCards[invokerIdx].chuCardBeRemoved(card);
            this.isRemoveHuCardFromDianPao = true ;
        }
    }

    public void onEat( int playerIdx , int targetCard , int withA , int withB )
    {
        this.mPlayerCards[playerIdx].onEat(targetCard,withA,withB) ;
    }

    public void showHoldCards( int playerIdx , List<int> vCards )
    {
        this.mPlayerCards[playerIdx].showCards(vCards);
    }

    Vector3 featchCardFromWall()
    {
        if ( this.mWalls[this.mCurWallIdx].getWallLeftCnt() > 0 )
        {
            return this.mWalls[this.mCurWallIdx].onFetchCardFromWall();
        }

        this.mCurWallIdx = ( this.mCurWallIdx + 1 ) % 4 ;
        return this.mWalls[this.mCurWallIdx].onFetchCardFromWall(); ;
    }

    eArrowDirect getDirection( int actIdx , int invokerIdx )
    {
        if ( actIdx ==  ( invokerIdx + 1 ) % 4 )
        {
            return eArrowDirect.eDirect_Righ ;
        }

        if ( actIdx ==  ( invokerIdx + 2 ) % 4 )
        {
            return eArrowDirect.eDirect_Opposite ;
        }
        return eArrowDirect.eDirect_Left ;
    }

}
