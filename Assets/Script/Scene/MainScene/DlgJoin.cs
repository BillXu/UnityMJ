using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events ;
using UnityEngine.UI ;
using System.Text ;
public class DlgJoin : DlgBase
{
    // Start is called before the first frame update
    public List<Text> mInputedNumbers ;
    int mCurIdx = 0 ;
    public string resultNumber
    {
        get 
        {
            var str = new StringBuilder() ;
            foreach ( var item in this.mInputedNumbers )
            {
                str.Append(item.text) ;
            }
            return str.ToString() ;
        }
    } 

    public void click_1()
    {
        this.onClickNumber(1);
    }

    public void click_2()
    {
        this.onClickNumber(2);
    }

    public void click_3()
    {
        this.onClickNumber(3);
    }
    public void click_4()
    {
        this.onClickNumber(4);
    }
    public void click_5()
    {
        this.onClickNumber(5);
    }
    public void click_6()
    {
        this.onClickNumber(6);
    }
    public void click_7()
    {
        this.onClickNumber(7);
    }
    public void click_8()
    {
        this.onClickNumber(8);
    }
    public void click_9()
    {
        this.onClickNumber(9);
    }
    public void click_0()
    {
        this.onClickNumber(0);
    }
    public void click_Reinput()
    {
        this.clear();
    }
    public void click_OK()
    {
        if ( this.mCurIdx != this.mInputedNumbers.Count )
        {
            Prompt.promptText("数字不合法，请输6个数字") ;
            return ;
        }

        if ( this.pFuncResult != null )
        {
            this.pFuncResult(this,null) ;
        }
    }

    void onClickNumber( int n )
    {
        if ( this.mCurIdx < this.mInputedNumbers.Count )
        {
            this.mInputedNumbers[this.mCurIdx++].text = n.ToString();
        }
    }

    public void clear()
    {
        this.mCurIdx = 0 ;
        foreach (var item in this.mInputedNumbers )
        {
            item.text = "" ;
        }
    }

    public override void showDlg<T>(ResultCallBack pfResult, T jsUserData, CloseCallBack pfOnClose)
    {
        base.showDlg(pfResult,jsUserData,pfOnClose) ;
        this.clear();
    }
}
