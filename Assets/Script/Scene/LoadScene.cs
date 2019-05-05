using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement ;
public class LoadScene : NetBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Network.getInstance().setUpAndConnect(GameConfig.getInstance().SVR_IP,GameConfig.getInstance().SVR_BACK_UP_IP);    
    }

    protected override void onConnectResult( bool isScucess )
    {
        if ( isScucess == false )
        {
            Debug.Log("connect svr failed");
            return ;
        }
        // wait player data , oN destroy will auto to remove handle , in base ;
        SceneManager.LoadScene(GameConfig.getInstance().SCENE_NAME_LOGIN) ;
    }
}
