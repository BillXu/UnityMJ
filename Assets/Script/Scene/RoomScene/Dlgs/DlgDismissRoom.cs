using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;
using System ;
using UnityEngine.Events ;
public class DlgDismissRoom : DlgBase
{
    [Serializable]
    public class ReplyEvent : UnityEvent<bool>{} ;
    public Text mDesc ;
    public Text mTimeCountDown ;
    public List<RawImage> mAgreeIcons ;
    public List<RawImage> mWaitIcons ;

    public List<PlayerInfoItem> mPlayers ;
    public ReplyEvent onReply = null;
    public GameObject mActButtonParent ;
    int mLeftTime = 0;
    int mApplyUID = 0 ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onPlayerReply( int idx , bool isAgree )
    {
        this.mAgreeIcons[idx].gameObject.SetActive(isAgree);
        this.mWaitIcons[idx].gameObject.SetActive(false);
        if ( isAgree == false )
        {
            Prompt.promptText( this.mPlayers[idx].mName.text + "拒绝解散房间");
            this.closeDlg();
            return ;
        }
    }

    public override void showDlg<T>( ResultCallBack pfResult, T jsUserData, CloseCallBack pfOnClose )
    {
        RoomData data = jsUserData as RoomData ;
        var appyer = data.mPlayers[data.mBaseData.applyDismissIdx];
        var applyerName = ""+appyer.nUID ;
        this.mApplyUID = appyer.nUID ;
        var pInfo = PlayerInfoDataCacher.getInstance().getPlayerInfoByID( appyer.nUID );
        if ( pInfo != null )
        {
            applyerName = pInfo.name;
        }
        this.mDesc.text = "用户【" + applyerName + "】请求解散房间，是否同意？\n(超过300秒默认同意)" ;

        // fill player info ;
        for ( int i = 0 ; i < 4 ; ++i )
        {
            var isEmpty = data.mPlayers[i] == null || data.mPlayers[i].isEmpty();
            this.mPlayers[i].gameObject.SetActive( isEmpty == false );
            if ( isEmpty )
            {
                continue;
            }

            var isAgre = data.mBaseData.applyDismissIdx == i || data.mBaseData.agreeDismissIdx.Contains(i) ;
            this.mAgreeIcons[i].gameObject.SetActive( isAgre );
            this.mWaitIcons[i].gameObject.SetActive( !isAgre );
            this.mPlayers[i].playerUID = data.mPlayers[i].nUID ;
        }

        setLeftTime(data.mBaseData.dimissRoomLeftTime);
        this.mActButtonParent.SetActive( data.mBaseData.agreeDismissIdx.Contains(data.getSelfIdx()) == false );
        // set count down ;
        base.showDlg(pfResult,jsUserData,pfOnClose);
    }

    void setLeftTime( int leftTime )
    {
        this.mLeftTime = leftTime ;
        this.mLeftTime += 1 ;// becauce immidiately invoke ;
        CancelInvoke();
        InvokeRepeating("timer",0,1);
    }

    void timer()
    {
        --this.mLeftTime;
        this.mTimeCountDown.text = "" + this.mLeftTime ;
    }

    public void onClickRefuse()
    {
        if ( this.onReply != null )
        {
            this.onReply.Invoke(false);
        }
        closeDlg();
    }

    public void onClickAgree()
    {
        if ( this.onReply != null )
        {
            this.onReply.Invoke(true);
        }

        this.mActButtonParent.SetActive(false);
    }

    public override void closeDlg()
    {
        CancelInvoke();
        base.closeDlg();
        this.mApplyUID = 0 ;
    }

    public void onRecivedPlayerBrifeData( PlayerInfoData infoData )
    {
        if ( this.mApplyUID == infoData.uid )
        {
            this.mDesc.text = "用户【" + infoData.name + "】请求解散房间，是否同意？\n(超过300秒默认同意)" ;
        }
    } 
}
