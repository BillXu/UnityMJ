using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerCard : MonoBehaviour
{
    public Transform mAnHoldCardNode = null ;
  
    List<MJCard> vAnHoldCard = new List<MJCard>();
    public void Clear()
    {
 
    }

    public void onMo( int nNum )
    {
 
    }

    public void removeAnHoldCard( int nNum , int cnt = 1 )
    {
        // if ( this.vAnHoldCard.Count == 0 )
        // {
        //     while( cnt-- > 0 )
        //     {
        //         this.mMJFactory.recycleUnknownCard(this.mAnHoldCardNode.GetChild(this.mAnHoldCardNode.childCount-1).GetComponent<MJCard>());
        //     }
        //     return ;
        // }

        // List<MJCard> vDel = new List<MJCard>();
        // foreach (var item in this.vAnHoldCard )
        // {
        //     if ( cnt <= 0 )
        //     {
        //         break ;
        //     }

        //     if ( item.cardNum != nNum )
        //     {
        //         continue ;
        //     }
        //     this.mMJFactory.recycleCard(item) ;
        //     vDel.Add(item);
        //     --cnt ;
        // }

        // foreach (var idel in vDel )
        // {
        //     this.vAnHoldCard.Remove(idel);
        // }
        // this.rerangeAnHoldCard();
    }

    void rerangeAnHoldCard()
    {
        // if ( this.vAnHoldCard.Count == 0 )
        // {
        //     var newFetached = this.mAnHoldCardNode.GetChild(this.mAnHoldCardNode.childCount-1);
        //     newFetached.transform.localPosition = new Vector3( (this.mAnHoldCardNode.childCount - 1)  * newFetached.GetComponent<MJCard>().world_x_Size,0,0) ;
        //     return ;
        // }

        // this.vAnHoldCard.Sort( (MJCard a , MJCard b )=>{ return a.cardNum - b.cardNum ;} ) ;
        // for ( int idx = 0 ; idx < this.vAnHoldCard.Count ; ++idx )
        // {
        //     var t = this.vAnHoldCard[idx];
        //     t.transform.localPosition = new Vector3(idx * t.world_x_Size,0,0);
        // }
    }

    public void onChu( int nNum )
    {
 
    }
    void Start()
    {
        
    }
}
