using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events ;
using UnityEngine.EventSystems ;
public interface SelfMJCardDelegate 
{
    void onOneClick( SelfMJCard v );
    void onDoubleClick( SelfMJCard v );
    bool onDragOut( SelfMJCard v );  // return value indicate , weather go back pos , true do not go back , false go back ;
}
public class SelfMJCard : MonoBehaviour,IPointerUpHandler,IDragHandler,IBeginDragHandler,IEndDragHandler,IPointerDownHandler
{
    // Start is called before the first frame update
    Camera mDraginAssociateCamera = null ;
    Vector3 mWorldPosBeforeDraging ;
    public SelfMJCardDelegate mDelegate = null  ;
    public bool isSelected = false ;
    enum State
    {
        Act_Init,
        Act_OneClickDown,
        Act_WaitSecondClickDown,
        Act_OneClick,
        Act_SecondClick_Down,
        Act_DobuleClick,
        Act_DRAGIN,
    }
    State mActState = State.Act_Init ;
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDisable() 
    {
        GetComponent<BoxCollider>().enabled = false ;
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        this.mDelegate = null ;
    }
    private void OnEnable() {
        var col = GetComponent<BoxCollider>();
        if ( null == col )
        {
            col = this.gameObject.AddComponent<BoxCollider>();
        }
        col.enabled = true ;
        this.isSelected = false ;
        this.mActState = State.Act_Init ;
        this.gameObject.layer = LayerMask.NameToLayer("Default");
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.LogWarning("OnPointerDown");
        if ( this.isSelected == false )
        {
            this.mWorldPosBeforeDraging = this.transform.position ;
        }
        
        if ( eventData.pressEventCamera != null )
        {
            this.mDraginAssociateCamera = eventData.pressEventCamera;
        }

        if ( this.mActState == State.Act_Init )
        {
            this.mActState = State.Act_OneClickDown ;
        }
        else if ( State.Act_WaitSecondClickDown == this.mActState )
        {
            this.mActState = State.Act_SecondClick_Down ;
            this.CancelInvoke();
        }
    }
    void waitSecondClickDown()
    {
        this.mActState = State.Act_OneClick;
        Debug.Log("one clicked ");
        if ( this.mDelegate != null )
        {
            this.mDelegate.onOneClick(this);
        }
        this.mActState = State.Act_Init;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.LogWarning("do clicked mj + cnt = " + eventData.clickCount );
        if ( this.mActState == State.Act_OneClickDown )
        {
            // wait double click down 
            this.mActState = State.Act_WaitSecondClickDown;
            this.Invoke("waitSecondClickDown",0.2f) ;
        }
        else if ( this.mActState == State.Act_SecondClick_Down )
        {
            this.mActState = State.Act_DobuleClick;
            // do double click ;
            Debug.Log("double clicked");
            if ( this.mDelegate != null )
            {
                this.mDelegate.onDoubleClick(this);
            }
            this.mActState = State.Act_Init;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.LogWarning("OnBeginDrag");
        this.mActState = State.Act_DRAGIN;
        
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag " + eventData.position );
        var cam = this.mDraginAssociateCamera;//GameObject.Find("Main Camera").GetComponent<Camera>();
        //var point = cam.ScreenToWorldPoint(new Vector3(eventData.position.x,eventData.position.y, cam.nearClipPlane));
        var inc = cam.transform.InverseTransformPoint(this.transform.position);
        var point = cam.ScreenToWorldPoint(new Vector3(eventData.position.x,eventData.position.y, inc.z));
        //point.y = this.transform.position.y ;
        point.z = this.transform.position.z ;
        if ( point.y < this.mWorldPosBeforeDraging.y )
        {
            point.y = this.mWorldPosBeforeDraging.y ;
        }
        this.transform.position = point ;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.LogWarning("OnEndDrag");
        bool isOut = this.transform.position.y - this.mWorldPosBeforeDraging.y > GetComponent<MJCard>().world_y_Size ;
        if ( isOut && this.mDelegate != null && this.mDelegate.onDragOut(this) == true )
        {

        } 
        else
        {
            this.transform.position = this.mWorldPosBeforeDraging ;
        }
        this.mActState = State.Act_Init;
    }
}
