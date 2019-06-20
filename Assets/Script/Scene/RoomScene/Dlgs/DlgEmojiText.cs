using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System ;
using UnityEngine.Events ;

public class DlgEmojiText : DlgBase
{
    [Serializable]
    public class DlgResulutEvent : UnityEvent<eChatMsgType,int>
    {

    }
    public DlgResulutEvent onResult = null ;
    float lastSendChatTime = 0 ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onEmoji( int idx )
    {
        if ( Time.fixedTime - this.lastSendChatTime < 4 )
        {
            Prompt.promptText("您说话太频繁，清稍作休息");
            return ;
        }
        this.lastSendChatTime = Time.fixedTime ;
        if ( null != this.onResult )
        {
            this.onResult.Invoke(eChatMsgType.eChatMsg_Emoji,idx ) ;
        }

        this.closeDlg();
    }

    public void onText( int idx )
    {
        if ( Time.fixedTime - this.lastSendChatTime < 4 )
        {
            Prompt.promptText("您说话太频繁，清稍作休息");
            return ;
        }
        this.lastSendChatTime = Time.fixedTime ;

        if ( null != this.onResult )
        {
            this.onResult.Invoke(eChatMsgType.eChatMsg_SysText,idx ) ;
        }
        this.closeDlg();
    }

    public void onEmoji_1()
    {
        this.onEmoji(1);
    }

    public void onEmoji_2()
    {
        this.onEmoji(2);
    }

    public void onEmoji_3()
    {
        this.onEmoji(3);
    }

    public void onEmoji_4()
    {
        this.onEmoji(4);
    }

    public void onEmoji_5()
    {
        this.onEmoji(5);
    }

    public void onEmoji_6()
    {
        this.onEmoji(6);
    }

    public void onEmoji_7()
    {
        this.onEmoji(7);
    }

    public void onEmoji_8()
    {
        this.onEmoji(8);
    }

    public void onEmoji_9()
    {
        this.onEmoji(9);
    }

    public void onEmoji_10()
    {
        this.onEmoji(10);
    }

    public void onEmoji_11()
    {
        this.onEmoji(11);
    }
    public void onEmoji_12()
    {
        this.onEmoji(12);
    }
    public void onEmoji_13()
    {
        this.onEmoji(13);
    }

    public void onEmoji_14()
    {
        this.onEmoji(14);
    }

    public void onEmoji_15()
    {
        this.onEmoji(15);
    }

    public void onEmoji_16()
    {
        this.onEmoji(16);
    }

    //----text
    public void onText_1()
    {
        this.onText(1);
    }

    public void onText_2()
    {
        this.onText(2);
    }

    public void onText_3()
    {
        this.onText(3);
    }

    public void onText_4()
    {
        this.onText(4);
    }
    public void onText_5()
    {
        this.onText(5);
    }

    public void onText_6()
    {
        this.onText(6);
    }

    public void onText_7()
    {
        this.onText(7);
    }
}
