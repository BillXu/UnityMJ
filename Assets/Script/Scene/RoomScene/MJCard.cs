using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MJCard : MonoBehaviour
{
    // Start is called before the first frame update
    static float MODEL_X_SIZE = 3.393452F ;
    static float MODEL_Y_SIZE = 2.315823F ;
    static float MODEL_Z_SIZE = 4.504555F ;
    
    public enum state
    {
        FACE_COVER,  // 麻将面盖着，就是牌墙的状态
        FACE_UP,   // 麻将面朝上，出了的牌，和碰了的牌那样。
        FACE_USER, // 麻将面朝向用户，通常手牌情况。       
    }
    
    public int cardNum = 0 ;
    
    public eMJCardType mjType 
    {
        get 
        {
            return MJCard.parseCardType(this.cardNum);
        }
    }

    state _state = state.FACE_COVER;

    public state curState { 
        set 
        {
            this._state = value ;
            switch ( this._state )
            {
                case state.FACE_COVER:
                {
                    this.transform.localEulerAngles = Vector3.zero;
                }
                break;
                case state.FACE_UP:
                {
                    this.transform.localEulerAngles = new Vector3(180,0,0);
                }
                break ;
                case state.FACE_USER:
                {
                    this.transform.localEulerAngles = new Vector3(90,0,0);
                }
                break ;
                default:
                {
                    Debug.LogError("unknown state of mj = " + this._state);
                }
                break ;
            }
        }

        get
        { return _state ;}

    }
    public int mjValue
    {
        get 
        {
            return MJCard.parseCardValue(this.cardNum);
        }
    }

    // 把麻将看出一个盒子，以盒子的贴近屏幕的一面，左下角为参考点，世界坐标系 x,y,z 方向的大小，不用考虑麻将状态。
    public float world_x_Size
    {
        get
        {
            return MODEL_X_SIZE ;
        }
    }

    public float world_y_Size
    {
        get
        {
            switch ( this._state )
            {
                case state.FACE_COVER:
                {
                    return MODEL_Y_SIZE ;
                }
                case state.FACE_UP:
                {
                    return MODEL_Y_SIZE ;
                }
                case state.FACE_USER:
                {
                    return MODEL_Z_SIZE ;
                }
                default:
                {
                    Debug.LogError("unknown state of mj = " + this._state);
                }
                break ;
            } 
            return 0 ;           
        }
    }

    public float world_z_Size
    {
        get
        {
            switch ( this._state )
            {
                case state.FACE_COVER:
                {
                    return MODEL_Z_SIZE ;
                }
                case state.FACE_UP:
                {
                    return MODEL_Z_SIZE ;
                }
                case state.FACE_USER:
                {
                    return MODEL_Y_SIZE ;
                }
                default:
                {
                    Debug.LogError("unknown state of mj = " + this._state);
                }
                break ;
            } 
            return 0 ;           
        }
    }

    public static eMJCardType parseCardType( int nCardNum  )
    {
        var nType = nCardNum & 0xF0 ;
        nType = nType >> 4 ;
        if ( ((eMJCardType)nType < eMJCardType.eCT_Max && (eMJCardType)nType > eMJCardType.eCT_None) == false )
        {
            Debug.LogError("parse card type error , cardnum = " + nCardNum) ;
        }

        return (eMJCardType)nType ;
    }

    public static int parseCardValue( int nCardNumer )
    {
        return  (nCardNumer & 0xF) ;
    }
    public static int makeCardNum( eMJCardType type , int val ) 
    {
        return ((int)type << 4) | val ;
    }
}
