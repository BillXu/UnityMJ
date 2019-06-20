using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
using UnityEngine.UI ;
using UnityEngine.EventSystems ;
public class LayerDlg : MonoBehaviour
{
    public GameObject mBtnInviteFirends;
    public GameObject mBtnCopyRoomID ;
    public RoomScene mScene ;
    public DlgShowMore mDlgShowMore ;
    public Toggle mToggleShowMore ;
    public DlgActButtons mDlgAct ; 
    public DlgEatGangCardsOpts mDlgEatGangOpts ;
    public DlgResultSingle mDlgResultSingle ;
    public DlgDismissRoom mDlgDismissRoom ;
    public DlgResultTotal mDlgResultTotal ;
    public DlgVoiceRecording mDlgVoiceRecording ;

    public DlgEmojiText mDlgEmojiText ;

    public void refresh( RoomData data )
    {
        if ( data.mBaseData.isDuringGame() )
        {
            this.mBtnCopyRoomID.SetActive(false);
            this.mBtnInviteFirends.SetActive(false);
            this.mDlgResultSingle.close();
            this.mDlgEatGangOpts.close();
            this.mDlgAct.close();
        }

        if ( data.mBaseData.applyDismissIdx >= 0 )
        {
            this.showDlgDismiss(data);
        }
    }
    public void onGameStart()
    {
        this.mBtnCopyRoomID.SetActive(false);
        this.mBtnInviteFirends.SetActive(false);
        this.mDlgResultSingle.close();
        this.mDlgEatGangOpts.close();
        this.mDlgAct.close();
    }
    public void onClickLoaction()
    {
        Prompt.promptText( "Loaction" );
    }

    public void onClickCopyRoomNum()
    {
        Prompt.promptText( "CopyRoomID" );
    }

    public void onClickInviteFriends()
    {
        Prompt.promptText( "invite firends" );
    }

    public void onClickShowMore( bool isShow )
    {
        if ( isShow )
        {
            this.mDlgShowMore.showDlg( this.onDlgShowMoreResult,this.mScene.mRoomData.mBaseData,(DlgBase d )=>{ if ( this.mToggleShowMore.isOn ) this.mToggleShowMore.isOn = false ;});
        } 
        else
        {
            this.mDlgShowMore.closeDlg();
        }
    }

    public void onDlgShowMoreResult( DlgBase d , JSONObject js )
    {
        var clickBtn = this.mDlgShowMore.ClickedBtnType;
        switch ( clickBtn )
        {
            case DlgShowMore.BtnType.Btn_Leave:
            {
                this.mScene.mRoomData.onPlayerApplyLeave();
            }
            break;
            case DlgShowMore.BtnType.Btn_Dismiss:
            {
                this.mScene.mRoomData.onPlayerApplyDismissRoom();
            }
            break ;
            case DlgShowMore.BtnType.Btn_Setting:
            {
                Debug.Log( "show settting dlg , clicked btn " + clickBtn );
            }
            break ;
        }
 
    }
    public void showDlgAct( JSONArray vjs, bool isRecivedCard )
    {
        if ( isRecivedCard == false && vjs.Length == 1 && (eMJActType)vjs[0].Number == eMJActType.eMJAct_Chu )
        {
            Debug.LogWarning("one opt need not show act buttons ");
            return ;
        }

        if ( isRecivedCard && vjs.Length == 1 && (eMJActType)vjs[0].Obj["act"].Number == eMJActType.eMJAct_Chu )
        {
            Debug.LogWarning("one opt need not show act buttons ");
            return ;
        }

        this.mDlgAct.showButtons(vjs) ;
    }
    public void onDlgActResult( eMJActType act )
    {
        this.mScene.mRoomData.onPlayerChosedAct(act,0);
    }

    public void showEatOpts( List<eEatType> vEatOpts , int nTargetCard )
    {
        this.mDlgEatGangOpts.showEatOpts(vEatOpts,nTargetCard);
    }

    public void showGangOpts( List<int> gangs )
    {
        this.mDlgEatGangOpts.showGangOpts(gangs);
    }
    public void onChoseGangResult( int card )
    {
        this.mScene.mRoomData.onPlayerChosedGangCard(card);
    }

    public void onChoseEatResult( eEatType type  )
    {
        this.mScene.mRoomData.onPlayerChoseEatType(type);
    }
    
    public void showDlgResultSingle( ResultSingleData data )
    {
        this.mDlgResultSingle.showResult(data);
        this.mDlgResultSingle.selfIdx = this.mScene.mRoomData.getSelfIdx();
        this.mDlgResultSingle.isFinal = false ;
    }
    public void onResultSingleNextGoOn()
    {
        if ( this.mScene.mRoomData.mTotalResultData.isRecived() )
        {
            this.mDlgResultTotal.showDlg(null,this.mScene.mRoomData,null) ;
        }
        else
        {
            this.mScene.mRoomData.onPlayerReady();
        }
        
    }

    public void showDlgDismiss( RoomData data )
    {
        this.mDlgDismissRoom.showDlg(null,data,null ) ;
    }

    public void closeDlgDismiss()
    {
        this.mDlgDismissRoom.closeDlg();
    }

    public void onReplayDismiss( int idx , bool isAgree )
    {
        this.mDlgDismissRoom.onPlayerReply(idx,isAgree );
    }

    public void onDlgDismissResult( bool isAgree )
    {
        this.mScene.mRoomData.onPlayerReplyDismiss( isAgree ) ;
    }

    public void showDlgResultTotal( RoomData data )
    {
        if ( this.mDlgResultSingle.isShow() == false )
        {
            this.mDlgResultTotal.showDlg(null,data,null) ;
        }
        else
        {
            this.mDlgResultSingle.isFinal = true ;
        }
        
    }

    public void onRecivedPlayerBrifeData( PlayerInfoData infoData )
    {
        if ( this.mDlgDismissRoom.isShowDlg() )
        {
            this.mDlgDismissRoom.onRecivedPlayerBrifeData(infoData);
        }
        
    }

    // button Voice 
    public void onButtonVoiceDown( BaseEventData eventData )
    {
        // show dlg ;
        if ( VoiceManager.getInstance().canStartRecorder() == false )
        {
            Prompt.promptDlg("系统正常处理语音，稍等一下");
            return ;
        }

        this.mDlgVoiceRecording.showDlg();
        VoiceManager.getInstance().startRecord();
    }

    public void onButtonVoiceResult( bool isCanncel )
    {
        if ( this.mDlgVoiceRecording.isShow() == false )
        {
            return ;
        }
        // close dlg ;
        this.mDlgVoiceRecording.closeDlg();
        VoiceManager.getInstance().stopRecord( isCanncel == false ) ;
    }

    public void onDlgVoiceRecordingTimeOut()
    {
        VoiceManager.getInstance().stopRecord(true) ;
        Prompt.promptDlg("录语超过时长自动结束并发送了");
    }

    public void showDlgEmojiText()
    {
        this.mDlgEmojiText.showDlg(null,0,null);
    }

    public void onDlgEmojiText( eChatMsgType type , int idx )
    {
        this.mScene.mRoomData.sendPlayerChat(type, idx.ToString() ) ;
    }
}
