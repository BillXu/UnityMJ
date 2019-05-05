 using System ;
 using System.Collections.Generic ;
 using UnityEngine ;
public class EventArg
{
    public string type ;
}

public class EventWithObject<T> : EventArg
{
    public T argObject  ;
} 

public class EventDispatcher : Singleton<EventDispatcher>
{
    public delegate bool EventHanlde( EventArg hevent );

    private Dictionary<string,List<EventHanlde>> m_vHandles = null;
    string mLockEventTypeName = null ; // 锁住事件类型，主要用于防止在事件函数内存在增减同类事件从而导致遍历的迭代器失效，出现bug。
    List<EventHanlde> mLockAddCacher = new List<EventHanlde>();
    List<EventHanlde> mLockRemoveCacher = new List<EventHanlde>();


    public bool registerEventHandle( string str , EventHanlde handle )
    {
        if ( str == this.mLockEventTypeName )
        {
            this.mLockAddCacher.Add(handle);
            return true;
        }

        if ( null == m_vHandles )
        {
            m_vHandles = new Dictionary<string,List<EventHanlde>>();
        }

        if ( !m_vHandles.ContainsKey(str) )
        {
            m_vHandles[str] = new List<EventHanlde>() ;
        }
        m_vHandles[str].Add(handle) ;
        Debug.Log("add one handle");
        return false ;
    }

    public bool removeEventHandle( string eventName , EventHanlde handle )
    {
        if ( eventName == this.mLockEventTypeName )
        {
            this.mLockRemoveCacher.Add(handle);
            return true;
        }

        if ( null == m_vHandles || false == m_vHandles.ContainsKey(eventName) )
        {
            return false ;
        }

        var vHandleList  = m_vHandles[eventName];
        if ( null != vHandleList && vHandleList.Contains(handle) )
        {
            Debug.Log("do removed handle");
            vHandleList.Remove(handle) ;
        }
        return true ;
    }

    public bool removeEventHandleByTarget( object target )
    {
        if ( null == m_vHandles )
        {
            return false ;
        }

        var vWillDelete = new Dictionary<string,List<EventHanlde>>();
        foreach ( var pair in m_vHandles )
        {
            var vHandlelist = pair.Value;
            var eventNmae = pair.Key ;
            List<EventHanlde> vDelList = null ;
            foreach ( var handle in vHandlelist )
            {
                if ( handle.Target.Equals(target) )
                {
                    if ( vDelList == null )
                    {
                        vDelList = new List<EventHanlde>();
                        vWillDelete[eventNmae] = vDelList ;
                    }
                    vDelList.Add(handle);
                    Debug.Log( "sign delete event handle in list " );
                }
            }
        }

        // do remove ;
        foreach ( var del in vWillDelete )
        {
            var vList = del.Value ;
            foreach ( var handle in vList )
            {
                removeEventHandle(del.Key,handle) ;
            }
        }
        vWillDelete = null ;
        return true ;
    }

    public void dispatch( string eventNme )
    {
        dispatch(eventNme,0);
        return ;
    }

    public void dispatch( EventArg arg )
    {
        if ( null == m_vHandles || false == m_vHandles.ContainsKey(arg.type) )
        {
            return ;
        }

        this.mLockEventTypeName = arg.type ;
        var vHandleList = m_vHandles[arg.type];
        foreach ( var handle in vHandleList )
        {
            if ( handle(arg) )
            {
                Debug.Log( "interup event hadle for retrun true " );
                return ;
            }
        }

        foreach ( var itemA in this.mLockAddCacher )
        {
            this.registerEventHandle(arg.type,itemA) ;
        }
        this.mLockAddCacher.Clear();

        foreach (var itemR in this.mLockRemoveCacher )
        {
            this.removeEventHandle(arg.type,itemR) ;
        }
        this.mLockRemoveCacher.Clear();
        this.mLockEventTypeName = null;
    }
    public void dispatch<T>(string eventName , T eventArgObj )
    {
        if ( null == m_vHandles || false == m_vHandles.ContainsKey(eventName) )
        {
            return ;
        }

        var eve = new EventWithObject<T>();
        eve.argObject = eventArgObj ;
        eve.type = eventName;
        dispatch(eve);
        return ;
    }
}
