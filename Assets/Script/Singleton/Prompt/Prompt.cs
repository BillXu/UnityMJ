using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prompt : SingletonBehaviour<Prompt>
{
    public PromptDlg pPromptDlg = null;

    public GameObject pPromptTextPrefab = null;

    public List<GameObject> vDisplayingPromptText = new List<GameObject>();
    public List<GameObject> vReserve = new List<GameObject>();
    // LIFE-CYCLE CALLBACKS:
    public void showPromptText( string text, float nDisplayTime = 3 )
    {
        GameObject p = null;
        if ( this.vReserve.Count > 0 )
        {
            p = this.vReserve[0];
            this.vReserve.RemoveAt(0);
        }
        else
        {
            p = Instantiate(this.pPromptTextPrefab);
            p.transform.SetParent(this.transform);
        }
        var pp = p.GetComponent<PromptText>();
        pp.nDisplayTime = nDisplayTime ;
        //Debug.Log("do show prompt text invoked");
        pp.setText(text,( PromptText t )=>{
            var idxt = this.vDisplayingPromptText.FindIndex(( GameObject s )=>{ if (s == null ) return false ; return s.Equals(p);} );
            if ( idxt < 0 )
            {
                Debug.LogError( "why dont store it ?" );
                return ;
            }
            
            if ( this.vReserve.Count < 6 ) // we do not cache too many 
            {
                this.vReserve.Add(p);
            }
            else
            {
                Debug.LogWarning("why have so many propmt text at the same time");
                GameObject.Destroy(p);
            }

            this.vDisplayingPromptText[idxt] = null ;
            
            //Debug.Log("cal back invoked");
            var notEmptyIdx = this.vDisplayingPromptText.FindIndex(( GameObject s )=>{ return s != null;} );
            if ( notEmptyIdx < 0 )
            {
                this.vDisplayingPromptText.Clear() ;
            }
        }) ;

        var emptyIdx = this.vDisplayingPromptText.FindIndex(( GameObject s )=>{ return s == null;} );
        if ( emptyIdx != -1 )
        {
            this.vDisplayingPromptText[emptyIdx] = p ;
        }
        else
        {
            this.vDisplayingPromptText.Add(p);
        }

        int idx = 0 ;
        foreach (var pNode in this.vDisplayingPromptText )
        {
            if ( null == pNode )
            {
                continue ;
            }
            var trans = pNode.GetComponent<RectTransform>();
            var y = -1 * trans.rect.height * trans.localScale.y * idx + 10;
            trans.localPosition = new Vector3(0,y );
            ++idx;
            //Debug.Log("new pos y = " + y);
        }
    }

    public void showDlg( string dlgText, bool isOneBtn = true, DlgBase.ResultCallBack pfResult = null , DlgBase.CloseCallBack pfOnClose = null)
    {
        this.pPromptDlg.isOneBtn = isOneBtn ;
        this.pPromptDlg.showDlg(pfResult,dlgText,pfOnClose ) ;
    }

    public static void promptText( string text, float nDisplayTime = 2.0f )
    {
        Prompt.getInstance().showPromptText(text,nDisplayTime);
    }

    public static void promptDlg( string dlgText, bool isOneBtn = true, DlgBase.ResultCallBack pfResult = null , DlgBase.CloseCallBack pfOnClose = null )
    {
        Prompt.getInstance().showDlg(dlgText,isOneBtn,pfResult,pfOnClose);
    }
}
