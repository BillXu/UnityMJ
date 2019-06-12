using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
using UnityEngine.Networking ;
public enum eWechatShareDestType
{
    eDest_Firend = 1,
    eDest_TimeLine = 2,
    eDest_Max,
} ;
public class WechatManager : SingletonBehaviour<WechatManager>
{
    private string APP_ID = "wx7858ac6ee1b0b978" ;
    private string APP_KEY = "b7b23107433cb9ac13bf46d3151f8011" ;
    private string access_token = "" ;
 
    static string EVENT_WECHAT_CODE = "EVENT_WECHAT_CODE" ; // { errorCode : 0 , code : "" } 
    static string EVENT_SHARE_RESULT = "EVENT_SHARE_RESULT" ; // { isOk : 0 , actFlag : "" } 
    static string EVENT_RECIEVED_WECHAT_INFO = "EVENT_RECIEVED_WECHAT_INFO" ; //{ isOK : 1 ,"openid":"ol23Kw11oXU051_Y5Ajkj_L5uOxc","nickname":"技术支持","sex":2,"language":"zh_CN","city":"","province":"Shanghai","country":"CN","headimgurl":"http:\/\/thirdwx.qlogo.cn\/mmopen\/vi_32\/ucIOqQI5mIQfH6Q0fFFONslMFrBibcXCziaQTXmEPjI21JogQibDTibNS6nHa6FgyfNBUTFKPYv7Q4n6aCbeR53xkg\/132","privilege":[],"unionid":"orLF31uNbrRRzsfVXZ54ATq1k694"}
    static string EVENT_WECHAT_SHARE_RESULT = "EVENT_WECHAT_SHARE_RESULT"; // { isOk : 0 , actionTag : "" }

    static string SDK_WECHAT_INIT = "SDK_WECHAT_INIT" ; // { appID : "dsjfa" }
    static string SDK_WECHAT_AUTHOR = "SDK_WECHAT_AUTHOR" ; // {} 
    static string SDK_WECHAT_SHARE_TEXT = "SDK_WECHAT_SHARE_TEXT" ; // { strContent : "aa", type : eWechatShareDestType, actionTag : "aa" }
    static string SDK_WECHAT_SHARE_LINK = "SDK_WECHAT_SHARE_LINK" ; // { strLink : "http://abc.com", type : eWechatShareDestType, strTitle : "hello", strDesc : "desc", actionTag : "tag"}
    static string SDK_WECHAT_SHARE_IMAGE = "SDK_WECHAT_SHARE_IMAGE" ; // { file : "c://hello/a.png" , type : eWechatShareDestType , actionTag : "aa" }

    bool mIsInit = false ;
    string mCode = "";
    private void Start() {
        if ( false == this.mIsInit )
        {
            this.init();
            this.mIsInit = true ;
        }
    }
    private void init()
    {
        this.registerEvent();

        var js = new JSONObject();
        js["appID"] = this.APP_ID;
        SDKHelp.sendRequestToPlatform(WechatManager.SDK_WECHAT_INIT,js ) ;
        return ;
    }

    private void registerEvent()
    {
        EventDispatcher.getInstance().registerEventHandle(WechatManager.EVENT_WECHAT_CODE,this.onEvent );
    }

    void unregisterEvent()
    {
        EventDispatcher.getInstance().removeEventHandleByTarget(this);
    }

    public void reqAuthor()
    {
        SDKHelp.sendRequestToPlatform(WechatManager.SDK_WECHAT_AUTHOR, null) ;
        return ;
    }

    bool onEvent( EventArg arg )
    {
        var eventID = arg.type;
        if ( eventID == WechatManager.EVENT_WECHAT_CODE )
        {
            var p = (EventWithObject<JSONObject>)arg ;
            if (  p.argObject.ContainsKey("errorCode") && ((int)p.argObject["errorCode"].Number) != 0 )
            {
                var detail = new JSONObject();
                detail["isOk"] = 0 ;
                EventDispatcher.getInstance().dispatch( WechatManager.EVENT_RECIEVED_WECHAT_INFO,detail );
                Debug.LogWarning( "获取授权失败code="+ p.argObject["errorCode"].Str);
                return true;
            }
            else
            {
                this.mCode = p.argObject["code"].Str;
                this.StartCoroutine("requestAccessToken");
            }
        }
        return false ;
    }

    private IEnumerator requestAccessToken()
    {
        var url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + this.APP_ID + "&secret="+this.APP_KEY+"&code="+this.mCode+"&grant_type=authorization_code" ;
        var webRequest = UnityWebRequest.Get(url) ;
        yield return webRequest.SendWebRequest();
        if ( webRequest.isNetworkError || webRequest.isHttpError )
        {
            Debug.LogError(webRequest.error) ;
            yield break ;
        }
        
        var str = webRequest.downloadHandler.text ;
        var js = JSONObject.Parse(str) ;
        if ( null == js || js.ContainsKey("access_token") == false )
        {
            Debug.LogError("result data parse js error");
            var detail = new JSONObject();
            detail["isOk"] = 0 ;
            EventDispatcher.getInstance().dispatch( WechatManager.EVENT_RECIEVED_WECHAT_INFO,detail );
            Debug.LogWarning( "获取授权失败code=");
            yield break ;
        }
        else
        {
            url = "https://api.weixin.qq.com/sns/userinfo?access_token="+js["access_token"].Str+"&openid="+js["openid"].Str ;
            var webRequestInfo = UnityWebRequest.Get(url) ;
            yield return webRequestInfo.SendWebRequest();
            str = webRequestInfo.downloadHandler.text ;
            var jsInfo = JSONObject.Parse(str) ;
            Debug.Log("info detail = " + str );
            if ( jsInfo == null || jsInfo.ContainsKey("nickname") == false )
            {
                var detailFailed = new JSONObject();
                detailFailed["isOk"] = 0 ;
                EventDispatcher.getInstance().dispatch( WechatManager.EVENT_RECIEVED_WECHAT_INFO,detailFailed );
                Debug.LogWarning("获取用户信息失败code=" + (( jsInfo == null ) ? "unknown" : jsInfo["errmsg"]) );
                yield break ;
            }

            jsInfo["isOk"] = 1 ;
            EventDispatcher.getInstance().dispatch( WechatManager.EVENT_RECIEVED_WECHAT_INFO,jsInfo );
            Debug.LogWarning("获取用户信息成功");

            //{"openid":"ol23Kw11oXU051_Y5Ajkj_L5uOxc","nickname":"技术支持","sex":2,"language":"zh_CN","city":"","province":"Shanghai","country":"CN","headimgurl":"http:\/\/thirdwx.qlogo.cn\/mmopen\/vi_32\/ucIOqQI5mIQfH6Q0fFFONslMFrBibcXCziaQTXmEPjI21JogQibDTibNS6nHa6FgyfNBUTFKPYv7Q4n6aCbeR53xkg\/132","privilege":[],"unionid":"orLF31uNbrRRzsfVXZ54ATq1k694"}
        }
    }

    public void shareTextWechat( string strContent,eWechatShareDestType type , string strTitle , string actionTag = "defaultTag" )
    {
        var jsArg = new JSONObject() ;
        jsArg["strContent"] = strContent;
        jsArg["type"] = (int)type;
        jsArg["actionTag"] = actionTag ;
        SDKHelp.sendRequestToPlatform(WechatManager.SDK_WECHAT_SHARE_TEXT,jsArg) ;
        return ;
    }

    public void shareLinkWechat( string strLink,eWechatShareDestType type, string strTitle, string strDesc, string actionTag = "defaultTag" )
    {
        var jsArg = new JSONObject() ;
        jsArg["strLink"] = strLink;
        jsArg["type"] = (int)type;
        jsArg["strTitle"] = strTitle ;
        jsArg["strDesc"] = strDesc ;
        jsArg["actionTag"] = actionTag ;
        SDKHelp.sendRequestToPlatform(WechatManager.SDK_WECHAT_SHARE_LINK,jsArg) ;
        return ;
    }

    // shareImageWechat( pCaptureNode: cc.Node ,type : eWechatShareDestType, actionTag : string = "defaultTag" )
    // {
    //     let strPathFile = "" ;
    //     if ( CC_JSB )
    //     {
    //         strPathFile = jsb.fileUtils.getWritablePath() + "screen_shoot.png" ;
    //     }

    //     this.captureScreen(pCaptureNode,strPathFile) ;

    //     sendRequestToPlatform(WechatManager.SDK_WECHAT_SHARE_IMAGE,{ file : strPathFile,type : type , actionTag : actionTag}) ;
    //     return ;
    // }

    // public captureScreen(parent: cc.Node, filePath: string)
    // {
    //     if ( cc.sys.isNative == false )
    //     {
    //         console.warn( "platform is not native , so can not capture screen" );
    //         return false ;
    //     }

    //     if ( parent == null )
    //     {
    //         console.error( "capture node is null" );
    //         return ;
    //     }

    //     console.log( "captureScreen(parent: cc.Node, filePath: string)" );

    //     let node = new cc.Node();
    //     node.setParent(parent);
    //     node.width = parent.getContentSize().width;
    //     node.height = parent.getContentSize().height;
    //     let camera = node.addComponent(cc.Camera);
    //     // 新建一个 RenderTexture，并且设置 camera 的 targetTexture 为新建的 RenderTexture，这样 camera 的内容将会渲染到新建的 RenderTexture 中。
    //     let texture = new cc.RenderTexture();
    //     texture.initWithSize(node.width, node.height);
    //     camera.targetTexture = texture;

    //     // 渲染一次摄像机，即更新一次内容到 RenderTexture 中
    //     parent.scaleY = -1;
    //     camera.render(parent);
    //     parent.scaleY = 1;

    //     // 这样我们就能从 RenderTexture 中获取到数据了
    //     let data = texture.readPixels();
    //     let width = texture.width;
    //     let height = texture.height;
    //     let fullPath = filePath;
    //     if (jsb.fileUtils.isFileExist(fullPath)) {
    //     jsb.fileUtils.removeFile(fullPath);
    //     }

    //     return jsb.saveImageData(data, width, height, fullPath);
    // }
}
