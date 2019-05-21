using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;

public class RoomPlayerItem : MonoBehaviour
{
    public Text mChip ;
    public PlayerInfoItem mItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void refresh( int nUID , int nChips )
    {
        this.mChip.text = "" + nChips ;
        this.mItem.playerUID = nUID ;
    }
}
