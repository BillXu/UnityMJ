using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHoldMing : MonoBehaviour
{
    public MJFactory mMJFactory = null;
    float mRightBounderPos = 0 ;
    List<MJCard> mPengMidle = new List<MJCard>();  // used when bu gang ; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clear()
    {
        while ( this.transform.childCount > 0 )
        {
            var child = this.transform.GetChild(0).GetComponent<MJCard>();
            if ( child == null )
            {
                this.transform.GetChild(0).gameObject.SetActive(false);
                this.transform.GetChild(0).SetParent(null);
                continue;
            }
            this.mMJFactory.recycleCard(child) ;
        }
        this.mRightBounderPos = 0 ;
        this.mPengMidle.Clear();
    }

    public void refresh( List<PlayerActedCard> vCards )
    {
        this.clear();
        if ( vCards == null )
        {
            return ;
        }

        foreach (var item in vCards )
        {
            switch ( item.eAct )
            {
                case eMJActType.eMJAct_AnGang:
                {
                    this.addAnGang(item.nTargetCard) ;
                }
                break;
                case eMJActType.eMJAct_BuGang:
                {
                    this.addPeng(item.nTargetCard,item.eDir);
                    this.addBuGang(item.nTargetCard);
                }
                break;
                case eMJActType.eMJAct_MingGang:
                {
                    this.addMingGang(item.nTargetCard,item.eDir);  
                }
                break;
                case eMJActType.eMJAct_Peng:
                {
                    this.addPeng(item.nTargetCard,item.eDir);
                }
                break;
                case eMJActType.eMJAct_Chi:
                {
                    this.addEat(item.vChiFinalCards[0],item.vChiFinalCards[1],item.vChiFinalCards[2] ) ;
                }
                break;
                default:
                {
                    Debug.LogError("unknown type for refresh act = " + item.eAct );
                }
                break;
            }
        }
    }

    public void addPeng( int nCard , eArrowDirect dir )
    {
        var t = this.mMJFactory.getCard(nCard,this.transform) ;
        t.curState = MJCard.state.FACE_UP;
        if ( dir == eArrowDirect.eDirect_Left )
        {
            t.transform.localEulerAngles = new Vector3(180,90,0) ;
            t.transform.localPosition = new Vector3(this.mRightBounderPos + t.world_z_Size * 0.5f,0,0);
            this.mRightBounderPos += t.world_z_Size;
        }
        else
        {
            t.transform.localPosition = new Vector3(this.mRightBounderPos + t.world_x_Size * 0.5f,0,0);
            this.mRightBounderPos += t.world_x_Size;
        }
        
        // cardB ;
        t = this.mMJFactory.getCard(nCard,this.transform) ;
        t.curState = MJCard.state.FACE_UP;
        t.transform.localPosition = new Vector3(this.mRightBounderPos + t.world_x_Size * 0.5f,0,0);
        this.mRightBounderPos += t.world_x_Size;
        this.mPengMidle.Add(t) ;

        // cardC ;
        t = this.mMJFactory.getCard(nCard,this.transform) ;
        t.curState = MJCard.state.FACE_UP;
        if ( dir == eArrowDirect.eDirect_Righ )
        {
            t.transform.localEulerAngles = new Vector3(180,90,0) ;
            t.transform.localPosition = new Vector3(this.mRightBounderPos + t.world_z_Size * 0.5f,0,0);
            this.mRightBounderPos += t.world_z_Size;
        }
        else
        {
            t.transform.localPosition = new Vector3(this.mRightBounderPos + t.world_x_Size * 0.5f,0,0);
            this.mRightBounderPos += t.world_x_Size;
        }

        // add elapas 
        this.mRightBounderPos += t.world_x_Size * 0.5f;
    }

    public void addMingGang( int nCard , eArrowDirect dir )
    {
        this.addPeng(nCard,dir) ;
        var lastIdx = this.mPengMidle.Count-1;
        var mid = this.mPengMidle[lastIdx];
        this.mPengMidle.RemoveAt(lastIdx);

        var t = this.mMJFactory.getCard(nCard,this.transform) ;
        t.curState = MJCard.state.FACE_UP;
        t.transform.localPosition = new Vector3(mid.transform.localPosition.x,t.world_y_Size,0);
    }

    public void addAnGang( int nCard)
    {
        this.addPeng(nCard,eArrowDirect.eDirect_Opposite) ;
        var lastIdx = this.mPengMidle.Count-1;
        var mid = this.mPengMidle[lastIdx];
        this.mPengMidle.RemoveAt(lastIdx);

        var t = this.mMJFactory.getCard(nCard,this.transform) ;
        t.curState = MJCard.state.FACE_COVER;
        t.transform.localPosition = new Vector3(mid.transform.localPosition.x,t.world_y_Size,0);
    }

    public void addBuGang( int nCard )
    {
        var idx = this.mPengMidle.FindIndex(( MJCard c )=>{ return c.cardNum == nCard ;}) ;
        if ( idx < 0 )
        {
             Debug.LogError("can not find peng ,so can not bu gang card = " + nCard );
             return ;
        }

        var mid = this.mPengMidle[idx];
        this.mPengMidle.RemoveAt(idx);

        var t = this.mMJFactory.getCard(nCard,this.transform) ;
        t.curState = MJCard.state.FACE_UP;
        t.transform.localPosition = new Vector3(mid.transform.localPosition.x,t.world_y_Size,0);
    }

    public void addEat( int aCard , int bCard , int cCard )
    {
        var t = this.mMJFactory.getCard(aCard,this.transform) ;
        t.curState = MJCard.state.FACE_UP;
        t.transform.localEulerAngles = new Vector3(180,90,0) ;
        t.transform.localPosition = new Vector3(this.mRightBounderPos + t.world_z_Size * 0.5f,0,0);
        this.mRightBounderPos += t.world_z_Size;
        
        // cardB ;
        t = this.mMJFactory.getCard(bCard,this.transform) ;
        t.curState = MJCard.state.FACE_UP;
        t.transform.localPosition = new Vector3(this.mRightBounderPos + t.world_x_Size * 0.5f,0,0);
        this.mRightBounderPos += t.world_x_Size;

        // cardC ;
        t = this.mMJFactory.getCard(cCard,this.transform) ;
        t.curState = MJCard.state.FACE_UP;
        t.transform.localPosition = new Vector3(this.mRightBounderPos + t.world_x_Size * 0.5f,0,0);
        this.mRightBounderPos += t.world_x_Size;

        // add elapas 
        this.mRightBounderPos += t.world_x_Size * 0.5f;
    }

    public float getHoldMingSize(){ return this.mRightBounderPos ; }
}
