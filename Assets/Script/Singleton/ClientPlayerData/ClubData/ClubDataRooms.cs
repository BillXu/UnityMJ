using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public class RoomPeer
{
    public int nUID = 0 ;
    public bool isOnline = false ;
}

public class ClubRoomItem
{
    public int nRoomID = 0 ;
    public List<RoomPeer> vRoomPeers = null ;
    public bool isOpen = false ;
    public int playedRound = 0 ;
    public int totalRound = 0 ;
    public bool isCircle = false ;
    public JSONObject jsmsgBrife { get ; set ;} = null ;
    public int seatCnt
    {
        get
        {
            return (int)this.jsmsgBrife["opts"].Obj["seatCnt"].Number;
        }
        
    }

    public JSONObject opts 
    {
        get
        {
            return this.jsmsgBrife["opts"].Obj ;
        }
        
    }
}
 
public class ClubDataRooms : ClubDataCompoent {

    public List<ClubRoomItem> vRooms = new List<ClubRoomItem>() ;

    public override void fetchData( bool isforce )
    {
        if ( false == isforce && this.isDataOutOfDate() == false )
        {
            this.doInformDataRefreshed(false) ;
            return ;
        }

        Debug.Log( "req clubl rooms clubID = " + this.clubID );
        var js = new JSONObject() ;
        js["clubID"] = this.clubID ;
        Network.getInstance().sendMsg(js,eMsgType.MSG_CLUB_REQ_ROOMS,eMsgPort.ID_MSG_PORT_CLUB,this.clubID ) ;
    }

    public override bool onMsg( eMsgType msgID, JSONObject msgData ) 
    {
        if ( eMsgType.MSG_CLUB_REQ_ROOMS == msgID )
        {
            // svr : { clubID : 234, name : 23, fullRooms : [ 23,23,4 ], emptyRooms :  [  23,2, .... ]  }
            if ( (int)msgData["clubID"].Number != this.clubID )
            {
                return false ;
            }
            this.vRooms.Clear() ;
            var jsvRooms = msgData["fullRooms"];
            if ( jsvRooms != null )
            {
                var jsa = jsvRooms.Array;
                foreach( var item in jsa )
                {
                    var p = new ClubRoomItem();
                    p.nRoomID = (int)item.Number ;
                    this.vRooms.Add(p);
                }
            }

            jsvRooms = msgData["emptyRooms"];
            if ( jsvRooms != null )
            {
                var jsa = jsvRooms.Array;
                foreach( var item in jsa )
                {
                    var p = new ClubRoomItem();
                    p.nRoomID = (int)item.Number ;
                    this.vRooms.Add(p);
                }
            }
            if ( this.vRooms.Count == 0 )
            {
                this.doInformDataRefreshed(true);
                return true ;
            }
            this.requestRoomsDetail();
            return true ;
        }

        if ( eMsgType.MSG_REQ_ROOM_ITEM_INFO == msgID )
        {
            int roomID = (int)msgData["roomID"].Number ;
            var p = this.getRoomByRoomID(roomID);
            if ( p == null )
            {
                return false ;
            }
            
            p.jsmsgBrife = msgData ;
            var vPlayers = msgData["players"];
            if ( vPlayers != null )
            {
                var jsv = vPlayers.Array ;
                foreach (var item in jsv )
                {
                    var pi = new RoomPeer();
                    pi.isOnline = true ;
                    pi.nUID = (int)item.Number ;
                    p.vRoomPeers.Add(pi);
                }  
            }


            var isAllRoomRecievedInfo = this.vRooms.Find(( ClubRoomItem d  )=>{ return d.jsmsgBrife == null;}) == null;
            if ( isAllRoomRecievedInfo )
            {
                this.doInformDataRefreshed(true);
            }
            return true ;
        }
        return false ;
    }

    protected void requestRoomsDetail()
    {
        var js = new JSONObject();
        foreach (var item in this.vRooms )
        {
            if ( item.jsmsgBrife != null )
            {
                continue ;
            } 

            Debug.Log("req club room detial id = " + item.nRoomID);
            js["roomID"] = item.nRoomID ;
            Network.getInstance().sendMsg(js,eMsgType.MSG_REQ_ROOM_ITEM_INFO,Utility.getMsgPortByRoomID(item.nRoomID),item.nRoomID ) ;
        } 
    }

    public ClubRoomItem getRoomByRoomID( int roomID )
    {
        foreach (var item in this.vRooms )
        {
            if ( roomID == item.nRoomID )
            {
                return item ;
            }  
        } 
        return null ;
    }

}

