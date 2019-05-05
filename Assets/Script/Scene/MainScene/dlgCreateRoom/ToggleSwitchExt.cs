using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSwitchExt : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject mChecked;
    public GameObject mDefault ;
    
    public void onToggle( bool isSel )
    {
        this.mChecked.SetActive(isSel);
        this.mDefault.SetActive(!isSel );
    }
}
