using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;

public class PromptDlg : DlgBase
{
    public Text pLabel = null;

    public GameObject pBtnCanncel = null ;
    public GameObject  pBtnOk = null ;
    public bool isOneBtn = true ;

    public override void showDlg<T>( ResultCallBack pfResult, T jsUserData, CloseCallBack pfOnClose )
    {
        if ( jsUserData == null )
        {
            Debug.LogError( "prompt dlg userdata can not be null" );
            return ;
        }

        var txt = "use data for prompt is not string , error " ;
        if ( false == typeof(T).Equals(typeof(string)) )
        {
            Debug.LogError(" promopt dlg is only accept string type as user data ");
        }
        else
        {
            txt = jsUserData as string ;
        }
        this.pLabel.text = txt;

        base.showDlg(pfResult,jsUserData,pfOnClose) ;
        this.pBtnCanncel.SetActive(!this.isOneBtn);
        this.pBtnOk.SetActive(this.isOneBtn);
    }

    public void onClickOk()
    {
        if ( this.pFuncResult != null )
        {
            this.pFuncResult(this,null);
            this.pOnCloseCallBack = null ; // do not invoker close ;invoker already known dlg closed ; 
        }
        this.closeDlg();
    }

    public void onClickCanncel()
    {
        this.closeDlg();
    }
}
