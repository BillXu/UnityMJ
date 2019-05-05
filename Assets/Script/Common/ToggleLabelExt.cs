using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;
public class ToggleLabelExt : MonoBehaviour
{
    // Start is called before the first frame update
    public Toggle mToggle = null ;
    public Color mChecked ;
    public Color mDefault;    
    void Start()
    {
        this.refresh();
    }
    public void onClick()
    { 
        if ( this.mToggle.group != null && this.mToggle.isOn && this.mToggle.group.allowSwitchOff == false )
        {
            return ;
        }
        this.mToggle.isOn = !this.mToggle.isOn ;
        this.refresh();
    }

    public void onToggle( bool isCheck )
    {
        this.refresh();
    }

    void refresh()
    {
        GetComponent<Text>().color = this.mToggle.isOn ? this.mChecked : this.mDefault ;
    }
}
