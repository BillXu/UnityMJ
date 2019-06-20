using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events ;
using UnityEngine.EventSystems ;
using System ;
public class ButtonVoice : MonoBehaviour
{
    float mDwonY = 0 ;
    float mCancelEaps = 30 ;
    [Serializable]
    public class ButtonVoiceResult : UnityEvent<bool>{}
    public ButtonVoiceResult onButtonVoiceResult = null ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(BaseEventData eventData)
    {
        PointerEventData t = (PointerEventData)eventData;
        Debug.Log("OnPointerDown pos = " + t.position );
        this.mDwonY = t.position.y ;
    }

    public void OnPointerUp(BaseEventData eventData)
    {
        PointerEventData t = (PointerEventData)eventData;
        Debug.Log("OnPointerUp pos = " + t.position );
        if ( this.onButtonVoiceResult != null )
        {
            this.onButtonVoiceResult.Invoke(t.position.y - this.mDwonY > this.mCancelEaps );
        }
    }
}
