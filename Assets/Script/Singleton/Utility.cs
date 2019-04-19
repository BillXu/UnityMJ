using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void showTip( string tip )
    {

    }

    public static eMsgPort getMsgPortByRoomID( int roomID )
    {
        return eMsgPort.ID_MSG_PORT_MJ;
    }

    public static string getTimeString( int utcSeconds )
    {
        return "time" ;
    }
}
