using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using UnityEngine.Events ;
using UnityEngine.EventSystems ;
using UnityEngine.UI;
using DG.Tweening;
public class DlgBase : MonoBehaviour
{
    public GameObject pRootNode = null ;
    public GameObject pBgImgArea = null;
    public bool isPopout = false ; // show dlg effect
    public bool isClickOutSideClose = false ;
    public bool isMaskBg = false ;

    public delegate void ResultCallBack ( DlgBase dlg , JSONObject jsResult ) ;
    public delegate void CloseCallBack( DlgBase pTargetDlg );
    protected ResultCallBack pFuncResult = null ;
    protected CloseCallBack  pOnCloseCallBack = null ;
    // LIFE-CYCLE CALLBACKS:
    private void Awake() {
        if ( this.pRootNode == null )
        {
            this.pRootNode = this.gameObject ;
        }

        if ( null == this.pBgImgArea )
        {
            this.pBgImgArea = this.pRootNode ;
        }

        if ( this.pBgImgArea != null )
        {
           var img = this.pBgImgArea.GetComponent<Image>();
           if ( null != img )
           {
               img.raycastTarget = true ;
           }

           var imgR = this.pBgImgArea.GetComponent<RawImage>();
           if ( null != imgR )
           {
               imgR.raycastTarget = true ;
           }
        //    var trigger = this.pBgImgArea.GetComponent<EventTrigger>();
        //    if ( trigger == null )
        //    {
        //         trigger = this.pBgImgArea.AddComponent<EventTrigger>();
        //    }
        }
    
        var g = new GameObject();
        var tract= g.AddComponent<RectTransform>();
        var rawImg = g.AddComponent<RawImage>();
        rawImg.raycastTarget = true ;
        tract.SetParent(this.pRootNode.transform) ;
        tract.SetAsFirstSibling();
        tract.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,1700);
        tract.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,1700);
        tract.localPosition = new Vector3(0,0);
        rawImg.color = this.isMaskBg ? ( new Color(0f,0f,0f,0.5f)) : Color.clear;

        if ( this.isClickOutSideClose )
        {
            var trige = g.AddComponent<EventTrigger>();
            trige.triggers = new List<EventTrigger.Entry>();
            var eng = new EventTrigger.Entry();
            eng.eventID = EventTriggerType.PointerClick;
            var evntAct = new EventTrigger.TriggerEvent();
            evntAct.AddListener((BaseEventData a)=>{this.closeDlg();});
            eng.callback = evntAct;
            trige.triggers.Add(eng);
        }

    }
    public virtual void showDlg<T>( ResultCallBack pfResult, T jsUserData, CloseCallBack pfOnClose )
    {
        this.pFuncResult = pfResult ;
        this.pOnCloseCallBack = pfOnClose ;
        this.pRootNode.SetActive(true) ;
        Debug.Log( " super is showDlg ");
        if ( this.isPopout )
        {
            this.pBgImgArea.transform.DOScale(0.8f,0.5f).From().SetEase(Ease.OutBounce);
        }
    }

    public void closeDlg()
    {
        this.pRootNode.SetActive( false ) ;
        if ( this.pOnCloseCallBack != null )
        {
            this.pOnCloseCallBack(this);
        }
        this.pFuncResult = null ;
        this.pOnCloseCallBack = null ;
    }
}
