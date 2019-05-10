using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening ;
public class CardHoldAnSelf : MonoBehaviour,ICardHoldAn,SelfMJCardDelegate
{
    // Start is called before the first frame update
    public MJFactory mMJFactory = null;
    List<MJCard> mCards = new List<MJCard>();
    public bool isCanChu = false ;
    void Start()
    {
        // sholud look at camera ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clear()
    {
        foreach (var item in this.mCards )
        {
            item.enabled = false ;
            this.mMJFactory.recycleCard(item.GetComponent<MJCard>()) ;
        }
        this.mCards.Clear();
        this.isCanChu = false ;
    }

    void addCard( int cardNum )
    {
        var p = this.mMJFactory.getCard(cardNum,this.transform);
        var self = p.GetComponent<SelfMJCard>();
        if ( self == null )
        {
            self = p.gameObject.AddComponent<SelfMJCard>();
        }
        self.enabled = true ;
        self.mDelegate = this ;
        p.curState = MJCard.state.FACE_USER ;
        p.transform.localPosition = new Vector3( ( this.transform.childCount - 1 ) * p.world_x_Size,0,0);
        this.mCards.Add(p);
    }
    public void refresh( List<int> vCards , int nCnt )
    {
        this.clear();
        vCards.Sort();
        for ( int i = 0  ; i < vCards.Count ; ++i )
        {
            addCard(vCards[i]);
        }
    }

    public void onMo( int nCard, Vector3 ptWallCardWorldPos )
    {
        this.addCard(nCard);
        var newCard = this.mCards[this.mCards.Count - 1 ] ;
        newCard.transform.position = ptWallCardWorldPos ;
        var targetPos = new Vector3((this.mCards.Count - 1 )* newCard.world_x_Size + newCard.world_x_Size * 0.3f,0,0 );
        newCard.transform.DOLocalMove(targetPos,0.3f) ;
    }

    public void onDistribute( List<int> vCards , int nCnt )
    {
        for ( int i = 0  ; i < vCards.Count ; ++i )
        {
            addCard(vCards[i]);
        }
    }
    public Vector3 doChu( int nCard )
    {
        var pcard = this.mCards.Find( ( MJCard c )=> { return c.cardNum == nCard ;} ) ;
        if ( null == pcard )
        {
            Debug.LogError("hold card do not have empty card = " + nCard );
            return Vector3.zero  ;
        }        
        var pos = pcard.transform.position ;
        this.removeCard(nCard,1);
        this.isCanChu = false ;
        return pos ;
    }

    public void removeCard( int nCard, int cnt )
    {
        while( cnt-- > 0 )
        {
            var pcard = this.mCards.Find( ( MJCard c )=> { return c.cardNum == nCard ;} ) ;
            if ( null == pcard )
            {
                Debug.LogError("hold card do not have empty card = " + nCard );
                break ;
            }
            pcard.GetComponent<SelfMJCard>().enabled = false ;
            this.mMJFactory.recycleCard(pcard);
            this.mCards.Remove(pcard);
        }
        this.rearrangeCard();
    }

    public void onWaitChu()
    {
        this.isCanChu = true ;
    }
    public void rearrangeCard()
    {
        this.mCards.Sort((MJCard a , MJCard b )=>{ return a.cardNum - b.cardNum ;});
        for ( int i = 0 ; i < this.mCards.Count; ++ i )
        {
            this.mCards[i].transform.localPosition = new Vector3( i * this.mCards[i].world_x_Size , 0 ,0 ) ;
        }
    }
    public void showCards( List<int> vCards ) // when game end do shou cards ;
    {
        for ( int i = 0 ; i < this.mCards.Count; ++ i )
        {
            this.mCards[i].curState = MJCard.state.FACE_UP ;
        }
    }

    // self card call back ;
    public void onOneClick( SelfMJCard v )
    {

    }
    public void onDoubleClick( SelfMJCard v )
    {

    }
    public bool onDragOut( SelfMJCard v )  // return value indicate , weather go back pos , true do not go back , false go back ;
    {
        return false ;
    }
}
