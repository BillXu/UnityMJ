using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System ;
public class Utility : MonoBehaviour
{
    // public static eMsgPort getMsgPortByGameType( eGameType game   ) 
    // {
    //     switch( game )
    //     {
    //         case eGameType.eGame_CFMJ:
    //         {
    //             return eMsgPort.ID_MSG_PORT_CFMJ;
    //         }
    //         break;
    //         case eGameType.eGame_NCMJ:
    //         {
    //             return eMsgPort.ID_MSG_PORT_NCMJ;
    //         }
    //         break ;
    //         case eGameType.eGame_AHMJ:
    //         {
    //             return eMsgPort.ID_MSG_PORT_AHMJ;
    //         }
    //         break ;
    //     }
    //     return null ;
    // }

    public static eMsgPort getMsgPortByRoomID( int nRoomID )
    {
        var nComNum = nRoomID % 100;
        var portTypeCrypt = (Math.Floor(nRoomID / 100.0)) % 100;
        if (nComNum >= 50) {
            portTypeCrypt = portTypeCrypt + 100 - nComNum;
        } else {
            portTypeCrypt = portTypeCrypt + 100 + nComNum;
        }
        return (eMsgPort)(portTypeCrypt %= 100);
    }

    public static string getTimeString( int timeStamp )
    {
        DateTime dtStart = new DateTime(1970, 1, 1);
        dtStart = dtStart.ToLocalTime();
        long lTime = ((long)timeStamp * 10000000);
        TimeSpan toNow = new TimeSpan(lTime);
        DateTime targetDt = dtStart.Add(toNow);
        return targetDt.ToString();
    }

    public static eMsgPort getMsgPortByGameType( eGameType game  )
    {
        return eMsgPort.ID_MSG_PORT_DDMJ ;
    }
}
