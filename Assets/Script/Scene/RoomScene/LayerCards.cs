using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening ;
public class LayerCards : MonoBehaviour
{
    public List<CardWall> mWalls ;
    public List<PlayerCard> mPlayerCards ;
    public CardChuIndicator mChuIndictor ;
    public RoomScene mScene ;
    bool isRemoveHuCardFromDianPao = false ;
    int mCurWallIdx = 0 ;
    public Transform mCameraParent ; 
    public int selfIdx
    {
        set
        {
            this.clear();
            for ( int idx = 0 ; idx < 4 ; ++idx )
            {
                if ( idx == value )
                {
                    Debug.Log(" self = idx = value " + idx + " v" + value );
                    this.mPlayerCards[idx].makeSelfPlayerCard( this.mScene.requestChu ) ;
                }
                else
                {
                    var ePosDir = getDirection(value,idx) ;
                    this.mPlayerCards[idx].makeOtherPlayerCard(ePosDir);
                }
            }

            this.mCameraParent.transform.DOLocalRotate( new Vector3(0,value * -90,0),0.3f);
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
        this.selfIdx = (int)2 ;
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
            Debug.Log("clear player card");
            itemc.clear();
        }
        this.isRemoveHuCardFromDianPao = false ;
        this.mChuIndictor.hide();
    }

    public void shuffle( int mjCnt )
    {
        this.refreshWall(1,0,mjCnt,mjCnt ) ;

        var wallParendNode = this.mWalls[0].transform.parent;
        var pos = wallParendNode.localPosition ;
        pos.y -= this.mWalls[0].wallHeight ;
        wallParendNode.localPosition = pos ;
        wallParendNode.DOLocalMoveY(pos.y + this.mWalls[0].wallHeight,1.5f ) ;
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
    void refreshWall( int nDiceValue , int bankerIdx , int nLeftMJCnt, int mjCnt )
    {
        if ( nDiceValue <= 0 )
        {
            return ;
        }

        int startWallIdx = bankerIdx + nDiceValue - 1 ;
        startWallIdx = startWallIdx % 4 ;
        int startWallLeftFront = ( nDiceValue % 6 + 1 ) * 2 ;

        int usedMJCnt = mjCnt - nLeftMJCnt ;
        int addtionCnt = mjCnt % 8 ;
        int addtionWallCnt = addtionCnt / 2 ;
        int mjCntPerWall = ( mjCnt - addtionCnt ) / 4 ;
        this.mCurWallIdx = -1 ;
        int idx = 0 ;
        for ( int i = startWallIdx ; i < ( startWallIdx + 4 ) ; ++i )
        {
            idx = i % 4 ;
            int mjCntThisWall = mjCntPerWall ;
            if ( addtionCnt > 0 && idx < addtionWallCnt )
            {
                mjCntThisWall += 2 ;
                addtionCnt -= 2 ;
            }
            int curWallCanUseMJCnt = mjCntThisWall;

            if ( idx == startWallIdx )
            {
                curWallCanUseMJCnt -= startWallLeftFront ;
            }

            if ( usedMJCnt >= curWallCanUseMJCnt )
            {
                usedMJCnt -= curWallCanUseMJCnt ;
                curWallCanUseMJCnt = 0 ;
            }
            else
            {
                curWallCanUseMJCnt -= usedMJCnt ;
                usedMJCnt = 0 ;
                if ( this.mCurWallIdx < 0 )
                {
                    this.mCurWallIdx = idx ;
                }
                
            }
            Debug.Log("cur idx = " + idx + "can wall cnt = " + curWallCanUseMJCnt + " usedCnt = " + usedMJCnt );
            this.mWalls[idx].refresh(mjCntThisWall,( startWallIdx == idx ) ? startWallLeftFront : 0,curWallCanUseMJCnt );
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
        this.mChuIndictor.SetPos(pt) ;
    }   

    public void onPeng( int playerIdx , int card , int invokerIdx )
    {
        this.mPlayerCards[playerIdx].onPeng(card,this.getDirection(playerIdx,invokerIdx) );
        this.mPlayerCards[invokerIdx].chuCardBeRemoved(card);
        // hide arrow ;
        this.mChuIndictor.hide();
    }

    public void onMingGang( int playerIdx , int card , int invokerIdx , int NewCard )
    {
        this.mPlayerCards[playerIdx].onMingGang( card,this.getDirection(playerIdx,invokerIdx),NewCard,this.featchCardFromWall() );
        this.mPlayerCards[invokerIdx].chuCardBeRemoved(card);
        // hide arrow ;
        this.mChuIndictor.hide();
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
            this.mChuIndictor.hide();
        }
    }

    public void onEat( int playerIdx , int targetCard , int withA , int withB )
    {
        this.mPlayerCards[playerIdx].onEat(targetCard,withA,withB) ;
        this.mChuIndictor.hide();
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
            return eArrowDirect.eDirect_Left ;
        }

        if ( actIdx ==  ( invokerIdx + 2 ) % 4 )
        {
            return eArrowDirect.eDirect_Opposite ;
        }
        return eArrowDirect.eDirect_Righ ;
    }

    /// test funct
    public void doShuffle()
    {
        this.clear();
        shuffle(108);
    }
    public void doClickDistribute()
    {
        this.refreshWall(2,0,55,108 );
        int nCnt = 9 ;
        List<int> vc = new List<int>();
        while ( nCnt-- > 0 )
        {
            vc.Add(MJCard.makeCardNum(eMJCardType.eCT_Wan,nCnt % 9 + 1 ));
        }
        vc.Add(MJCard.makeCardNum(eMJCardType.eCT_Wan, 1 ));
        vc.Add(MJCard.makeCardNum(eMJCardType.eCT_Wan, 1 ));
        vc.Add(MJCard.makeCardNum(eMJCardType.eCT_Wan, 2 ));
        vc.Add(MJCard.makeCardNum(eMJCardType.eCT_Wan, 4 ));
        vc.Add(MJCard.makeCardNum(eMJCardType.eCT_Wan, 4 ));
        this.refreshPlayerCards(0,null,null,vc,14);
        this.refreshPlayerCards(1,null,null,vc,13);
        this.refreshPlayerCards(2,null,null,vc,13);
        this.refreshPlayerCards(3,null,null,vc,13);
 
    }

    public void doPeng()
    {
        this.onPeng(1,MJCard.makeCardNum(eMJCardType.eCT_Tiao, 1 ),3);
    }

    public void doMo()
    {
        this.onMo(1,MJCard.makeCardNum(eMJCardType.eCT_Tiao, 1 ));
    }

    public void doGang()
    {
        this.onBuGang(1,MJCard.makeCardNum(eMJCardType.eCT_Tiao, 1 ),MJCard.makeCardNum(eMJCardType.eCT_Wan, 6 )) ;
    }

    public void onChu()
    {
        //this.onChu(3,MJCard.makeCardNum(eMJCardType.eCT_Tiao, 1 ) );
        this.selfIdx = 1;
    }
}
