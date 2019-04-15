 using System ;
 using System.Collections.Generic ;
 using UnityEngine ;
 namespace Client
 {
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

        public bool registerEventHandle( string str , EventHanlde handle )
        {
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

            var vHandleList = m_vHandles[arg.type];
            foreach ( var handle in vHandleList )
            {
                if ( handle(arg) )
                {
                    Debug.Log( "interup event hadle for retrun true " );
                    return ;
                }
            }
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

 }
