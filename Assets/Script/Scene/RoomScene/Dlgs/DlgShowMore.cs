using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;
public class DlgShowMore : DlgBase
{
    // Start is called before the first frame update
    public enum BtnType
    {
        Btn_Leave,
        Btn_Dismiss,
        Btn_Setting,
        Btn_Max,
    }
    public Button mLeave = null ;
    public Button mDismiss = null ;

    BtnType mClickedBtnType ;
    public BtnType ClickedBtnType
    {
        get
        {
            return this.mClickedBtnType ;
        }

        set
        {
            this.mClickedBtnType = value ;
            if ( this.pFuncResult != null )
            {
                this.pFuncResult(this,null);
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickLeaveRoom()
    {
        this.ClickedBtnType = BtnType.Btn_Leave ;
    }

    public void onClickDismissRoom()
    {
        this.ClickedBtnType = BtnType.Btn_Dismiss ;
    }

    public void onClickSettting()
    {
        this.ClickedBtnType = BtnType.Btn_Setting ;   
    }

    public override void showDlg<T>(ResultCallBack pfResult, T jsUserData, CloseCallBack pfOnClose)
    {
        RoomBaseData da = jsUserData as RoomBaseData;
        this.mLeave.interactable = da.isCanLeaveRoom();
        this.mDismiss.interactable = da.isCanDismissRoom();
        base.showDlg(pfResult,jsUserData,pfOnClose);
    }
}
