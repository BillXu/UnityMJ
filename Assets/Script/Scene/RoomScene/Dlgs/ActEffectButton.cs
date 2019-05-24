using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening ;
using System ;
using UnityEngine.Events ;

public class ActEffectButton : MonoBehaviour
{
    // Start is called before the first frame update
    [Serializable]
    public class ClickEvent : UnityEvent<eMJActType>{}
    public RawImage mBgRing = null ;
    public Sequence mSeq ;
    public eMJActType mActType ;
    public ClickEvent onClick = new ClickEvent() ;
    void Start()
    {
        if ( this.mActType != eMJActType.eMJAct_Pass )
        {
            mSeq = DOTween.Sequence();
            var scale = this.mBgRing.transform.DOScale(new Vector3( 1.2f,1.2f,1),0.9f ).SetEase(Ease.OutCirc) ;
            mSeq.Append(scale);
            var fade = this.mBgRing.DOFade(0,0.9f).SetEase(Ease.OutCirc);
            mSeq.Join(fade);
            mSeq.AppendInterval(0.3f);
            mSeq.AppendCallback(()=>{ this.mBgRing.DOFade(1,0f) ;this.mBgRing.transform.localScale = new Vector3(1,1,1); this.mBgRing.gameObject.SetActive(true);});
            mSeq.SetLoops(-1) ;
        }
        else
        {
            this.mBgRing.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClicked()
    {
        if ( this.onClick != null )
        {
            this.onClick.Invoke(this.mActType);
        } 
    }

    private void OnDestroy() {
        mSeq.Kill();
    }
}
