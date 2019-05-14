using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening ;
public class CardChuIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform mArrow = null;
    Tween mAnimateHight = null ;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hide()
    {
        this.mArrow.gameObject.SetActive(false);
    }

    public void SetPos( Vector3 ptWorldPos )
    {
        this.mArrow.gameObject.SetActive(true);
        ptWorldPos.y = this.transform.position.y ;
        this.transform.position = ptWorldPos ;
        if ( this.mAnimateHight == null )
        {
            this.mAnimateHight = DOTween.To(()=>{return 0;},this.animateValue,10000,2.0f).SetLoops(-1) ;
        }
        this.mAnimateHight.Restart();
    }

    void animateValue( int v )
    {
        var va = Mathf.Abs(5000-v)/5000.0f ; // 0--1 ;
        var pos = this.mArrow.localPosition;
        pos.y = va * 1.0f;
        this.mArrow.localPosition = pos;
    }

    private void OnDestroy() {
        this.mAnimateHight.Kill();
        this.mAnimateHight = null ;    
    }
}
