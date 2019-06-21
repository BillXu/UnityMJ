using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement ;

public class LoadScene : NetBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var ip = GameConfig.getInstance().SVR_IP ;
        Debug.Log("load scene ip = " + ip );
        Network.getInstance().setUpAndConnect(ip,GameConfig.getInstance().SVR_BACK_UP_IP);    
    }

    protected override void onConnectResult( bool isScucess )
    {
        if ( isScucess == false )
        {
            Debug.Log("connect svr failed");
            return ;
        }
        // wait player data , oN destroy will auto to remove handle , in base ;
        Debug.Log("change scene = " + GameConfig.getInstance().SCENE_NAME_LOGIN );
        SceneManager.LoadScene(GameConfig.getInstance().SCENE_NAME_LOGIN) ;
    }
}
