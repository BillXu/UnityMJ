using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
public class ClubMember
{
    public int uid = 0 ;
    public eClubPrivilige privliage = eClubPrivilige.eClubPrivilige_Normal ;
}

public class ClubDataMembers : ClubDataCompoent {

    public List<ClubMember> vMembers = null ;
    public override void fetchData( bool isforce )
    {
        if ( false == isforce && false == this.isDataOutOfDate() )
        {
            this.doInformDataRefreshed(false);
            return ;
        } 

        JSONObject msg = new JSONObject() ;
        msg["clubID"] = this.clubID ;
        Network.getInstance().sendMsg(msg,eMsgType.MSG_CLUB_REQ_PLAYERS,eMsgPort.ID_MSG_PORT_CLUB,this.clubID) ;
        Debug.Log( "featch data clubid = " + this.clubID );
    }

    public override bool onMsg( eMsgType msgID , JSONObject msgData )
    {
        if ( eMsgType.MSG_CLUB_REQ_PLAYERS == msgID )
        {
            var clubID = msgData["clubID"] ;
            if ( clubID == null )
            {
                Prompt.promptDlg("MSG_CLUB_REQ_PLAYERS msg must have clubID key , inform server add it") ;
                return true;
            }

            if ( this.clubID != (int)clubID.Number )
            {
                return false ;
            }

            JSONArray vM  = msgData["players"].Array ;
            var pageIdx = (int)msgData["pageIdx"].Number ;
            if ( 0 == pageIdx )
            {
                if ( this.vMembers == null )
                {
                    this.vMembers = new List<ClubMember>();
                }
                this.vMembers.Clear() ;
            }

            foreach ( var p in vM )
            {
                var mem = new ClubMember();
                mem.uid = (int)p.Obj["uid"].Number ;
                mem.privliage = (eClubPrivilige)p.Obj["privilige"].Number ;
                this.vMembers.Add(mem);
            }

            if ( vM.Length < 10 )
            {
                this.doInformDataRefreshed(true);
            }
            return true ;
        }
        return false ;
    }
    public ClubMember getClubMember( int uid )    {
        foreach (var item in this.vMembers )
        {
            if ( item.uid == uid )
            {
                return item ;
            }
        }  
        return null ;
    }

    public int getMemberCnt()
    {
        if ( this.vMembers != null )
        {
            return this.vMembers.Count;
        }

        if ( this.getClub().getClubBaseData().jsCore != null )
        {
            return (int)this.getClub().getClubBaseData().jsCore["curCnt"].Number;
        }
        return 0 ;
    }

    public bool isPlayerMgr( int uid )
    {
        if ( this.vMembers == null )
        {
            var datajs = this.getClub().getClubBaseData().jsCore ;
            if ( datajs == null || datajs.ContainsKey("mgrs") == false )
            {
                return false;
            }

            var vMgr = datajs["mgrs"].Array;
            foreach( var item in vMgr )
            {
                if ( uid == (int)item.Number )
                {
                    return true;
                }
            }
            return false;
        }

        var p = getClubMember(uid);
        if ( null == p )
        {
            return false ;
        }

        return p.privliage == eClubPrivilige.eClubPrivilige_Manager ;
    }

    public bool setNewCreator( int nNewOwnerUid )
    {
        if ( this.vMembers != null )
        {
            var mem = getClubMember(nNewOwnerUid);
            if ( mem == null )
            {
                Debug.Log("candinate uid = " + nNewOwnerUid + " is not member of club " + this.clubID + " can not be new owner" );
                return false ;
            }

            mem.privliage = eClubPrivilige.eClubPrivilige_Creator;
            var preOwner = getClubMember(this.getClub().getClubBaseData().creatorUID );
            if ( null == preOwner )
            {
                Debug.LogError("why pre owner is null clubid = " + this.clubID + " uid = " + this.getClub().getClubBaseData().creatorUID );
            }
            else
            {
                preOwner.privliage = eClubPrivilige.eClubPrivilige_Normal ;
            }
        }
        this.getClub().getClubBaseData().creatorUID = nNewOwnerUid ;
        return true ;
    }
}
