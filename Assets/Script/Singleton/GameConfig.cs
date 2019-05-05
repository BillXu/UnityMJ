using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : SingletonBehaviour<GameConfig>
{
    public string DWONLOAD_URL { get ; set; } = "http://www.baidu.com" ;
    public int MAX_RECORDER_CACHER = 20;

    public string SVR_IP = "127.0.0.1:40008";
    public string SVR_BACK_UP_IP = null ;

    public string SCENE_NAME_LOGIN = "loginScene";
    public string SCENE_NAME_MAIN = "mainScene" ;
    public string SCENE_NAME_ROOM = "roomScene";
}
