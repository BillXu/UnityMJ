using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones ;
public class EffectPlayerActResult : MonoBehaviour
{
    // Start is called before the first frame update
    public UnityArmatureComponent mEat ;
    public UnityArmatureComponent mPeng;
    public UnityArmatureComponent mGang;
    public UnityArmatureComponent mHu ;
    void Start()
    {
        this.mEat.AddEventListener(DragonBones.EventObject.COMPLETE,this.onAnimationComplete);
        this.mPeng.AddEventListener(DragonBones.EventObject.COMPLETE,this.onAnimationComplete);
        this.mGang.AddEventListener(DragonBones.EventObject.COMPLETE,this.onAnimationComplete);
        this.mHu.AddEventListener(DragonBones.EventObject.COMPLETE,this.onAnimationComplete);

        this.playActEffect(eMJActType.eMJAct_Hu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playActEffect( eMJActType act )
    {
        switch ( act )
        {
            case eMJActType.eMJAct_Chi:
            {
                this.mEat.gameObject.SetActive(true);
                this.mEat.animation.Play(null,1);
            }
            break;
            case eMJActType.eMJAct_Peng:
            {
                this.mPeng.gameObject.SetActive(true);
                this.mPeng.animation.Play(null,1);
            }
            break ;
            case eMJActType.eMJAct_Hu:
            {
                this.mHu.gameObject.SetActive(true);
                this.mHu.animation.Play(null,1);
            }
            break ;
            case eMJActType.eMJAct_MingGang:
            case eMJActType.eMJAct_AnGang:
            case eMJActType.eMJAct_BuGang:
            case eMJActType.eMJAct_BuGang_Declare:
            case eMJActType.eMJAct_BuGang_Done:
            {
                this.mGang.gameObject.SetActive(true);
                this.mGang.animation.Play(null,1);
            }
            break ;
            default:
            Debug.LogError("unknwon act type = " + act + " to play act result effect" );
            break ;
        }
    }

    void onAnimationComplete( string type, EventObject v )
    {
        Debug.Log("animation finished type = " + type + " name = " + v.animationState.name );
        switch ( v.animationState.name )
        {
            case "texiaochi":
            {
                this.mEat.gameObject.SetActive(false);
            }
            break;
            case "texiaopeng":
            {
                this.mPeng.gameObject.SetActive(false);
            }
            break;
            case "texiaogang":
            {
                this.mGang.gameObject.SetActive(false);
            }
            break;
            case "texiaohu":
            {
                this.mHu.gameObject.SetActive(false);
            }
            break;
            default:
            Debug.LogError("unknwon name = " + v.animationState.name );
            break ;
        }
 
    }
}
