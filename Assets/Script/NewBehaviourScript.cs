using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using System ;
using UnityEngine.UI;
using UnityEngine.Events;
internal class Test
{
    static public string Event = "adc" ; 
    public bool a( EventArg arg )
    {
        Debug.Log( "this is a function" );
        return false ;
    }

    public bool b( EventArg arg )
    {
        var obj = arg as EventWithObject<JSONObject> ;
        var s = obj.argObject.ToString();
        var obj2 = JSONObject.Parse(s) ;
        double a = obj2["a"].Number ;
        string b = obj2["b"].Str; ;
        Type c = typeof(EventArg);
        Debug.Log( "this is b funciton js = " + s + "a = " + a + " b = " + b + "type info = " + c.Name );
        return false ;
    }

    public void testEvent()
    {
        EventDispatcher.getInstance().registerEventHandle(Test.Event,this.a) ;
        EventDispatcher.getInstance().registerEventHandle(Test.Event,this.b) ;
        var js = new JSONObject();
        js["a"] = 3 ;
        js["b"] = "acc" ;
        EventDispatcher.getInstance().dispatch(Test.Event,js);
        EventDispatcher.getInstance().removeEventHandleByTarget(this) ;
        EventDispatcher.getInstance().dispatch("a");
    }
}

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    public RawImage img ;
    public Button img2;
    public DlgBase dlg ;
    void Start()
    {
        //var t = new Test();
        //t.testEvent();
        JSONArray js = new JSONArray();
        js.Add(1);
        js.Add(2);
        string str = js.ToString();
        str = "{ \"a\" : " + str + "}" ;
        Debug.Log(str);
        var v = JSONObject.Parse(str);
        if ( v == null )
        {
            Debug.LogError("parse error ");
        }
        else
        {
            var ss2 = v.ToString();
            Debug.LogWarning("---ss2 = " + ss2);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void doClick()
    {
        dlg.showDlg(null,null,null);
    }
}


