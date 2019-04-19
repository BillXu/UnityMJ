using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : SingletonBehaviour<GameConfig>
{
    public string DWONLOAD_URL { get ; set; } = "http://www.baidu.com" ;
    public int MAX_RECORDER_CACHER = 20;
}
