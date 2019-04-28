using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events ;
using DG.Tweening ;
public class PromptText : MonoBehaviour
{
    public Text labelText = null;
    public Image pTextBg = null;
    public int nLeftRightSpace = 160 ;
    // LIFE-CYCLE CALLBACKS:

    public float nDisplayTime = 3 ;
    private Sequence actFade = null ;
    private UnityAction<PromptText> pCloseFunc = null ;
    // onLoad () {}
    public void setText( string strContent , UnityAction<PromptText> closeFunc = null  )
    {
        this.labelText.text = strContent ;
        this.gameObject.SetActive(true) ;
        this.pTextBg.GetComponent<CanvasGroup>().alpha = 1;
        this.pCloseFunc = closeFunc ;
        if ( this.actFade == null )
        {
            this.actFade = DOTween.Sequence();
            this.actFade.PrependInterval(this.nDisplayTime);
            var tp = DOTween.To(()=>1.0f,v=>this.pTextBg.GetComponent<CanvasGroup>().alpha = v,0.0f,0.6f);
            this.actFade.Append(tp.SetEase(Ease.InSine));
            this.actFade.AppendCallback(this.fadeFinish) ;
            this.actFade.SetAutoKill(false);
        }
        this.actFade.Restart();
    }

    void fadeFinish()
    {
        if ( this.pCloseFunc != null )
        {
            this.pCloseFunc(this);
        }
        this.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        if ( this.actFade != null )
        {
            this.actFade.SetRecyclable(false) ;
            this.actFade.Kill(true);
        }
        this.actFade = null ;
    }
}
