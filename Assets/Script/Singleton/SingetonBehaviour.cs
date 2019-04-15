using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingetongBehaviour<T> : MonoBehaviour where T : Object
{
    // Start is called before the first frame update
    static private T s_instance = null ;
    protected void Awake() {
        if ( s_instance == null )
        {
            s_instance = this as T;
        }
    }

    static public T getInstance()
    {
        return s_instance;
    }
}
