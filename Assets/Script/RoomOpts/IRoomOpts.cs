using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public interface IRoomOpts
{
    int round { get ; set ;}
    int seatCnt { get ; set ; }

    ePayRoomCardType payType { get ; set ;}

    int fee { get ; set ; }

    eGameType gameType{ get ; set ;}
    void parseOpts( JSONObject jsOpts );
    JSONObject toJsOpts();
}
