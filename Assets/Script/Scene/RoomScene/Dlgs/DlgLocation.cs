using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI ;
public class DlgLocation : DlgBase
{
    public List<PlayerInfoItem> mPlayers = new List<PlayerInfoItem>();
    public List<GameObject> mUnknownGPSIcons = new List<GameObject>();
    public List<Text> mDistanceLabes = new List<Text>();

    public List<PlayerInfoItem> mPlayersSeat3 = new List<PlayerInfoItem>();
    public List<GameObject> mUnknownGPSIconsSeat3 = new List<GameObject>();
    public List<Text> mDistanceLabesSeat3 = new List<Text>();

    public Transform mLocationLayer  ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void showDlg<T>( ResultCallBack pfResult, T jsUserData, CloseCallBack pfOnClose )
    {
        base.showDlg(pfResult,jsUserData,pfOnClose);
        var data = jsUserData as RoomData ;
        this.refresh(data);
    }

    public void refresh( RoomData data )
    {
        var setaCnt = data.mBaseData.seatCnt ;
        var vPlayers = setaCnt == 4 ? this.mPlayers : this.mPlayersSeat3;
        var vDistanceLabes = setaCnt == 4 ? this.mDistanceLabes : this.mDistanceLabesSeat3 ;
        var vUnknownGPSIcon = setaCnt == 4 ? this.mUnknownGPSIcons : this.mUnknownGPSIconsSeat3 ;
        this.mLocationLayer.localPosition = setaCnt == 4 ? Vector3.zero : ( new Vector3(0,-88,0) );

        var selfIdx = data.getSelfIdx() ;
        selfIdx = selfIdx == -1 ? 0 : selfIdx ;
        int distanceIdx = 0 ;
        for ( int idx = 0 ; idx < setaCnt ; ++idx )
        {
            var realIdx = ( selfIdx + idx ) % setaCnt ;
            var pCur = data.mPlayers[realIdx];
            vPlayers[idx].gameObject.SetActive(true);
            if ( pCur == null || pCur.isEmpty() )
            {
                vPlayers[idx].clear();
                vUnknownGPSIcon[idx].SetActive(false);
            }
            else
            {
                vPlayers[idx].playerUID = pCur.nUID ;
                vUnknownGPSIcon[idx].SetActive(false);
                var pd = PlayerInfoDataCacher.getInstance().getPlayerInfoByID(pCur.nUID);
                if ( null != pd )
                {
                    vUnknownGPSIcon[idx].SetActive( pd.GPS_J < 1 || pd.GPS_W < 1 );
                }
            }

            // caculate distance 
            for ( int targetIdx = idx + 1 ; targetIdx < setaCnt ; ++targetIdx )
            {
                var targetSvrIdx = (selfIdx + targetIdx) % setaCnt ;
                var pT = data.mPlayers[targetSvrIdx] ;
                bool isNoNeedCaculateDis = pCur == null || pT == null ;
                PlayerInfoData curData = null ,targetData = null;
                if ( isNoNeedCaculateDis == false )
                {
                    curData = PlayerInfoDataCacher.getInstance().getPlayerInfoByID(pCur.nUID);
                    targetData = PlayerInfoDataCacher.getInstance().getPlayerInfoByID(pT.nUID);
                    isNoNeedCaculateDis = pCur.isEmpty() || pT.isEmpty() || curData == null || targetData == null || ( curData.GPS_J < 1 || curData.GPS_W < 1 ) || ( targetData.GPS_J < 1 || targetData.GPS_W < 1 ) ;
                }

                if ( isNoNeedCaculateDis )
                {
                    vDistanceLabes[distanceIdx].text = "未知距离" ;
                }
                else
                {
                    vDistanceLabes[distanceIdx].text = GPSManager.getInstance().caculateDistance(curData.GPS_J,curData.GPS_W,targetData.GPS_J ,targetData.GPS_W ) + "米" ;
                }
#if UNITY_EDITOR
                if ( null != targetData && null != curData )
                {
                    vDistanceLabes[distanceIdx].text += targetData.name + " AND " + curData.name ;
                }
#endif
                vDistanceLabes[distanceIdx].transform.parent.parent.gameObject.SetActive(true);
                ++distanceIdx ;
            }
        }
    }
}
