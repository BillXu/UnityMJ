using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events ;
public class PlayerInfoItem : MonoBehaviour
{
    // Start is called before the first frame update
    public Text mID ;
    public Text mName ;
    public GameObject mMaleIcon ;
    public GameObject mFemalIcon ;
    public NetImage mHeadIcon ;

    public int playerUID 
    {
        get { return int.Parse(mID.text) ;}
        set
        {
            var pdata = PlayerInfoDataCacher.getInstance().getPlayerInfoByID(value) ;
            if ( pdata != null )
            {
                this.onPlayerItemInfo(pdata);
                return ;
            }
            Debug.Log("register player info recived event for player info");
            this.mID.text = value.ToString();
            EventDispatcher.getInstance().registerEventHandle(PlayerInfoDataCacher.EVENT_RECIEVED_PLAYER_INFO_DATA,this.onEvent);
        }
    }

    public void onClickHeadIcon()
    {
        Debug.Log("click head icon");
    }

    public void onPlayerItemInfo( PlayerInfoData data )
    {
        if ( data.uid != this.playerUID )
        {
            return ;
        }
        this.mID.text = data.uid.ToString();
        this.mName.text = data.name ;
        this.mHeadIcon.url = data.headUrl ;
        var isMale = data.gender == eSex.eSex_Male ;
        this.mMaleIcon.SetActive(isMale)  ;
        this.mFemalIcon.SetActive(!isMale);
    }

    bool onEvent( EventArg arg )
    {
        var p = (EventWithObject<PlayerInfoData>)arg ;
        Debug.Log("player info recived event for player info id = " + this.playerUID + "r id =" + p.argObject.uid );
        if ( p.argObject.uid != this.playerUID )
        {
            return false ;
        }

        Debug.Log("do refresh player info recived event for player info");
        this.onPlayerItemInfo(p.argObject);
        EventDispatcher.getInstance().removeEventHandle(PlayerInfoDataCacher.EVENT_RECIEVED_PLAYER_INFO_DATA,this.onEvent);
        return false ;
    }

    private void OnDestroy() {
        EventDispatcher.getInstance().removeEventHandle(PlayerInfoDataCacher.EVENT_RECIEVED_PLAYER_INFO_DATA,this.onEvent);
    }
}
