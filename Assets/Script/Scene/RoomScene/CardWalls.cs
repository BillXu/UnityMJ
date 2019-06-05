using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening ;
public class CardWalls : MonoBehaviour
{
    public List<CardWall> mWalls ;

    int mStartWallPreseverCnt = 0 ;

    int mCurWallIdx = 0 ;
    int mMoIdxOfCurWall = 0 ;

    int mCurGangWallIdx = 0 ;
    int mGangIdxOfGangWall = 0 ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void shuffle( int mjCnt, int nbankeIdx , int nDiceValue )
    {
        this.prepareWalls( mjCnt,nbankeIdx,nDiceValue );
        var wallParendNode = this.mWalls[0].transform.parent;
        var pos = wallParendNode.localPosition ;
        pos.y -= this.mWalls[0].wallHeight ;
        wallParendNode.localPosition = pos ;
        wallParendNode.DOLocalMoveY(pos.y + this.mWalls[0].wallHeight,0.6f ) ;
    }
    public void clear()
    {
        for (int i = 0; i < 4; i++)
        {
            this.mWalls[i].clear();
        }

        this.mStartWallPreseverCnt = 0 ;

        this.mCurWallIdx = 0 ;
        this.mMoIdxOfCurWall = 0 ;

        this.mCurGangWallIdx = 0 ;
        this.mGangIdxOfGangWall = 0 ;
    }

    void prepareWalls( int mjCnt , int nbankeIdx , int nDiceValue )
    {
        int addtionCnt = mjCnt % 8 ;
        int mjCntPerWall = ( mjCnt - addtionCnt ) / 4 ;
        for ( int i = 0; i < 4; i++ )
        {
            this.mWalls[i].refresh(mjCntPerWall + ( addtionCnt > 0 ? 2 : 0 ) );
            if ( addtionCnt > 0 )
            {
                addtionCnt -= 2 ;
            }
        }

        int StartWallIdx = nbankeIdx + nDiceValue - 1 ;
        StartWallIdx = StartWallIdx % 4 ;
        this.mStartWallPreseverCnt = ( nDiceValue % 6 + 1 ) * 2 ;
        
        this.mCurWallIdx = StartWallIdx ;
        this.mMoIdxOfCurWall = this.mStartWallPreseverCnt ;

        this.mCurGangWallIdx = StartWallIdx ;
        this.mGangIdxOfGangWall = this.mMoIdxOfCurWall - 2 ;
    }

    public void refresh( RoomBaseData data , int gangCnt )
    {
        int mjCnt = data.initCardCnt ;
        int normalUseCnt = mjCnt - data.leftMJCnt - gangCnt ;
        this.prepareWalls(mjCnt,data.bankerIdx,data.diceValue);
        this.showWallCard(data.wallCard8,data.wallCard16 ) ;

        while ( normalUseCnt > 0 )
        {
            this.moCardFromWall(false);
            --normalUseCnt ;
        }

        while ( gangCnt > 0 )
        {
            this.moCardFromWall(true);
            --gangCnt ;
        }
    }

    public void showWallCard( int card8 , int card16 )
    {
        this.showWallCardByGroupIdx(3,card8);
        this.showWallCardByGroupIdx(7,card16 );
    }
    void showWallCardByGroupIdx( int reverGroupIdx, int nCardNum )
    {
        Debug.LogWarning("showWallCardByGroupIdx = " + nCardNum );
        int persvCnt = this.mStartWallPreseverCnt / 2 ;
        if ( reverGroupIdx < persvCnt )
        {
            int nNormalIdx = persvCnt - 1 - reverGroupIdx ;
            this.mWalls[this.mCurWallIdx].showWallCard(nNormalIdx * 2,nCardNum);
            return ;
        }

        // locate it last wall ;
        var lastWallIdx  =  ( 4 + this.mCurWallIdx - 1 ) % 4 ;
        var gcnt = this.mWalls[lastWallIdx].mWallTotalCnt / 2 ; 
        reverGroupIdx -= persvCnt ;
        int normalIdx = gcnt - 1 - reverGroupIdx ;
        Debug.LogWarning("gcnt = " + gcnt + " nomral idx " + normalIdx + " persvCnt " + persvCnt + " rever groupidx " + reverGroupIdx );
        this.mWalls[lastWallIdx].showWallCard(normalIdx * 2,nCardNum);
    }
    public Vector3 moCardFromWall( bool isGang )
    {
        if ( isGang )
        {
            return gangCardFromWall();
        }

        if ( false == this.mWalls[this.mCurWallIdx].isTargetCardEmpty( this.mMoIdxOfCurWall) )
        {
            return this.mWalls[this.mCurWallIdx].removeCardFromWall( this.mMoIdxOfCurWall++ );
        }

        if ( this.mMoIdxOfCurWall < this.mWalls[this.mCurWallIdx].mWallTotalCnt )
        {
            Debug.LogWarning("all card are be mo , finished ? why can mo now ? ");
            return Vector3.zero ;
        }

        this.mCurWallIdx = ( this.mCurWallIdx + 1 ) % 4 ;
        this.mMoIdxOfCurWall = 0 ;
        return moCardFromWall(isGang);
    }

    public Vector3 gangCardFromWall()
    {
        var ret = this.mWalls[this.mCurGangWallIdx].removeCardFromWall( this.mGangIdxOfGangWall ) ;
        if ( this.mGangIdxOfGangWall % 2 == 0 )
        {
            ++this.mGangIdxOfGangWall ;
        }
        else
        {
            this.mGangIdxOfGangWall -= 3 ;
        }

        if ( this.mGangIdxOfGangWall < 0 )
        {
            this.mCurGangWallIdx = ( 4 + this.mCurGangWallIdx - 1 ) % 4;
            this.mGangIdxOfGangWall = this.mWalls[this.mCurGangWallIdx].mWallTotalCnt - 2 ;
            return gangCardFromWall();
        }
        return ret ;
    }
}
