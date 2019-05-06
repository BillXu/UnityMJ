using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    // Start is called before the first frame update
    public DlgJoin mDlgJoin = null ;
    public DlgCreateRoom mDlgCreate = null ;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onJoin()
    {
        this.mDlgJoin.showDlg<string>(null,null,null) ;
    }

    public void onCreate()
    {
        this.mDlgCreate.showDlg<string>(null,null,null) ;
    }
}
