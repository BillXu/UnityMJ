using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;
using DG.Tweening ;
public class SeatOrientation : MonoBehaviour
{
    // Start is called before the first frame update
    public List<RawImage> mHighLight;

    Tween mAnimateHight = null ;
    RawImage mCurHighLigt = null ;
    public void setCurActIdx( int idx )
    {
        for ( int i = 0; i < this.mHighLight.Count; i++ )
        {
            this.mHighLight[i].gameObject.SetActive( idx == i );
        }

        if ( idx < 0 || idx >= this.mHighLight.Count )
        {
            if ( this.mAnimateHight != null )
            {
                this.mAnimateHight.Pause();
            }
            return ;
        }

        this.mCurHighLigt = this.mHighLight[idx] ;
        if ( this.mAnimateHight == null )
        {
            this.mAnimateHight = DOTween.To(()=>{return 0;},this.animateValue,10000,2.8f).SetLoops(-1) ;
        }
        this.mAnimateHight.Restart();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void animateValue( int v )
    {
        var va = Mathf.Abs(5000-v)/5000.0f ; // 0--1 ;
        va = 0.35f + va * 0.65f ; // 0.35 - 1 ;
        this.mCurHighLigt.color = new Color(1,1,1,va ) ;
    }

    private void OnDestroy() {
        this.mAnimateHight.Kill();
        this.mAnimateHight = null ;    
    }
}
