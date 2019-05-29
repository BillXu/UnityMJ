using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public class ResultTotalDataItem 
{
    public int uid ;
    public int huCnt ;
    public int gangCnt ;
    public int dianPaoCnt ;
    public int SingleBestWin ; 
    public int final ;
    public int waitTime ;
}

public class ResultTotalData
{
    // Start is called before the first frame update
    public List<ResultTotalDataItem> mResults = new List<ResultTotalDataItem>();
    public List<int> mBigWinerUID = new List<int>() ;
    public int mApplyDismissID = 0 ;
    public List<int> mTuHaoID = new List<int>() ;

    public void parseResult( JSONObject js )
    {
        if ( js.ContainsKey("dismissID") )
        {
            this.mApplyDismissID = (int)js["dismissID"].Number ;
        }

        var jsRs = js["result"].Array;
        foreach (var item in jsRs )
        {
            var jsobj = item.Obj;
            var p = new ResultTotalDataItem();
            p.uid = (int)jsobj["uid"].Number;
            p.dianPaoCnt = (int)jsobj["dianPaoCnt"].Number;
            p.final = (int)jsobj["final"].Number;
            p.gangCnt = (int)(jsobj["anGangCnt"].Number + jsobj["mingGangCnt"].Number);
            p.huCnt = (int)jsobj["huCnt"].Number;
            p.waitTime = (int)jsobj["extraTime"].Number / 60 ;
            this.mResults.Add(p);
        }

        // find tu hao , and big winer ;
        int tuHaoLose = -1 ;
        int nBigWiner = 1 ;
        foreach ( var item in this.mResults )
        {
            if ( tuHaoLose > item.final )
            {
                this.mTuHaoID.Clear();
                this.mTuHaoID.Add( item.uid );
                tuHaoLose = item.final ;
            }

            if ( item.final == tuHaoLose )
            {
                this.mTuHaoID.Add( item.uid );
            }

            if ( nBigWiner < item.final )
            {
                this.mBigWinerUID.Clear();
                this.mBigWinerUID.Add(item.uid );
                nBigWiner = item.final ;
            }

            if ( nBigWiner == item.final )
            {
                this.mBigWinerUID.Add(item.uid );
            }
        }
    }
}
