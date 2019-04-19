using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubRecorderData : ClubDataCompoent
{
    public RecorderData vRecorder = null ;

    public override void fetchData( bool isforce )
    {
        if ( isforce == false && false == this.isDataOutOfDate() )
        {
            this.doInformDataRefreshed(false);
            return ;
        }
        
        if ( this.vRecorder == null )
        {
            this.vRecorder = new RecorderData() ;
            this.vRecorder.init(this.clubID,true) ;
        }
        this.vRecorder.fetchData( ( RecorderData data, bool isCacher )=>{ this.doInformDataRefreshed(true) ;} );
    } 
}
