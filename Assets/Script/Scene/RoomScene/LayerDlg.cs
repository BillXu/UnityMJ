using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
using UnityEngine.UI ;

public class LayerDlg : MonoBehaviour
{
    public RoomScene mScene ;
    public DlgShowMore mDlgShowMore ;
    public Toggle mToggleShowMore ;
    public DlgActButtons mDlgAct ; 
    public DlgEatGangCardsOpts mDlgEatGangOpts ;
    public void onClickLoaction()
    {

    }

    public void onClickCopyRoomNum()
    {

    }

    public void onClickInviteFriends()
    {

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
    public void showDlgAct( JSONArray vjs )
    {
        if ( vjs.Length == 1 && (eMJActType)vjs[0].Number == eMJActType.eMJAct_Chu )
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
}
