using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events ;
using UnityEngine.EventSystems ;
using System ;
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
            this.mID.text = value.ToString();
            var pdata = PlayerInfoDataCacher.getInstance().getPlayerInfoByID(value) ;
            if ( pdata != null )
            {
                this.onPlayerItemInfo(pdata);
                return ;
            }
            Debug.Log("register player info recived event for player info");
            EventDispatcher.getInstance().registerEventHandle(PlayerInfoDataCacher.EVENT_RECIEVED_PLAYER_INFO_DATA,this.onEvent);
        }
    }

    [Serializable]
    public class ClickInfoItemEvent : UnityEvent<int>{} ;
    public ClickInfoItemEvent onClickItem = null ;
    public void clear()
    {
        this.onPlayerItemInfo( null) ;
    }

    private void Awake() {
        mID.text = "0" ;

        var eventTargetObj = this.mHeadIcon.gameObject ;
        if ( eventTargetObj.GetComponent<EventTrigger>() == null )
        {
            var trige = eventTargetObj.AddComponent<EventTrigger>();
            trige.triggers = new List<EventTrigger.Entry>();
            var eng = new EventTrigger.Entry();
            eng.eventID = EventTriggerType.PointerClick;
            var evntAct = new EventTrigger.TriggerEvent();
            evntAct.AddListener((BaseEventData a)=>{ Debug.Log("click head icon"); if ( this.onClickItem != null ){ this.onClickItem.Invoke(this.playerUID);} });
            eng.callback = evntAct;
            trige.triggers.Add(eng);
        }
    }


    public void onPlayerItemInfo( PlayerInfoData data )
    {
        if ( data != null && data.uid != this.playerUID && this.playerUID != 0 )
        {
            return ;
        }
        this.mID.text = data == null ? "0" : data.uid.ToString();
        this.mName.text = data == null ? "" : data.name ;
        this.mHeadIcon.url = data == null ? "" : data.headUrl ;
        var isMale = data == null ? false : (data.gender == eSex.eSex_Male) ;
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
