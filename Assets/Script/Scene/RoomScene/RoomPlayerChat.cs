using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones ;
using UnityEngine.UI ;

public class RoomPlayerChat : MonoBehaviour
{
    // Start is called before the first frame update
    public UnityArmatureComponent mPlayingVoice ;
    public GameObject mVoiceNode ;
    public UnityDragonBonesData mEmojiAniData ;
    public UnityEngine.Transform mEmojiNode = null;
    public Text mChatText ;
    public GameObject mChatTextNode ;
    public Vector3 mChatTextNodePos ;
    public Dictionary<string,UnityArmatureComponent> mEmojis = new Dictionary<string, UnityArmatureComponent>();
    void Start()
    {
        this.mEmojiNode.gameObject.SetActive(true);
        this.mChatTextNodePos = this.mChatTextNode.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    void loadAni()
    {
        if ( this.mEmojis.Count == 0 )
        {
            UnityFactory.factory.LoadData(this.mEmojiAniData,true,1);
            for ( int idx = 1 ; idx <= 16; ++idx )
            {
                var name = string.Format("ChatFace_{0:D2}",idx) ;
                var drag = UnityFactory.factory.BuildArmatureComponent(name,"","","",null,true ) ;
                if ( drag == null )
                {
                    continue ;
                }
                this.mEmojis.Add(name,drag );
                drag.transform.parent = this.mEmojiNode ;
                drag.transform.localPosition = Vector3.zero;
                drag.gameObject.SetActive(false);
            }
        }
    }

    void playVoice()
    {
        this.mPlayingVoice.animation.Play();
    }

    IEnumerator playEmoji( string idx  )
    {
        if ( this.mEmojis.Count == 0 )
        {
            this.loadAni();
        }

        var name = string.Format("ChatFace_{0:D2}",int.Parse(idx)) ;
        if ( this.mEmojis.ContainsKey( name ) == false )
        {
            Debug.LogWarning("unknown emoji = " + name );
            yield break ;
        }


        this.mEmojis[name].gameObject.SetActive(true);        
        this.mEmojis[name].animation.Play(name,1) ;
        var d = this.mEmojis[name].animation.animationConfig.duration ;
        Debug.Log("face = " + name + "time = " + d );
        yield return new WaitForSeconds(1.0f);
        this.mEmojis[name].animation.Stop();
        this.mEmojis[name].gameObject.SetActive(false);    
    }

    IEnumerator playChatText( string idx )
    {
        string content = "unknown" ;
        switch ( int.Parse(idx) )
        {
            case 1: content = "你今天挺有电啊" ; break;
            case 2: content = "你们等会，我有点事！" ; break;
            case 3: content = "别吵吵，都消停的！" ; break;
            case 4: content = "别墨迹，赶紧的！" ; break;
            case 5: content = "我这一手牌挺破的！" ; break;
            case 6: content = "见到你们，我真高兴！" ; break;
            case 7: content = "能不能有点追求,点炮就胡" ; break;
        }

        // unity text bg image update maybe delay , so we need some tricks ;
        this.mChatTextNode.SetActive(true);
        this.mChatTextNode.transform.localPosition = new Vector3(-1000,0,0);
        this.mChatText.text = content ;
        yield return new WaitForEndOfFrame();
        this.mChatTextNode.SetActive(false);
        yield return new WaitForEndOfFrame();
        this.mChatTextNode.SetActive(true);
        this.mChatTextNode.transform.localPosition = this.mChatTextNodePos;
        yield return new WaitForSeconds(2.5f);
        this.mChatTextNode.SetActive(false);
    }

    public void onPlayerChatMsg( eChatMsgType type , string strContent )
    { 
        switch ( type )
        {
            case eChatMsgType.eChatMsg_Emoji:
            {
                StartCoroutine(this.playEmoji(strContent)) ;
            }
            break ;
            case eChatMsgType.eChatMsg_SysText:
            {
                StartCoroutine(this.playChatText(strContent)) ;
            }
            break ;
            case eChatMsgType.eChatMsg_Voice:
            {
                if ( strContent == "begin" )
                {
                    this.mPlayingVoice.animation.Play();
                    this.mVoiceNode.SetActive(true);
                }
                else
                {
                    this.mPlayingVoice.animation.Stop();
                    this.mVoiceNode.SetActive(false);
                }
            }
            break ;
            default:
            Debug.LogError("unknown chat type = " + type + " content = " + strContent );
            break ;
        }
    }
}
