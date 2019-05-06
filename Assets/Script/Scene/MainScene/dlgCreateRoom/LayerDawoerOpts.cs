using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
using UnityEngine.UI ;
public class LayerDawoerOpts : MonoBehaviour,ILayerOpts
{
    // Start is called before the first frame update
    DawoerRoomOpts mOpts = new DawoerRoomOpts();
    public JSONObject jsOpts{ get { this.buildOpts() ; return mOpts.toJsOpts();}}
    
    public Toggle mRound8 , mRound16 ;
    public Toggle mSeat4, mSeat3 ;
    public Toggle mPayTypeOwner, mPayTypeAA ;
    public Toggle mRuleOnePao, mRuleOtherPao ;
    public Toggle mGuang, mLimit30, mLimit50,mLimit70,mLimit100;
    public Toggle mRandSeat,mIPAndPos,mForceGPS ;

    public Text mAAPayText ;
    public Text mOtherPaoText ;
    public void onToggleSeat4( bool isOn )
    {
        if ( isOn )
        {
            this.mAAPayText.text = "4家支付" ;
            this.mOtherPaoText.text = "3家炮";  
        }
    }

    public void onToggleSeat3( bool isOn )
    {
        if ( isOn )
        {
            this.mAAPayText.text = "3家支付" ;
            this.mOtherPaoText.text = "2家炮";  
        }
    }

    public void onToggleGuang( bool isOn )
    {
        if ( isOn )
        {
            if ( this.mLimit30.isOn || this.mLimit50.isOn || this.mLimit70.isOn || this.mLimit100.isOn  )
            {

            }
            else
            {
                this.mLimit30.isOn = true ;
            }
        }
        else
        {
            this.mLimit30.group.allowSwitchOff = !this.mLimit30.group.allowSwitchOff ;
            this.mLimit30.isOn = this.mLimit50.isOn = this.mLimit70.isOn = this.mLimit100.isOn = false ;
            this.mLimit30.group.allowSwitchOff = !this.mLimit30.group.allowSwitchOff ;
        }
    }

    public void onToggleLimitFen( bool isOn )
    {
        if ( isOn )
        {
            this.mGuang.isOn = true ;
        }
    }

    public void onToggleIPAndGps( bool isOn )
    {
        this.mForceGPS.gameObject.SetActive(isOn);
        this.mForceGPS.isOn = false ;
    }

    void buildOpts()
    {
        this.mOpts.round = 8 ;
        if ( this.mRound8.isOn )
        {
            this.mOpts.round = 8 ;
        }
        else if ( this.mRound16.isOn )
        {
            this.mOpts.round = 16 ;
        }

        if ( this.mSeat3.isOn )
        {
            this.mOpts.seatCnt = 3 ;
        }
        else if ( this.mSeat4.isOn )
        {
            this.mOpts.seatCnt = 4 ;
        }

        if ( this.mPayTypeAA.isOn )
        {
            this.mOpts.payType = ePayRoomCardType.ePayType_AA ;
        }
        else if ( this.mPayTypeOwner.isOn )
        {
            this.mOpts.payType = ePayRoomCardType.ePayType_RoomOwner ;
        }

        if ( this.mRuleOnePao.isOn )
        {
            this.mOpts.paoType = 1 ;
        }
        else if ( this.mRuleOtherPao.isOn )
        {
            this.mOpts.paoType = 0 ;
        }

        if ( false == this.mGuang.isOn )
        {
            this.mOpts.limitFen = 0 ;
        }
        else
        {
            if ( this.mLimit30.isOn )
            {
                this.mOpts.limitFen = 30 ;
            }
            else if ( this.mLimit50.isOn )
            {
                this.mOpts.limitFen = 50 ;
            }
            else if ( this.mLimit70.isOn )
            {
                this.mOpts.limitFen = 70;
            }
            else if ( this.mLimit100.isOn )
            {
                this.mOpts.limitFen = 100 ;
            }
        }

        this.mOpts.isRandSeat = this.mRandSeat.isOn ;
        this.mOpts.isEnableIPAndGps = this.mIPAndPos.isOn ;
        this.mOpts.isForceGPS = false ;
        if ( this.mIPAndPos.isOn )
        {
            this.mOpts.isForceGPS = this.mForceGPS.isOn ;
        }
    }
}
