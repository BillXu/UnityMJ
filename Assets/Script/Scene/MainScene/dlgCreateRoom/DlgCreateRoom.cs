using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DlgCreateRoom : DlgBase
{
    // Start is called before the first frame update
    public LayerDawoerOpts mDawoerLayer ;
    ILayerOpts mCurLayer = null ;
    public void onToggleDanDong( bool isOn )
    {
        if ( isOn == false )
        {
            return ;
        }

        this.mCurLayer = null ;
        Debug.Log("onToggleDanDong = " + isOn );
    }

    public void onToggleMoQi( bool isOn )
    {
        if ( isOn == false )
        {
            return ;
        }
        this.mCurLayer = null ;
    }

    public void onToggleShiSiLuo( bool isOn )
    {
        if ( isOn == false )
        {
            return ;
        }
        this.mCurLayer = null ;
    }

    public void onToggleDaWoEr( bool isOn )
    {
        if ( isOn == false )
        {
            return ;
        }
        this.mCurLayer = mDawoerLayer ;
    }

    public void onToggleDouDiZhu( bool isOn )
    {
        if ( isOn == false )
        {
            return ;
        }
        this.mCurLayer = null ;
    }

    public void onDoCreate()
    {
        if ( null == this.mCurLayer )
        {
            Prompt.promptDlg("当前麻将未开通") ;
            return ;
        }

        if ( this.pFuncResult != null )
        {
            this.pFuncResult(this,this.mCurLayer.jsOpts) ;
        }
    }
}
