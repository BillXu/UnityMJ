using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonBones;
using UnityEngine.Events ;
public class DlgVoiceRecording : MonoBehaviour
{
    public Image mImageFrame ;
    public UnityArmatureComponent mRecordingAni ;
    float MAX_RECORDING_TIME = 15;
    float mLeftTime = 0 ;
    public UnityEvent onTimeOut = null ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.mLeftTime -= Time.deltaTime;
        if ( this.mLeftTime < 0 )
        {
            this.mLeftTime = 0 ;
            if ( this.onTimeOut != null )
            {
                this.onTimeOut.Invoke();
            }
            this.closeDlg();
        }
        this.mImageFrame.fillAmount = this.mLeftTime / MAX_RECORDING_TIME ;
    }

    public void showDlg()
    {
        this.gameObject.SetActive(true);
        this.mLeftTime = MAX_RECORDING_TIME ;
        this.mImageFrame.fillAmount = 1 ;
        this.mRecordingAni.animation.Play();
    }

    public void closeDlg()
    {
        this.gameObject.SetActive(false);
        this.mRecordingAni.animation.Stop();
    }

    public bool isShow()
    {
        return this.gameObject.activeSelf ;
    }
}
