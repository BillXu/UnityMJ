using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;

public class TotalResultItem : MonoBehaviour
{
    public PlayerInfoItem mInfo ;
    public Text mHuPaiCnt ;
    public Text mGangPaiCnt ;
    public Text mDianPaoCnt ;
    public Text mBestSingleWin ;
    public Text mFianlWin ;
    public Text mFinalLose ;
    public GameObject mBigWinIcon ;
    public GameObject mApplyDismissIcon ;
    public GameObject mTuHaoIcon ;
    public Text mWaitTime ;
    public GameObject mBigWinBG ;

    public int playerUID 
    {
        set
        {
            this.mInfo.playerUID = value ;
        }
    }

    public int huCnt
    {
        set
        {
            this.mHuPaiCnt.text = "" + value ;
        }
    }

    public int gangCnt
    {
        set
        {
            this.mGangPaiCnt.text = "" + value ;
        }
    }

    public int dianPaoCnt 
    {
        set
        {
            this.mDianPaoCnt.text = "" + value ;
        }
    }

    public int bestSingleWin
    {
        set
        {
            this.mBestSingleWin.text = "" + value ;
        }
    }

    public bool isBigWin
    {
        set
        {
            this.mBigWinBG.SetActive(value);
            this.mBigWinIcon.SetActive(value);
        }
    }

    public bool isDismiss
    {
        set
        {
            this.mApplyDismissIcon.SetActive(value);
        }
    }

    public bool isTuHao
    {
        set
        {
            this.mTuHaoIcon.SetActive( value );
        }
    }
    public int waitTime
    {
        set
        {
            this.mWaitTime.text = "等待操作时间： " + value + "分钟" ;
        }
    }
    public int final
    {
        set 
        {
            var t = value >= 0 ? this.mFianlWin : this.mFinalLose;
            t.gameObject.SetActive(true);
            t.text = "" + value ;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
