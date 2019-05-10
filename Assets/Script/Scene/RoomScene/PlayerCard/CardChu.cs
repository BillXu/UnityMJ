using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening ;
public class CardChu : MonoBehaviour
{
    // Start is called before the first frame update
    public MJFactory mMJFactory = null;
    int mCountPerRow = 8 ;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clear()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var mj = this.transform.GetChild(i).GetComponent<MJCard>();
            this.mMJFactory.recycleCard(mj) ;
        }
    }

    public void refresh( List<int> vChuCards )
    {
        clear();
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
            int rowIdx = (curIdx + this.mCountPerRow - 1 ) / this.mCountPerRow;
            var colIdx = curIdx % this.mCountPerRow ;
            mj.transform.localPosition = new Vector3( colIdx * mj.world_x_Size, -1* mj.world_z_Size * rowIdx ,0 ) ;
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
        int rowIdx = (curIdx + this.mCountPerRow - 1 ) / this.mCountPerRow;
        var colIdx = curIdx % this.mCountPerRow ;
        var posTarget = new Vector3( colIdx * mj.world_x_Size, -1* mj.world_z_Size * rowIdx ,0 ) ;
        mj.transform.DOMove(posTarget,0.3f) ; 
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
