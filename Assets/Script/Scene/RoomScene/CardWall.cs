using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardWall : MonoBehaviour
{
    // Start is called before the first frame update
    public MJFactory mMJFactory ;
    public List<MJCard> mCards = new List<MJCard>();
    //public int mFrontWallCnt = 0 ;
    public int mWallTotalCnt = 0 ;
    void Start()
    {

    }

    public float wallHeight
    {
        get
        {
            if ( this.transform.childCount == 0 )
            {
                Debug.LogError("invoke this func , should after refresh");
                return 0 ;
            } 
            var m = this.transform.GetChild(1).GetComponent<MJCard>();
            if ( m == null )
            {
                Debug.LogError("no one ? is null ");
                return 6.0f ;
            }
            return this.transform.GetChild(1).GetComponent<MJCard>().world_y_Size * 2.0f ;
        }
    }
    public void clear()
    {
        while ( this.transform.childCount > 0 )
        {
            var child = this.transform.GetChild(0).GetComponent<MJCard>();
            if ( child == null )
            {
                this.transform.GetChild(0).SetParent(null);
                continue;
            }
            this.mMJFactory.recycleUnknownCard(child) ;
        }
        this.mCards.Clear();
    }

    public void refresh( int totalCnt )
    {
        clear();
        if ( totalCnt % 2 != 0 )
        {
            Debug.LogError("invlaid total wall card count = " + totalCnt );
        }
        int pairCnt = totalCnt / 2 ;
        this.mWallTotalCnt = totalCnt ;
        for ( int pairIdx = 0 ; pairIdx < pairCnt ; ++pairIdx )
        {
            var pBottom = this.mMJFactory.getUnknownCard(this.transform);
            pBottom.curState = MJCard.state.FACE_COVER ;
            var startX = pBottom.world_x_Size * pairCnt / 2;
            pBottom.transform.localPosition = new Vector3( startX - pairIdx * pBottom.world_x_Size,pBottom.world_y_Size,0 );
            this.mCards.Add(pBottom);

            var p = mMJFactory.getUnknownCard(this.transform);
            p.curState = MJCard.state.FACE_COVER ;
            p.transform.localPosition = new Vector3( startX -  pairIdx * p.world_x_Size,0,0 );
            this.mCards.Add(p);
        }
    }

    // public void refresh( int totalCnt , int nFrontCnt, int nBackCnt )
    // {
    //     clear();
    //     if ( totalCnt % 2 != 0 )
    //     {
    //         Debug.LogError("invlaid total wall card count = " + totalCnt );
    //     }
    //     int pairCnt = totalCnt / 2 ;
    //     this.mFrontWallCnt = nFrontCnt ;
    //     this.mWallTotalCnt = totalCnt ;
    //     int nBackBeginIdx = totalCnt - nBackCnt ;
    //     int idx = 0 ;
    //     for ( int pairIdx = 0 ; pairIdx < pairCnt ; ++pairIdx )
    //     {
    //         if ( idx < nFrontCnt || idx >= nBackBeginIdx )
    //         {
    //             var pBottom = this.mMJFactory.getUnknownCard(this.transform);
    //             pBottom.curState = MJCard.state.FACE_COVER ;
    //             var startX = -1 * pBottom.world_x_Size * pairCnt / 2;
    //             pBottom.transform.localPosition = new Vector3( startX + pairIdx * pBottom.world_x_Size,pBottom.world_y_Size,0 );
    //         }
    //         ++idx ;

    //         if ( idx < nFrontCnt || idx >= nBackBeginIdx )
    //         {
    //             var p = mMJFactory.getUnknownCard(this.transform);
    //             p.curState = MJCard.state.FACE_COVER ;
    //             var startX = -1 * p.world_x_Size * pairCnt / 2;
    //             p.transform.localPosition = new Vector3( startX +  pairIdx * p.world_x_Size,0,0 );
    //         }
    //         ++idx ;
    //     }
    // }
    public Vector3 removeCardFromWall( int idx )
    {
        if ( this.isTargetCardEmpty(idx) )
        {
            Debug.Log("this wall is empty front cnt = " + idx );
            return Vector3.zero;

        }

        var card = this.mCards[idx] ;
        var pos = card.transform.position; ;
        this.mMJFactory.recycleUnknownCard(card);
        this.mCards[idx] = null ;
        return pos ;
    }
    public bool isTargetCardEmpty( int idx )
    {
        if ( idx < this.mCards.Count )
        {
            return this.mCards[idx] == null ;
        }
        return true ;
    }

    public bool showWallCard( int idx , int cardNum )
    {
        if ( this.isTargetCardEmpty(idx) )
        {
            Debug.LogWarning("target idx is null can not show card = " + cardNum );
            return false ;
        }

        var pos = this.mCards[idx].transform.localPosition ;
        removeCardFromWall(idx) ;
        var card = this.mMJFactory.getCard(cardNum,this.transform);
        card.curState = MJCard.state.FACE_UP ;
        this.mCards[idx] = card ;
        card.transform.localPosition = pos ;
        Debug.LogWarning("show wall card = idx " + idx + " cardnum =" + cardNum );
        return true ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
