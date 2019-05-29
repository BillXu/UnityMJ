using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;
using System ;
using UnityEngine.SceneManagement ;
public class DlgResultTotal : DlgBase
{
    public List<TotalResultItem> mItems ;
    public Text mRulesDesc ;
    public Text mRoomID ;
    public Text mTime ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void showDlg<T>(ResultCallBack pfResult, T jsUserData, CloseCallBack pfOnClose)
    {
        RoomData data = jsUserData as RoomData ;
        this.mRoomID.text = "房间号: " + data.mBaseData.roomID ;
        this.mTime.text = DateTime.Now.ToString(); 
        this.mRulesDesc.text = data.mBaseData.rule + "  局数:" + data.mBaseData.totalRoundCnt + "  底分:" + data.mBaseData.baseScore;
        foreach (var item in this.mItems )
        {
            item.gameObject.SetActive(false);
        }

        var results = data.mTotalResultData.mResults ;
        for ( int i = 0; i < results.Count; i++ )
        {
            this.mItems[i].gameObject.SetActive(true);
            this.mItems[i].playerUID = results[i].uid ;
            this.mItems[i].huCnt = results[i].huCnt ;
            this.mItems[i].gangCnt = results[i].gangCnt;
            this.mItems[i].dianPaoCnt = results[i].dianPaoCnt ;
            this.mItems[i].bestSingleWin = results[i].SingleBestWin ;
            this.mItems[i].final = results[i].final; 
            this.mItems[i].waitTime = results[i].waitTime ;
            this.mItems[i].isBigWin = data.mTotalResultData.mBigWinerUID.Contains( results[i].uid );
            this.mItems[i].isDismiss = results[i].uid == data.mTotalResultData.mApplyDismissID;
            this.mItems[i].isTuHao = data.mTotalResultData.mTuHaoID.Contains( results[i].uid );
        }
    }
    public void onClickShare()
    {
        Prompt.promptText( "即将分享成功" );
    }

    public void onClickToMainScene()
    {
        SceneManager.LoadScene(GameConfig.getInstance().SCENE_NAME_MAIN) ;
    }
}
