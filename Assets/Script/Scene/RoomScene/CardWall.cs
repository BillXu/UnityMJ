using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardWall : MonoBehaviour
{
    // Start is called before the first frame update
    public MJFactory mFactory ;
    void Start()
    {
        for ( int i = 0 ; i < 16 ; ++i )
        {
            var p = mFactory.getUnknownCard();
            p.transform.SetParent(this.transform);
            p.gameObject.SetActive(true);
            p.curState = MJCard.state.FACE_COVER ;
            p.transform.localPosition = new Vector3(i * p.world_x_Size,0,0 );

            p = mFactory.getUnknownCard();
            p.transform.SetParent(this.transform);
            p.gameObject.SetActive(true);
            p.curState = MJCard.state.FACE_COVER ;
            p.transform.localPosition = new Vector3(i * p.world_x_Size,p.world_y_Size,0 );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
