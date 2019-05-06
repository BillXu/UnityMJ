using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public interface IRoomOpts
{
    int round { get ; set ;}
    int playerCnt { get ; set ; }

    int payType { get ; set ;}

    int fee { get ; set ; }

    void parseOpts( JSONObject jsOpts );
    JSONObject toJsOpts();
}
