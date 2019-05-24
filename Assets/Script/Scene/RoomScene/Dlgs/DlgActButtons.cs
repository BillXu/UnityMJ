using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System ;
using Boomlagoon.JSON ;

public class DlgActButtons : MonoBehaviour
{
    [Serializable]
    public class ActResultEvent : UnityEvent<eMJActType>{} 
    
    public ActEffectButton mBtnPeng;
    public ActEffectButton mBtnGang ;
    public ActEffectButton mBtnHu;
    public ActEffectButton mBtnPass ;
    public ActEffectButton mBtnEat ;
    public ActResultEvent onActedResult = null;
    // Start is called before the first frame update
    private void Awake() {
        this.mBtnGang.mActType = eMJActType.eMJAct_MingGang ;
        this.mBtnPeng.mActType = eMJActType.eMJAct_Peng ;
        this.mBtnHu.mActType = eMJActType.eMJAct_Hu ;
        this.mBtnPass.mActType = eMJActType.eMJAct_Pass ;
        this.mBtnEat.mActType = eMJActType.eMJAct_Chi;

        this.mBtnGang.onClick.AddListener(this.onClickAct);
        this.mBtnPeng.onClick.AddListener(this.onClickAct);
        this.mBtnHu.onClick.AddListener(this.onClickAct);
        this.mBtnPass.onClick.AddListener(this.onClickAct);
        this.mBtnEat.onClick.AddListener(this.onClickAct);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showButtons( JSONArray vjs )
    {
        this.gameObject.SetActive(true);
        this.mBtnGang.gameObject.SetActive(false);
        this.mBtnPeng.gameObject.SetActive(false);
        this.mBtnHu.gameObject.SetActive(false);
        this.mBtnEat.gameObject.SetActive(false);

        foreach (var item in vjs)
        {
            var type = (eMJActType)item.Number;
            switch ( type )
            {
                case eMJActType.eMJAct_Chi:
                {
                    this.mBtnEat.gameObject.SetActive(true);
                }
                break;
                case eMJActType.eMJAct_Peng:
                {
                    this.mBtnPeng.gameObject.SetActive(true);
                }
                break ;
                case eMJActType.eMJAct_Hu:
                {
                    this.mBtnHu.gameObject.SetActive(true);
                }
                break ;
                case eMJActType.eMJAct_MingGang:
                case eMJActType.eMJAct_AnGang:
                case eMJActType.eMJAct_BuGang:
                case eMJActType.eMJAct_BuGang_Declare:
                case eMJActType.eMJAct_BuGang_Done:
                {
                    this.mBtnGang.gameObject.SetActive(true);
                    this.mBtnGang.mActType = type ;
                }
                break ;
                default:
                Debug.LogError("unknwon act type = " + type + " to show act buttons" );
                break ;
            }
      
        }
    }
   
    void onClickAct( eMJActType act )
    {
        if ( this.onActedResult != null )
        {
            this.onActedResult.Invoke(act);
        }
        this.gameObject.SetActive(false);
        Debug.Log("click act = " + act.ToString());
    }
}
