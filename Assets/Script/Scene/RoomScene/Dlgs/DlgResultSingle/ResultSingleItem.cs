using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultSingleItem : MonoBehaviour
{
    public Text mHuScore ;
    public Text mGangScore ;
    public Text mHuType ;
    public GameObject mHuTypeComponent ;
    public int huScore 
    {
        set 
        {
            this.mHuScore.text = "胡分：" + (value > 0 ? "+" : "" ) + value ;
            this.mHuTypeComponent.SetActive( value > 0 );
            Color clr ;
            ColorUtility.TryParseHtmlString( value >= 0 ? "#FFCD1A":"#43CEFF",out clr );
            this.mHuScore.color = clr ;
        }
    }

    public int gangScore
    {
        set 
        {
            this.mGangScore.text = "杠分：" + (value > 0 ? "+" : "" ) + value ;
            Color clr ;
            ColorUtility.TryParseHtmlString( value >= 0 ? "#FFCD1A":"#43CEFF",out clr );
            this.mGangScore.color = clr ;
        }
    }
    public string huType
    {
        set 
        {
            this.mHuType.text = value ;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
