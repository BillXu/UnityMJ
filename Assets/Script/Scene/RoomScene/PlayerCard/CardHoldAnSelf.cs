using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening ;
using UnityEngine.Events ;
public class CardHoldAnSelf : MonoBehaviour,ICardHoldAn,SelfMJCardDelegate
{
    // Start is called before the first frame update
    public MJFactory mMJFactory = null;
    List<MJCard> mCards = new List<MJCard>();
    public UnityAction<int,Vector3> mChuPaiCallBack = null ;
    public bool isCanChu = false ;
    SelfMJCard mCurSelectedCard = null ;
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
            item.GetComponent<SelfMJCard>().enabled = false ;
            this.mMJFactory.recycleCard(item) ;
        }
        this.mCards.Clear();
        this.isCanChu = false ;
        this.mCurSelectedCard = null ;
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

    public void onHu( int nCard )
    {
        this.addCard(nCard);
        var newCard = this.mCards[this.mCards.Count - 1 ] ;
        newCard.curState = MJCard.state.FACE_UP ;
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
        Debug.LogError("self type will not use this function ");
        return Vector3.zero ;
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

            if ( this.mCurSelectedCard != null && this.mCurSelectedCard.transform == pcard.transform )
            {
                this.mCurSelectedCard = null ;
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
        
        if ( this.mCurSelectedCard != null )
        {
            this.mCurSelectedCard.isSelected = false ;
            this.mCurSelectedCard = null ;
        }

    }
    public void showCards( List<int> vCards ) // when game end do shou cards ;
    {
        for ( int i = 0 ; i < this.mCards.Count; ++ i )
        {
            this.mCards[i].curState = MJCard.state.FACE_UP ;
        }
    }

    public float getHoldAnXSize()
    {
        if ( this.mCards.Count == 0 )
        {
            return 0 ;
        }

        return  (this.mCards.Count + 0.5f) * this.mCards[0].world_x_Size ;
    }
    // self card call back ;
    public void onOneClick( SelfMJCard v )
    {
        var mj = v.GetComponent<MJCard>();
        if ( v.isSelected == false )
        {
            v.isSelected = true ;
            var pos = v.transform.localPosition ;
            pos.y = mj.world_y_Size * 0.3f ;
            v.transform.localPosition = pos;
            if ( this.mCurSelectedCard != null )
            {
                this.mCurSelectedCard.isSelected = false ;
                var posr = this.mCurSelectedCard.transform.localPosition ;
                posr.y = 0 ;
                this.mCurSelectedCard.transform.localPosition = posr ;
            }
            this.mCurSelectedCard = v ;
            return ;
        }

        if ( this.isCanChu && this.mChuPaiCallBack != null )
        {
            this.mCurSelectedCard = null ;
            var chuCard = v.GetComponent<MJCard>();
            this.mChuPaiCallBack(chuCard.cardNum,this.doChu(chuCard));
            this.isCanChu = false ;
        }
        else
        {
            v.isSelected = false ;
            var posr = v.transform.localPosition ;
            posr.y = 0 ;
            v.transform.localPosition = posr ;
            this.mCurSelectedCard = null ;
        }
    }

    Vector3 doChu( MJCard card )
    {
        var pos = card.transform.position ;
        card.GetComponent<SelfMJCard>().enabled = false ;
        this.mCards.Remove(card);
        this.mMJFactory.recycleCard(card);
        if ( this.mCurSelectedCard != null && this.mCurSelectedCard.transform == card.transform )
        {
            this.mCurSelectedCard = null ;
        }
        this.rearrangeCard();
        return pos ;

    }
    public void onDoubleClick( SelfMJCard v )
    {
        if ( this.isCanChu && this.mChuPaiCallBack != null )
        {
            var chuCard = v.GetComponent<MJCard>();
            this.mChuPaiCallBack(chuCard.cardNum,this.doChu(chuCard));
            this.isCanChu = false ;
        }

        if ( this.mCurSelectedCard != null )
        {
            this.mCurSelectedCard.isSelected = false ;
            var posr = this.mCurSelectedCard.transform.localPosition ;
            posr.y = 0 ;
            this.mCurSelectedCard.transform.localPosition = posr ;
            this.mCurSelectedCard = null ;
        }
    }
    public bool onDragOut( SelfMJCard v )  // return value indicate , weather go back pos , true do not go back , false go back ;
    {
        if ( this.isCanChu == false || this.mChuPaiCallBack == null )
        {
            if ( this.mCurSelectedCard != null && v.transform == this.mCurSelectedCard.transform )
            {
                this.mCurSelectedCard.isSelected = false ;
                this.mCurSelectedCard = null ;
                // drag act , will reset pos ;
            }
            return false ;
        }
        this.onDoubleClick(v);
        return true ;
    }
}
