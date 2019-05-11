using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardWall : MonoBehaviour
{
    // Start is called before the first frame update
    public MJFactory mMJFactory ;
    public int mFrontWallCnt = 0 ;
    int mWallTotalCnt = 0 ;
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

            return this.transform.GetChild(0).GetComponent<MJCard>().world_y_Size * 2.0f ;
        }
    }
    public void clear()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.mMJFactory.recycleUnknownCard(this.transform.GetChild(i).GetComponent<MJCard>()) ;
        }
    }

    public void refresh( int totalCnt , int nFrontCnt, int nBackCnt )
    {
        clear();
        if ( totalCnt % 2 != 0 )
        {
            Debug.LogError("invlaid total wall card count = " + totalCnt );
        }
        int pairCnt = totalCnt / 2 ;
        this.mFrontWallCnt = nFrontCnt ;
        this.mWallTotalCnt = totalCnt ;

        int idx = 0 ;
        for ( int pairIdx = 0 ; pairIdx < pairCnt ; ++pairIdx )
        {
            if ( idx < nFrontCnt || idx > nBackCnt )
            {
                var pBottom = this.mMJFactory.getUnknownCard(this.transform);
                pBottom.curState = MJCard.state.FACE_COVER ;
                pBottom.transform.localPosition = new Vector3( pairIdx * pBottom.world_x_Size,0,0 );
            }
            ++idx ;

            if ( idx < nFrontCnt || idx > nBackCnt )
            {
                var p = mMJFactory.getUnknownCard(this.transform);
                p.curState = MJCard.state.FACE_COVER ;
                p.transform.localPosition = new Vector3(pairIdx * p.world_x_Size,p.world_y_Size,0 );
            }
            ++idx ;
        }
    }
    public Vector3 onFetchCardFromWall()
    {
        if ( getWallLeftCnt() < 0 )
        {
            Debug.LogError("this wall is empty front cnt = " + this.mFrontWallCnt );
            return Vector3.zero;

        }

        var tran = this.transform.GetChild(this.mFrontWallCnt);
        return tran.position;
    }
    public int getWallLeftCnt()
    {
        return this.transform.childCount - this.mFrontWallCnt ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
