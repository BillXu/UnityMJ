using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Toggle))]
public class ToggleToSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    Image mBackGroundImg = null ;
    void Start()
    {
        var tog = this.transform.GetComponent<Toggle>();
        if ( tog == null )
        {
            Debug.LogError("must use with toggle control");
        }

        var bgNode = this.transform.GetChild( 0 );
        if ( bgNode == null || bgNode.gameObject.name != "Background" )
        {
            Debug.LogError("invalid toggle");
            return ;
        }

        this.mBackGroundImg = bgNode.GetComponent<Image>();
        if ( null == this.mBackGroundImg )
        {
            Debug.LogError("back ground do not have image component");
            return ;
        }

        if ( tog.onValueChanged == null )
        {
            tog.onValueChanged = new Toggle.ToggleEvent();
        }
        tog.onValueChanged.AddListener(this.onClickSwitch) ;
        this.onClickSwitch(tog.isOn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickSwitch( bool isCheck )
    {
        this.mBackGroundImg.enabled = !isCheck ;
    }
}
