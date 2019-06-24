using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;
using System ;
using UnityEngine.Events ;
public class DlgPlayerInfo : DlgBase
{
    // Start is called before the first frame update
    public PlayerInfoItem mInfo ;
    public Text mAddress ;
    public Text mIP ;
    [Serializable]
    public class ResultEvent : UnityEvent<int,int>{} ;
    public ResultEvent onResult = null;
    public float mLastActTime = 0 ;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onReplayVoice()
    {
        if ( this.onResult != null )
        {
            this.onResult.Invoke( this.mInfo.playerUID,-1);
        }
    }

    public void onClickItem( int idx )
    {
        if ( Time.fixedTime - this.mLastActTime < 4 )
        {
            Prompt.promptText("请休息再发，不能太频繁!");
            return ;
        }
        this.mLastActTime = Time.fixedTime ;

        if ( this.onResult != null )
        {
            this.onResult.Invoke( this.mInfo.playerUID, idx);
        }
    }


    public override void showDlg<T>(ResultCallBack pfResult, T jsUserData, CloseCallBack pfOnClose)
    {
        base.showDlg(pfResult,jsUserData,pfOnClose);
        var pdata = jsUserData as RoomPlayerData ;
        this.mInfo.playerUID = pdata.nUID ;
        var detail = PlayerInfoDataCacher.getInstance().getPlayerInfoByID(pdata.nUID);
        if ( detail == null )
        {
            return ;
        }

        this.refresh(detail);
    }

    public void refresh( PlayerInfoData detail )
    {
        if ( this.mInfo.playerUID != detail.uid )
        {
            return ;
        }
        this.mIP.text = "IP:    " + detail.ip;
        this.mAddress.text = "   位置: " + detail.address ;
    }

    public void clickItem_1()
    {
        this.onClickItem(1);
    }

    public void clickItem_2()
    {
        this.onClickItem(2);
    }

    public void clickItem_3()
    {
        this.onClickItem(3);
    }

    public void clickItem_4()
    {
        this.onClickItem(4);
    }

    public void clickItem_5()
    {
        this.onClickItem(5);
    }

    public void clickItem_6()
    {
        this.onClickItem(6);
    }

    public void clickItem_7()
    {
        this.onClickItem(7);
    }

    public void clickItem_8()
    {
        this.onClickItem(8);
    }

}
