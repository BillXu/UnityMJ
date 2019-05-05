using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginScene : MonoBehaviour
{
    // Start is called before the first frame update
    public LoginSceneData mData = null ;
    void Start()
    {
    }
    // Update is called once per frame
    public void regWechatLogin()
    {
        if ( this.mData.isWaitRespone )
        {
            Prompt.promptText("正在登录中...");
            return ;
        }
    }

    public void clickVistor_0()
    {
        this.mData.login("v0","0") ;
        this.mData.setPlayerDetailInfo("v0","http://thirdwx.qlogo.cn/mmopen/vi_32/MBVyjTLDPY5ALD6QaicTpx3lgVAApjY7FAH1CCe7kjtQibxWRmgsibAznQwXklA8nMMOpq5g4Y0wSGJ9KJyzDC9IA/132",eSex.eSex_Male) ;
    }
    public void clickVistor_1()
    {
        this.mData.login("v1","0") ;
        this.mData.setPlayerDetailInfo("v1","http://thirdwx.qlogo.cn/mmopen/vi_32/MBVyjTLDPY5ALD6QaicTpx3lgVAApjY7FAH1CCe7kjtQibxWRmgsibAznQwXklA8nMMOpq5g4Y0wSGJ9KJyzDC9IA/132",eSex.eSex_Male) ;
    }
    public void clickVistor_2()
    {
        this.mData.login("v2","0") ;
        this.mData.setPlayerDetailInfo("v2","http://thirdwx.qlogo.cn/mmopen/vi_32/MBVyjTLDPY5ALD6QaicTpx3lgVAApjY7FAH1CCe7kjtQibxWRmgsibAznQwXklA8nMMOpq5g4Y0wSGJ9KJyzDC9IA/132",eSex.eSex_Male) ;
    }
    public void clickVistor_3()
    {
        this.mData.login("v3","0") ;
        this.mData.setPlayerDetailInfo("v3","http://thirdwx.qlogo.cn/mmopen/vi_32/MBVyjTLDPY5ALD6QaicTpx3lgVAApjY7FAH1CCe7kjtQibxWRmgsibAznQwXklA8nMMOpq5g4Y0wSGJ9KJyzDC9IA/132",eSex.eSex_Male) ;
    }
}
