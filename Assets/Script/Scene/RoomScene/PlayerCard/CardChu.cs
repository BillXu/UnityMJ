using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening ;
public class CardChu : MonoBehaviour
{
    // Start is called before the first frame update
    public MJFactory mMJFactory = null;
    int mCountPerRow = 6 ;
    void Start()
    {
        // int nCnt = 16 ;
        // List<int> vc = new List<int>();
        // while ( nCnt-- > 0 )
        // {
        //     vc.Add(MJCard.makeCardNum(eMJCardType.eCT_Wan,nCnt % 9 + 1 ));
        // }
        // refresh(vc) ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clear()
    {
        while ( this.transform.childCount > 0 )
        {
            var child = this.transform.GetChild(0).GetComponent<MJCard>();
            if ( child == null )
            {
                this.transform.GetChild(0).gameObject.SetActive(false);
                this.transform.GetChild(0).SetParent(null);
                Debug.Log("directo remove d");
            }
            this.mMJFactory.recycleCard(child) ;
        }
    }

    public void refresh( List<int> vChuCards )
    {
        clear();
        if ( vChuCards == null )
        {
            return ;
        }

        foreach (var item in vChuCards )
        {
            var mj = this.mMJFactory.getCard(item,this.transform) ;
            if ( null == mj )
            {
                Debug.LogError("create mj null for chu num = " + item );
                continue;
            }
            mj.curState = MJCard.state.FACE_UP;     

            var curIdx = this.transform.childCount ;
            int rowIdx = (curIdx + this.mCountPerRow - 1 ) / this.mCountPerRow - 1;
            var colIdx = ( curIdx - 1 ) % this.mCountPerRow ;
            mj.transform.localPosition = new Vector3( colIdx * mj.world_x_Size, 0,-1* mj.world_z_Size * rowIdx ) ;
        }
    }

    public void addChu( int nCardNum , Vector3 holdWordPos )
    {
        var mj = this.mMJFactory.getCard(nCardNum,this.transform) ;
        if ( null == mj )
        {
            Debug.LogError("create mj null for chu num = " + nCardNum );
            return;
        }
        mj.curState = MJCard.state.FACE_UP;
        mj.transform.position = holdWordPos; 

        var curIdx = this.transform.childCount ;
        int rowIdx = (curIdx + this.mCountPerRow - 1 ) / this.mCountPerRow - 1;
        var colIdx = ( curIdx - 1 ) % this.mCountPerRow ;
        var posTarget = new Vector3( colIdx * mj.world_x_Size, 0,-1* mj.world_z_Size * rowIdx ) ;
        mj.transform.DOLocalMove(posTarget,0.3f) ; 
    }

    public void removeLastChu( int nCard )
    {
        if ( this.transform.childCount == 0 )
        {
            Debug.LogError("child count is 0 , can not remove chu ");
            return ;
        }
        var lastCard = this.transform.GetChild(this.transform.childCount-1).GetComponent<MJCard>();
        if ( lastCard.cardNum != nCard )
        {
            Debug.LogError("can not remove , last chu card is not " + nCard );
            return ;
        }
        this.mMJFactory.recycleCard(lastCard) ;
    }

    public Vector3 getLastChuCardWorldPos()
    {
        if ( this.transform.childCount == 0 )
        {
            return this.transform.position;
        }
        return this.transform.GetChild(this.transform.childCount-1).transform.position ;
    }
}
