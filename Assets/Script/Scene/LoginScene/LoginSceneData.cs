using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
using UnityEngine.SceneManagement ;
public class LoginSceneData : NetBehaviour
{
    // Start is called before the first frame update
    string mCurAccount = "" ;
    string mCurPwd = "";
    string mCurNickName = "" ;
    string mCurHeadIcon = "" ;
    eSex mSex = eSex.eSex_Max;
    
    public bool isWaitRespone = false ;
    private void Start() {
        var us = UserSetting.getInstance() ;
        if ( string.IsNullOrEmpty(us.localAccount) == false && string.IsNullOrEmpty(us.localPassword) == false )
        {
            this.login(us.localAccount,us.localPassword);
        }

        EventDispatcher.getInstance().registerEventHandle(PlayerBaseData.EVENT_RECIEVED_BASE_DATA,this.loginEvent ) ;
        EventDispatcher.getInstance().registerEventHandle(WechatManager.EVENT_RECIEVED_WECHAT_INFO,this.recivedWechatInfoEvent ) ;
    }
    protected override void onConnectResult(bool isSucess)
    {
        if ( isSucess )
        {
            this.doLogin();
        }
        else
        {
            this.onDisconnect();
        }
    }
    protected override void onReconnect(bool isSuccess)
    {
        if ( false == isSuccess )
        {
            this.doLogin();
        }
    }
    protected override void onDisconnect()
    {
        base.onDisconnect();
    }
    public bool login( string acc , string pwd )
    {
        if ( this.isWaitRespone )
        {
            return false ;
        } 

        this.mCurPwd = pwd ;
        this.mCurAccount = acc ;
        this.doLogin();
        return true;
    }
    protected void doLogin()
    {
        if ( string.IsNullOrEmpty(this.mCurAccount) || string.IsNullOrEmpty( this.mCurPwd ) )
        {
            Debug.Log("accout or pwd is null or empty , skip do login");
            return ;
        }

        this.isWaitRespone = true ;
        var msgLogin = new JSONObject();
        msgLogin["cAccount"] = this.mCurAccount ;
        msgLogin["cPassword"] = this.mCurPwd ;
        sendMsg(msgLogin,eMsgType.MSG_PLAYER_LOGIN,eMsgPort.ID_MSG_PORT_GATE,1,
            ( JSONObject jsmg  )=>{
                var ret = (int)jsmg["nRet"].Number ;
                if ( ret == 0 )  // clientData will recieved base data , and invoke loading scene ;
                {
                    // save a valid account , most used when developing state ;
                    UserSetting.getInstance().localAccount = this.mCurAccount ;
                    UserSetting.getInstance().localPassword = this.mCurPwd ;
                    this.isWaitRespone = false ;
                    Debug.Log("login scene login ok");
                    return true ;
                } 
                Debug.Log("login failed try register account = " + this.mCurAccount );
                this.doRegister();
                return true ;
            });
        return ;
    }
    protected void doRegister()
    {
        var msgReg = new JSONObject(); // cName
        msgReg["cAccount"] = this.mCurAccount ;
        msgReg["cPassword"] = this.mCurPwd ;
        msgReg["cRegisterType"] = this.mCurNickName.Length > 0 ;
        msgReg["nChannel"] = 0 ;
        this.sendMsg(msgReg,eMsgType.MSG_PLAYER_REGISTER,eMsgPort.ID_MSG_PORT_GATE,1,
            ( JSONObject jsmg )=>{
                var ret = (int)jsmg["nRet"].Number ;
                if ( ret != 0 )  // clientData will recieved base data , and invoke loading scene ;
                {
                    Prompt.promptText("注册账号失败");
                    this.isWaitRespone = false ;
                    return true;
                } 

                // register ok , then save account info to local ;
                Debug.Log("login scene register ok : " + this.mCurAccount );
                this.login(this.mCurAccount,this.mCurPwd);
                return true ;
            });
    }
    public bool setPlayerDetailInfo( string name , string headIcon , eSex sex )
    {
        this.mCurNickName = name;
        this.mCurHeadIcon = headIcon ;
        this.mSex = sex ;
        return true ;
    }
    public void reqSetPlayerInfo()
    {
        if ( string.IsNullOrEmpty(this.mCurNickName) && string.IsNullOrEmpty(this.mCurHeadIcon) && eSex.eSex_Max == this.mSex )
        {
            return ;
        }

        var msgupdateinfo = new JSONObject() ;
        msgupdateinfo["name"] = this.mCurNickName ;
        msgupdateinfo["headIcon"] = this.mCurHeadIcon;
        msgupdateinfo["sex"] = (int)this.mSex;
        var baseData = ClientPlayerData.getInstance().getComponentData<PlayerBaseData>();
        baseData.name = this.mCurNickName ;
        baseData.headUrl = this.mCurHeadIcon ;
        baseData.gender = this.mSex ;
        this.sendMsg(msgupdateinfo,eMsgType.MSG_PLAYER_UPDATE_INFO,eMsgPort.ID_MSG_PORT_DATA,baseData.uid);
    }
    protected bool loginEvent( EventArg arg )
    {
        if ( PlayerBaseData.EVENT_RECIEVED_BASE_DATA == arg.type )
        {
            this.reqSetPlayerInfo();
            var baseData = ClientPlayerData.getInstance().getComponentData<PlayerBaseData>();
            if ( baseData.stayInRoomID > 0 )
            {
                SceneManager.LoadScene(GameConfig.getInstance().SCENE_NAME_ROOM);
            }
            else
            {
                SceneManager.LoadScene(GameConfig.getInstance().SCENE_NAME_MAIN);
            }
            return false ;
        }
        return false ;
    }
    protected bool recivedWechatInfoEvent( EventArg arg )
    {
        //{ isOk : 1 ,"openid":"ol23Kw11oXU051_Y5Ajkj_L5uOxc","nickname":"技术支持","sex":2,"language":"zh_CN","city":"","province":"Shanghai","country":"CN","headimgurl":"http:\/\/thirdwx.qlogo.cn\/mmopen\/vi_32\/ucIOqQI5mIQfH6Q0fFFONslMFrBibcXCziaQTXmEPjI21JogQibDTibNS6nHa6FgyfNBUTFKPYv7Q4n6aCbeR53xkg\/132","privilege":[],"unionid":"orLF31uNbrRRzsfVXZ54ATq1k694"}
        var js = ((EventWithObject<JSONObject>)arg).argObject;
        if ( ((int)js["isOk"].Number) == 0 )
        {
            Prompt.promptText("获取微信信息失败！");
            return false;
        }
        this.mCurAccount = js["unionid"].Str;
        this.mCurHeadIcon = js["headimgurl"].Str ;
        this.mCurNickName = js["nickname"].Str;
        this.mCurPwd = "1" ;
        this.mSex = (eSex)((int)js["sex"].Number - 1) ;

        this.doLogin();
        return true ;
    }
}
