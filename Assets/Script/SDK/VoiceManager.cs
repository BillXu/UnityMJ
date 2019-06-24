using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gcloud_voice;

public class VoiceManager : SingletonBehaviour<VoiceManager>
{
    public enum eVoiceMgrState
    {
        Idle = 1,
        Downloading = 1 << 1,
        Uploading = 1 << 2,
        Playing = 1 << 3, 
        Recording = 1 << 4 ,       
    }
    
    class VoiceFile
    {
        public string fileID ;
        public string fileFullPath ;
        public int userTag ;
    }

    private string APP_ID = "1123375188" ;
    private string APP_KEY = "fecbbfdd3174e7d3493583a413b4da1c" ;
    private bool isInit = false ;
    private string TEMP_PATH = "" ;

    public static string EVENT_PLAY_BEGIN = "EVENT_PLAY_BEGIN"; // int userTag = 0 ;
    public static string EVENT_PLAY_FINISH = "EVENT_PLAY_FINISH"; // int userTag = 0 ;
    public static string EVENT_UPLOAD_FINISH = "EVENT_UPLOAD_FINISH"; // string fileID ;

    IGCloudVoice mVoiceEngine = null;
    private bool _mIsObtainedKey;
    private string _mRecordingFilePath;
    int mStateFlag = (int)eVoiceMgrState.Idle ;

    List<VoiceFile> mWaitDownloadFiles = new List<VoiceFile>();
    List<VoiceFile> mWaitPlayFiles = new List<VoiceFile>();
    List<VoiceFile> mFilePool = new List<VoiceFile>();
    Dictionary<int,VoiceFile> mCacherLastVoice = new Dictionary<int, VoiceFile>();
    public bool init( string playerTag )
    {
        if ( this.isInit )
        {
            Debug.Log( "already init voice mgr do not init again" );
            return false;
        }

        this.mVoiceEngine = GCloudVoice.GetEngine();
        if ( null != this.mVoiceEngine )
        {
            int result = this.mVoiceEngine.SetAppInfo(this.APP_ID,this.APP_KEY,playerTag ) ;
            if (GCloudVoiceErr.GCLOUD_VOICE_SUCC == (GCloudVoiceErr) result)
            {
                Debug.Log("GVoice SetAppInfo success");
            }
            else
            {
                Prompt.promptText("GVoice SetAppInfo fail");
                return false ;
            }


            Debug.Log("Start initializing the gvoice engine!");
            int iret = this.mVoiceEngine.Init();
            if (GCloudVoiceErr.GCLOUD_VOICE_SUCC == (GCloudVoiceErr) iret)
            {
                Debug.Log("GVoice Init success");
            }
            else
            {

                Prompt.promptText("GVoice Init fail");
                return false ;
                // Do error handling work according to the error code and the document
                // from the official website.
            }
        }
        else
        {
            Debug.LogError("get voice engine failed");
            return false ;
        }
        
        int ret = this.mVoiceEngine.SetMode(GCloudVoiceMode.Messages);
        if (GCloudVoiceErr.GCLOUD_VOICE_SUCC == (GCloudVoiceErr) ret)
        {
            Debug.Log("GVoice SetMode success");
        }
        else
        {
            Debug.Log("GVoice SetMode fail with code " + ret);
            return false;
            // Do error handling work according to the error code and the document
            // from the official website.
        }
        this.isInit = true ;
        this.setUpCallBack();
        return this.isInit ;
    }
    void setUpCallBack()
    {
        this.TEMP_PATH = Application.persistentDataPath ;
        this._mRecordingFilePath = Application.persistentDataPath + "/RecordFile.data";

        // The engine has been initialized in the MainScene.
        this.mVoiceEngine.OnApplyMessageKeyComplete += code =>
        {
            if (IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_MESSAGE_KEY_APPLIED_SUCC == code)
            {
                Debug.Log("GVoice apply message key success");
                //_mMsgLogTxt.text = "GVoice apply message key success";
                _mIsObtainedKey = true;
            }
            else
            {
                Debug.Log("GVoice apply message key fail with code " + code);
                //_mMsgLogTxt.text = "GVoice apply message key fail with code " + code;
                _mIsObtainedKey = false;
                Prompt.promptText( "申请语音服务失败" );
            }
        };

        this.mVoiceEngine.OnUploadReccordFileComplete += (code, filepath, fileid) =>
        {
            this.clearStateFlag(eVoiceMgrState.Uploading);
            if (IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_UPLOAD_RECORD_DONE == code)
            {
                string logStr = "Upload record file success.\t" +
                                "The filepath is: " + filepath + "\t." +
                                "The fileid is: " + fileid + ".";
                Debug.Log(logStr);
                EventDispatcher.getInstance().dispatch(VoiceManager.EVENT_UPLOAD_FINISH,fileid);
            }
            else
            {
                string logStr = "Upload record file meets some error.\t" +
                                "The complete code is: " + code + "\t." +
                                "The filepath is: " + filepath + "\t." +
                                "The fileid is: " + fileid + ".";
                Debug.Log(logStr);
                Prompt.promptText( "处理音频文件失败2" );
            }
        };

        this.mVoiceEngine.OnDownloadRecordFileComplete += (code, filepath, fileid) =>
        {
            clearStateFlag(eVoiceMgrState.Downloading);
            if (IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_DOWNLOAD_RECORD_DONE == code)
            {
                string logStr = "Download record file success.\t" +
                                "The filepath is: " + filepath + "\t." +
                                "The fileid is: " + fileid + ".";
                Debug.Log(logStr);
                if ( this.mWaitDownloadFiles.Count > 0 )
                {
                    var p = this.mWaitDownloadFiles[0] ;
                    this.playerFile(p);
                    this.mWaitDownloadFiles.RemoveAt(0);
                }
                else
                {
                    Prompt.promptText( "不可以思议的错误dc" );
                }
            }
            else
            {
                string logStr = "Download record file meets some error.\t" +
                                "The complete code is: " + code + "\t." +
                                "The filepath is: " + filepath + "\t." +
                                "The fileid is: " + fileid + ".";
                Debug.Log(logStr);
                if ( this.mWaitDownloadFiles.Count > 0 )
                {
                    var p = this.mWaitDownloadFiles[0] ;
                    this.mFilePool.Add(p);
                    this.mWaitDownloadFiles.RemoveAt(0);
                }
                else
                {
                    Prompt.promptText( "不可以思议的错误dc" );
                }               
            }

            while ( this.mWaitDownloadFiles.Count > 0 )
            {
                var p = this.mWaitDownloadFiles[0] ;
                if ( false == this.downloadFile(p.fileID,p.fileFullPath ) )
                {
                    this.mWaitDownloadFiles.Remove(p);
                    this.mFilePool.Add(p);
                    continue ;
                }
                break ;
            }
        };

        this.mVoiceEngine.OnPlayRecordFilComplete += (code, filepath) =>
        {
            if (IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_PLAYFILE_DONE == code)
            {
                string logStr = "Finish playing the record file.\t" +
                                "The filepath is: " + filepath + ".";
                Debug.Log(logStr);
            }
            else
            {
                string logStr = "Play the record file meets some error.\t" +
                                "The complete code is: " + code + "\t." +
                                "The filepath is: " + filepath + ".";
                Debug.Log(logStr);
            }

            if ( this.mWaitPlayFiles.Count > 0 )
            {
                var first = this.mWaitPlayFiles[0];
                EventDispatcher.getInstance().dispatch(VoiceManager.EVENT_PLAY_FINISH,first.userTag);
                this.mWaitPlayFiles.RemoveAt(0);
                if ( this.mCacherLastVoice.ContainsKey(first.userTag) )
                {
                    this.mFilePool.Add(this.mCacherLastVoice[first.userTag]);
                    this.mCacherLastVoice.Remove(first.userTag);
                }
                this.mCacherLastVoice.Add(first.userTag,first);
            }
            clearStateFlag(eVoiceMgrState.Playing);

            while ( this.mWaitPlayFiles.Count > 0 )
            {
                Debug.Log("go on play voice file " + this.mWaitPlayFiles[0].userTag );
                if ( !this.doPlayFile(this.mWaitPlayFiles[0]) )
                {
                    this.mFilePool.Add(this.mWaitPlayFiles[0]);
                    this.mWaitPlayFiles.RemoveAt(0);
                    continue ;
                }
                break ;
            }
        };

        this.mVoiceEngine.ApplyMessageKey(10000);
    }
    public void reset() {
        this.mStateFlag = (int)eVoiceMgrState.Idle ;
    }
    private void Update() {
        if ( this.mVoiceEngine != null && this.isInit )
        {
            this.mVoiceEngine.Poll();
        }
    }
    bool isHaveStateFlag( eVoiceMgrState state )
    {
        return (this.mStateFlag & (int)state) == (int)state ;
    }

    void clearStateFlag( eVoiceMgrState state )
    {
        this.mStateFlag &= ~(int)state;
    }
    void addStateFlag( eVoiceMgrState state )
    {
        this.mStateFlag |= (int)state;
    }

    public bool canStartRecorder()
    {
        return (isHaveStateFlag(eVoiceMgrState.Uploading) == false) && (isHaveStateFlag(eVoiceMgrState.Recording) == false) && (isHaveStateFlag( eVoiceMgrState.Playing ) == false );
    }
    public bool startRecord() 
    {
        if ( this.isInit == false )
        {
            Prompt.promptText( "语音模块没有初始化，暂无法使用语音消息" );
            return false;
        }

        if ( this._mIsObtainedKey == false )
        {
            Prompt.promptText( "服务不可用" );
            return false;
        }

        GCloudVoiceErr nRet =  (GCloudVoiceErr)this.mVoiceEngine.StartRecording(_mRecordingFilePath);
        if ( nRet == GCloudVoiceErr.GCLOUD_VOICE_NEED_AUTHKEY )
        {
            Prompt.promptText( "需要语音授权" );
            return false;
        }

        if ( nRet == GCloudVoiceErr.GCLOUD_VOICE_AUTHING )
        {
            Prompt.promptText( "正在等待语音授权,请稍候" );
            return false;
        }

        if ( nRet != GCloudVoiceErr.GCLOUD_VOICE_SUCC )
        {
            Prompt.promptText( "录音失败code="+nRet );
            return false;
        }
        this.addStateFlag(eVoiceMgrState.Recording);
        return true ;
    }

    public bool stopRecord( bool isUpLoad )
    {
        if ( this.isInit == false )
        {
            Prompt.promptText( "语音模块没有初始化，暂无法使用语音消息" );
            return false;
        }

        if ( this._mIsObtainedKey == false )
        {
            Prompt.promptText( "服务不可用" );
            return false;
        }

        if ( this.isHaveStateFlag( eVoiceMgrState.Recording ) == false )
        {
            Prompt.promptText( "没有开始录音，怎么结束录音呢？" );
            return false;
        }

        this.clearStateFlag( eVoiceMgrState.Recording );
        int ret = this.mVoiceEngine.StopRecording();
        if (GCloudVoiceErr.GCLOUD_VOICE_SUCC == (GCloudVoiceErr) ret)
        {
            Debug.Log("GVoice StopRecording success");
        }
        else
        {
            Debug.Log("GVoice StopRecording fail with error code " + ret);
            return false ;
        }

        if ( isUpLoad )
        {
           var uret = (GCloudVoiceErr)this.mVoiceEngine.UploadRecordedFile(_mRecordingFilePath, 10000);
           if ( uret != GCloudVoiceErr.GCLOUD_VOICE_SUCC )
           {
               Prompt.promptText( "处理音频文件失败" );
               return false ;
           }
           addStateFlag( eVoiceMgrState.Uploading );
        }
        return true ;
    }
    
    public bool playVoice( string fileID, int userTag )
    {
        if ( this.isInit == false )
        {
            Prompt.promptText( "语音模块没有初始化，暂无法使用语音消息" );
            return false;
        }

        if ( this._mIsObtainedKey == false )
        {
            Prompt.promptText( "服务器不可用" );
            return false ;
        }

        VoiceFile p = null ;
        if ( this.mFilePool.Count > 0 )
        {
            p = this.mFilePool[0];
            this.mFilePool.RemoveAt(0);
        }
        else
        {
            p = new VoiceFile();
        }
        p.userTag = userTag ;
        p.fileFullPath = this.TEMP_PATH + userTag + "_"+ Time.fixedTime ;
        p.fileID = fileID ;
        this.mWaitDownloadFiles.Add(p);
        if ( isHaveStateFlag( eVoiceMgrState.Downloading ) == false )
        {
            if ( false == this.downloadFile(this.mWaitDownloadFiles[0].fileID,this.mWaitDownloadFiles[0].fileFullPath ) )
            {
                this.mFilePool.Add(this.mWaitDownloadFiles[0]);
                this.mWaitDownloadFiles.Remove(this.mWaitDownloadFiles[0]);
                return false ;
            }
        }
        return true ;
    }



    bool doPlayFile( VoiceFile sfile )
    {
        if ( this.isInit == false )
        {
            Prompt.promptText( "语音模块没有初始化，暂无法使用语音消息" );
            return false;
        }

        if ( this._mIsObtainedKey == false )
        {
            Prompt.promptText( "服务器不可用" );
            return false ;
        }

        
       var ret = (GCloudVoiceErr)this.mVoiceEngine.PlayRecordedFile(sfile.fileFullPath);
       if ( ret != GCloudVoiceErr.GCLOUD_VOICE_SUCC )
       {
            Prompt.promptText("播放声音文件失败" + ret );
            return false ;
       }
       this.addStateFlag(eVoiceMgrState.Playing);
       EventDispatcher.getInstance().dispatch(VoiceManager.EVENT_PLAY_BEGIN,sfile.userTag);
       return true ;
    }
    
    void playerFile( VoiceFile file )
    {
        this.mWaitPlayFiles.Add(file);
        if ( this.isHaveStateFlag( eVoiceMgrState.Playing ) == false )
        {
            while ( this.mWaitPlayFiles.Count > 0 )
            {
                Debug.Log("go on play voice file " + this.mWaitPlayFiles[0].userTag );
                if ( !this.doPlayFile(this.mWaitPlayFiles[0]) )
                {
                    this.mFilePool.Add(this.mWaitPlayFiles[0]);
                    this.mWaitPlayFiles.RemoveAt(0);
                    continue ;
                }
                break ;
            }
        } 
    }

    public void replayCacheVoice( int nUserTag )
    {
        if ( this.mCacherLastVoice.ContainsKey(nUserTag) == false )
        {
            Prompt.promptText( "无数据播放或正在等待播放" );
            return ;
        }

        var file = this.mCacherLastVoice[nUserTag] ;
        this.playerFile(file);
        this.mCacherLastVoice.Remove(nUserTag);
    }
    private bool downloadFile( string fileID, string fileFullPath )
    {
        int timeOut = 10000;
        var ret = (GCloudVoiceErr)this.mVoiceEngine.DownloadRecordedFile(fileID, fileFullPath,timeOut);
        if ( ret != GCloudVoiceErr.GCLOUD_VOICE_SUCC )
        {
            Prompt.promptText("下载声音文件失败" + ret );
            return false ;
        }
        this.addStateFlag( eVoiceMgrState.Downloading );
        return true ;
    }
}
