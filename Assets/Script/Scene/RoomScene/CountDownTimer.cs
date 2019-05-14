using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;

public class CountDownTimer : MonoBehaviour
{
    public Text mText = null ;
    public int selfIdx
    {
        set
        {
            this.mText.transform.Rotate(0,0, 90 * value );
        }
    }

    public bool mIsDecreseTime = true ;
    int _curTime = 0 ;
    public int mCurTime 
    {
        set
        {
            this._curTime = value ;
            this.mText.text = string.Format("{0:D2}",value) ;
            this.CancelInvoke();
            this.InvokeRepeating("timeRepeat",1,1);
        } 

        get 
        {
            return this._curTime ;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        this.mCurTime = 89;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void timeRepeat()
    {
        if ( this.mIsDecreseTime )
        {
            if ( this.mCurTime > 0 )
            {
                this.mCurTime -= 1 ;
                if ( this.mCurTime == 0 )
                {
                    this.CancelInvoke();
                }
            }
        }
        else
        {
            this.mCurTime += 1 ;
        }
    }

}
