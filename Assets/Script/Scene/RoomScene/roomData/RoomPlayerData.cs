using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public class PlayerActedCard
{
    public eMJActType eAct ;
    public int invokeIdx ;
    public int nTargetCard ; 
    public List<int> vChiFinalCards = null;
    public eArrowDirect eDir ;
} ;

public class RoomPlayerData
{
    public int idx ;
    public int nUID ;
    public int nChips ;
    public bool isOnline ;
    public bool isReady ;
    public List<int> vHoldCards = new List<int>();
    public List<int> vChuCards = new List<int>();
    public List<PlayerActedCard> vActedCards = new List<PlayerActedCard>();

    public bool isSelf{ get ;set ;}

    public void parseBaseInfo( JSONObject jsInfo )
    {
        this.nChips = (int)jsInfo["chips"].Number;
        this.idx = (int)jsInfo["idx"].Number;
        this.nUID = (int)jsInfo["uid"].Number;
        this.isOnline = (int)jsInfo["isOnline"].Number == 1 ;
        // jsInfo["state"]
        // jsInfo["extraTime"]
    }

    public void parseCardInfo( JSONObject jsInfo )
    {

    }

    public void clear()
    {
        this.onEndGame();
        this.nUID = 0 ;
        this.idx = -1 ;
    }

    public bool isEmpty()
    {
        return 0 == this.nUID ;
    }

    public void onMo( int card )
    {
        this.vHoldCards.Add(card) ;
    }

    public void onChu( int card )
    {
        this.removeHold(card);
        this.vChuCards.Add(card);
    }

    public void onChi( int card , int withA , int withB )
    {
        this.removeHold(withA);
        this.removeHold(withB);
        var v = new PlayerActedCard();
        v.eAct = eMJActType.eMJAct_Chi ;
        v.eDir = eArrowDirect.eDirect_Opposite ;
        v.invokeIdx = 0 ;
        v.nTargetCard = card ;
        v.vChiFinalCards = new List<int>();
        v.vChiFinalCards.Add(card);
        v.vChiFinalCards.Add(withA);
        v.vChiFinalCards.Add(withB);
        this.vActedCards.Add(v);
    }

    public void removeChu( int card ) // be penged , eat , gang , or hu 
    {
        var r = this.vChuCards.Pop();
        if ( r != card )
        {
            Debug.LogError("why remove last is not the same ? ");
            this.vChuCards.Add(r);
            return;
        }
    }

    public void onPeng( int card, int invokerIdx )
    {
        this.removeHold(card,2);

        var v = new PlayerActedCard();
        v.eAct = eMJActType.eMJAct_Peng ;
        v.invokeIdx = invokerIdx ;
        v.eDir = this.getDirection(invokerIdx);
        v.nTargetCard = card ;
        this.vActedCards.Add(v);
    }

    public void onAnGang( int card , int newCard )
    {
        this.removeHold(card,4);
        this.vHoldCards.Add(newCard);

        var v = new PlayerActedCard();
        v.eAct = eMJActType.eMJAct_AnGang ;
        v.invokeIdx = this.idx ;
        v.nTargetCard = card ;
        this.vActedCards.Add(v);
    }

    public void onBuGang( int card , int newCard )
    {
        var actedCard = this.getActedCardInfo(card) ;
        if ( actedCard == null )
        {
            Debug.LogError("do not peng , how can bu gang ? card = " + card );
            return ;
        }
        actedCard.eAct = eMJActType.eMJAct_BuGang ;
        this.removeHold(card);
        this.vHoldCards.Add(newCard);
    }
    public void onMingGang( int card , int newCard , int invokerIdx )
    {
        this.removeHold(card,3);
        this.vHoldCards.Add(newCard);

        var v = new PlayerActedCard();
        v.eAct = eMJActType.eMJAct_MingGang ;
        v.invokeIdx = invokerIdx ;
        v.eDir = this.getDirection(invokerIdx);
        v.nTargetCard = card ;
        this.vActedCards.Add(v);
    }

    public void onHu( int card )
    {
        // uselesss ;
    }

    public void onRecivedHoldCard( JSONArray cards , int nCnt )
    {
        for ( int idx = 0 ; idx < nCnt ; ++idx )
        {
            if ( this.isSelf )
            {
                this.vHoldCards.Add( (int)cards[idx].Number );
            }
            else
            {
                this.vHoldCards.Add(0);
            }
        }
    }

    public void onEndGame()
    {
        this.vHoldCards.Clear();
        this.vActedCards.Clear();
        this.vChuCards.Clear();
        this.isReady = false ;
    }

    public PlayerActedCard getActedCardInfo( int keyCard )
    {
        foreach (var item in this.vActedCards )
        {
            if ( item.nTargetCard == keyCard )
            {
                return item ;
            }
        }
        return null ;
    }

    void removeHold( int card , int cnt = 1 )
    {
        while ( cnt-- > 0 )
        {
            if ( this.isSelf )
            {
                this.vHoldCards.Remove(card);
            }
            else
            {
                this.vHoldCards.Pop();
            }
        }
    }

    eArrowDirect getDirection( int invokerIdx )
    {
        if ( this.idx ==  ( invokerIdx + 1 ) % 4 )
        {
            return eArrowDirect.eDirect_Righ ;
        }

        if ( this.idx ==  ( invokerIdx + 2 ) % 4 )
        {
            return eArrowDirect.eDirect_Opposite ;
        }
        return eArrowDirect.eDirect_Left ;
    }
}
