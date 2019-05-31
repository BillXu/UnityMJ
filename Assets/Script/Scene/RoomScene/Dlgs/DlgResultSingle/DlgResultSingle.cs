using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
using UnityEngine.Events ;
using DG.Tweening ;
using UnityEngine.UI ;
public class DlgResultSingle : MonoBehaviour
{
    public List<ResultSingleItem> mSingleItems ;
    public GameObject mLiuju ;
    List<Vector3> mInitPos = new List<Vector3>();
    public int selfIdx 
    {
        set 
        {
            int idx = 0 ;
            if ( value >= 0 )
            {
                idx = value ;
            }

            for ( int i = 0 ; i < 4 ; ++i )
            {
                this.mSingleItems[(i+idx)%4].transform.localPosition = this.mInitPos[i];
            }
        }
    }
    public UnityEvent onNext = null;
    public GameObject mNext;
    public GameObject mTotal;
    public bool isFinal
    {
        set 
        {
            this.mNext.SetActive( value == false );
            this.mTotal.SetActive( value );
        }

    }
    private void Awake() {
        foreach (var item in this.mSingleItems )
        {
            this.mInitPos.Add(item.transform.localPosition );            
        }
        this.clear();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showResult( ResultSingleData data )
    {
        this.gameObject.SetActive(true);
        foreach (var item in data.mResults )
        {
            if ( item == null || item.isEmpty() )
            {
                continue ;
            }

            var idx = item.mIdx ;
            this.mSingleItems[idx].gameObject.SetActive(true);
            this.mSingleItems[idx].gangScore = item.mGangScore ;
            this.mSingleItems[idx].huScore = item.mHuScore ;
            if ( item.haveHu() )
            {
                this.mSingleItems[idx].huType = item.getHuTypeStr();
            }
        }

        if ( data.mIsLiuJu )
        {
            var p = this.mLiuju.GetComponent<CanvasGroup>();
            this.mLiuju.SetActive(true);
            p.alpha = 1;
            var seq = DOTween.Sequence();
            seq.AppendInterval(1);
            seq.Append( p.DOFade(0,0.5f ));
            seq.AppendCallback( ()=>{this.mLiuju.SetActive(false);} ) ;
        }
    }

    public void onClickNext()
    {
        if ( null != this.onNext )
        {
            this.onNext.Invoke();
        }
        close();
    }

    public void close()
    {
        this.gameObject.SetActive(false);
        this.clear();
    }

    public bool isShow()
    {
        return this.gameObject.activeSelf ;
    }

    void clear()
    {
        foreach (var item in this.mSingleItems )
        {
            item.gameObject.SetActive(false);
        }
        this.mLiuju.SetActive(false);
    }
}
