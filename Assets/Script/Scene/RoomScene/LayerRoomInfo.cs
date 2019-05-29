using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;
using System;
public class LayerRoomInfo : MonoBehaviour
{
    public Text mRoomID ;
    public Text mBaseScore;
    public Text mRound ;
    public Text mRules ;

    public Text mVersion ;
    public Text mTime ;
    public Text mLeftCardCnt ;
    public List<Transform> mBatteryLevel ;

    public int leftMJCnt
    {
        set
        {
            this.mLeftCardCnt.text = value + "" ;
        }
    }
    // Start is called before the first frame update
    private void Start() {
        this.CancelInvoke();
        this.InvokeRepeating("refreshTime",0,60);
        this.mVersion.text = "ver:" + (null != GameConfig.getInstance() ?  GameConfig.getInstance().VERSION : "null");
        setBatteryLevel(0.6f);
    }
    public void refresh( RoomBaseData data )
    {
        this.mRoomID.text = data.roomID + "" ;
        this.mBaseScore.text = data.baseScore + "";
        this.mRound.text = data.curRound + "/" + data.totalRoundCnt ;
        this.mRules.text = data.rule;
        this.mLeftCardCnt.text = data.leftMJCnt + "" ;
    }

    void refreshTime()
    {
        this.mTime.text = DateTime.Now.ToString("HH:mm");
    }

    void setBatteryLevel( float level  ) // 0 - 1 ;
    {
        int nl = (int)(level * 100 );
        int per = 100 / this.mBatteryLevel.Count ;
        nl = ( nl + per -1 ) / per ;
        Debug.Log("nl = " + nl );
        for (int i = 0; i < this.mBatteryLevel.Count; i++)
        {
            this.mBatteryLevel[i].gameObject.SetActive( i < nl );
        }
    }
}
