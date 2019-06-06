using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening ;
using System ;
public class LayerCards : MonoBehaviour
{
    public List<PlayerCard> mPlayerCards ;
    public CardChuIndicator mChuIndictor ;
    public RoomScene mScene ;
    public CardWalls mWalls ;
    bool isRemoveHuCardFromDianPao = false ;
    int lastWallIdx
    {
        get
        {
            var baseData = mScene.mRoomData.mBaseData;
            int startWallIdx = baseData.bankerIdx + baseData.diceValue - 1 ;
            startWallIdx = startWallIdx % 4 ;
            return startWallIdx ;
        }
    }
    public Transform mCameraParent ; 
    int _selfIdx = -1 ;
    public int selfIdx
    {
        set
        {
            if ( this._selfIdx == value )
            {
                return ;
            }
            this.clear();
            for ( int idx = 0 ; idx < 4 ; ++idx )
            {
                if ( idx == value )
                {
                    Debug.Log(" self = idx = value " + idx + " v" + value );
                    this.mPlayerCards[idx].makeSelfPlayerCard( this.mScene.requestChu ,value ) ;
                }
                else
                {
                    var ePosDir = getDirection(value,idx) ;
                    this.mPlayerCards[idx].makeOtherPlayerCard(ePosDir,value );
                }
            }

            this.mCameraParent.transform.DOLocalRotate( new Vector3(0,value * -90,0),0.3f);
            switch ( value )
            {
                case 0 :
                {
                    GameObject.Find("Main Camera").transform.localPosition = new Vector3(0,201f,-165.8f);
                }
                break;
                case 1:
                {
                    GameObject.Find("Main Camera").transform.localPosition = new Vector3(0,208f,-173.1f);
                }
                break;
                case 2 :
                {
                    GameObject.Find("Main Camera").transform.localPosition = new Vector3(0,201.2f,-167.6f);
                }
                break ;
                case 3 :
                {
                    GameObject.Find("Main Camera").transform.localPosition = new Vector3(0,206f,-171f);
                }
                break ;
            }
            
            this._selfIdx = value ;
        }

        get
        {
            return this._selfIdx ;
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
        this.mWalls.clear();

        foreach (var itemc in this.mPlayerCards )
        {
            Debug.Log("clear player card");
            itemc.clear();
        }
        this.isRemoveHuCardFromDianPao = false ;
        this.mChuIndictor.hide();
    }

    public void refresh( RoomData data )
    {
        var idx = data.getSelfIdx() ;
        this.selfIdx = idx < 0 ? 0 : idx;
        this.clear();
        if ( data.mBaseData.isDuringGame() == false )
        {
            return ;
        }

        this.mWalls.refresh(data.mBaseData,data.getAlreadyGangCnt() );
    
        foreach (var item in data.mPlayers )
        {
            if ( item == null || item.isEmpty() )
            {
                continue ;
            }
            this.refreshPlayerCards(item.idx,item.vChuCards,item.vActedCards,item.vHoldCards,item.vHoldCards.Count );
        }
    }

    public void refreshPlayerCards( int playerIdx , List<int> vChu , List<PlayerActedCard> vMing , List<int> vHoldAn, int holdAnCnt )
    {
        this.mPlayerCards[playerIdx].refresh(vChu,vMing,vHoldAn,holdAnCnt);
    }

    public void onDistribute( RoomData data )
    {
        Debug.Log("onDistribute card sss");
        // this.refreshWall( data.mBaseData.diceValue,data.mBaseData.bankerIdx,data.mBaseData.initCardCnt,data.mBaseData.initCardCnt );
        StartCoroutine("doDistribute");
    }

    public IEnumerator doDistribute()
    {
        yield return new WaitForSeconds(0.9f);
        var data = this.mScene.mRoomData ;
        var baseData = this.mScene.mRoomData.mBaseData ;
        this.mWalls.shuffle(baseData.initCardCnt,baseData.bankerIdx,baseData.diceValue);
        yield return new WaitForSeconds(1.5f);

        for ( int iRound = 0 ; iRound < 3 ; ++iRound )
        {
            foreach (var item in data.mPlayers )
            {
                if ( item == null || item.isEmpty() )
                {
                    continue ;
                }

                // decrease wall
                var cnt = 4 ;
                if ( iRound == 2 )
                {
                    cnt = item.idx == baseData.bankerIdx ? 6 : 5 ;
                }


                for ( int i = 0 ; i < cnt ; ++i )
                {
                    this.featchCardFromWall();
                }
                this.mPlayerCards[item.idx].onDistribute(item.vHoldCards.GetRange(iRound * 4,cnt),cnt);
            }
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.8f);
        this.mWalls.showWallCard(baseData.wallCard8,baseData.wallCard16 ) ;
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
        this.mPlayerCards[invokerIdx].chuCardBeRemoved(card);
        this.mPlayerCards[playerIdx].onPeng(card,this.getDirection(playerIdx,invokerIdx) );
        // hide arrow ;
        this.mChuIndictor.hide();
    }

    public void onMingGang( int playerIdx , int card , int invokerIdx , int NewCard )
    {
        this.mPlayerCards[invokerIdx].chuCardBeRemoved(card);
        this.mPlayerCards[playerIdx].onMingGang( card,this.getDirection(playerIdx,invokerIdx),NewCard,this.featchCardFromWall(true) );
        // hide arrow ;
        this.mChuIndictor.hide();
    }

    public void onBuGang( int playerIdx , int card , int NewCard )
    {
        this.mPlayerCards[playerIdx].onBuGang( card,NewCard,this.featchCardFromWall(true) );
    } 

    public void onAnGang( int playerIdx , int card , int NewCard )
    {
        this.mPlayerCards[playerIdx].onAnGang(card,NewCard,this.featchCardFromWall(true) );
    }

    public void onHu( int playerIdx , int card , int invokerIdx )
    {
        if ( playerIdx != invokerIdx && this.isRemoveHuCardFromDianPao == false )
        {
            this.mPlayerCards[invokerIdx].chuCardBeRemoved(card);
            this.isRemoveHuCardFromDianPao = true ;
            this.mChuIndictor.hide();
        }

        if ( playerIdx != invokerIdx )  // zi mo need not add hold card
        {
            this.mPlayerCards[playerIdx].onHu(card);
        }
        
    }

    public void onEat( int playerIdx , int targetCard , int withA , int withB )
    {
        this.mPlayerCards[playerIdx].onEat(targetCard,withA,withB) ;
        this.mPlayerCards[( playerIdx - 1 + 4 ) % 4 ].chuCardBeRemoved(targetCard);
        this.mChuIndictor.hide();
    }

    public void showHoldCards( int playerIdx , List<int> vCards )
    {
        this.mPlayerCards[playerIdx].showCards(vCards);
    }

    Vector3 featchCardFromWall( bool isGang = false )
    {
        return this.mWalls.moCardFromWall(isGang) ;
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


    /// test func
    public int testSelfIdx = 0 ;
    public void TestFunc_0()
    {
        this.mWalls.shuffle(136,1,1) ;
    }

    public void TestFunc_1()
    {
        this.clear();
        this.selfIdx = (++testSelfIdx) % 4;
    }

    public void TestFunc_2()
    {
        List<int> vCards = new List<int>();
        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,1) );
        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,1) );
        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,1) );
        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,2) );

        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,3) );
        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,3) );
        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,4) );
        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,4) );

        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,5) );
        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,5) );
        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,6) );
        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,6) );

        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,7) );
        vCards.Add( MJCard.makeCardNum( eMJCardType.eCT_Wan,7) );
        this.mPlayerCards[this.selfIdx].onDistribute(vCards,vCards.Count);

        int nCnt = 12 ;
        vCards.Clear();
        while ( nCnt-- > 0 )
        {
            vCards.Add(0);
        }
        this.mPlayerCards[(this.selfIdx + 1 ) % 4].onDistribute(vCards,vCards.Count);
        this.mPlayerCards[(this.selfIdx + 2 ) % 4].onDistribute(vCards,vCards.Count);
        this.mPlayerCards[(this.selfIdx + 3 ) % 4].onDistribute(vCards,vCards.Count);
    }
    public void TestFunc_3()
    {
        this.mPlayerCards[(this.selfIdx + 1 ) % 4].onMingGang(MJCard.makeCardNum( eMJCardType.eCT_Tong,9),eArrowDirect.eDirect_Opposite,MJCard.makeCardNum( eMJCardType.eCT_Wan,7),Vector3.zero);
        this.mPlayerCards[this.selfIdx].onMingGang(MJCard.makeCardNum( eMJCardType.eCT_Wan,1),eArrowDirect.eDirect_Opposite,MJCard.makeCardNum( eMJCardType.eCT_Wan,7),Vector3.zero);
        this.mPlayerCards[(this.selfIdx + 2 ) % 4].onMingGang(MJCard.makeCardNum( eMJCardType.eCT_Tiao,9),eArrowDirect.eDirect_Opposite,MJCard.makeCardNum( eMJCardType.eCT_Wan,7),Vector3.zero);
        this.mPlayerCards[(this.selfIdx + 3 ) % 4].onMingGang(MJCard.makeCardNum( eMJCardType.eCT_Tong,7),eArrowDirect.eDirect_Opposite,MJCard.makeCardNum( eMJCardType.eCT_Wan,7),Vector3.zero);
    }


}
