using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones ;
using DG.Tweening ;
public class PlayerInteractEmoji : MonoBehaviour
{
    public UnityDragonBonesData mEmojiAniData ;
    Dictionary<string,List<UnityArmatureComponent>> mEmojis = new Dictionary<string,List<UnityArmatureComponent>>();
    List<UnityArmatureComponent> mPlayingAni = new List<UnityArmatureComponent>();
    bool mIsLoadedData = false ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playInteractEmoji( Vector3 startPos , Vector3 targetPos , int emojiIdx )
    {
        if ( emojiIdx != 7 )
        {
            var ani = this.getAni( "item" + emojiIdx);
            ani.gameObject.SetActive(true);
            ani.transform.position = startPos;
            ani.animation.Play("item"+emojiIdx);
            //ani.animation.GotoAndStopByFrame("item"+emojiIdx,2);
            var move = ani.transform.DOMove(targetPos,0.3f) ;
            //DOTween.Sequence().Append(move).AppendCallback( ()=>{ ani.animation.Play("item"+emojiIdx);} );
        }
        else
        {
            var ani = this.getAni( "item" + emojiIdx);
            ani.gameObject.SetActive(true);
            ani.transform.position = startPos;
            ani.animation.Play("item" + emojiIdx);

            var aniTarget = this.getAni( "item7_bow" );
            aniTarget.gameObject.SetActive(true);
            aniTarget.transform.position = targetPos;
            aniTarget.animation.Play( "item7_bow" );
        }
    }

    UnityArmatureComponent getAni( string name )
    {
        if ( this.mEmojis.ContainsKey( name ) )
        {
            var p = this.mEmojis[name];
            if ( p != null && p.Count > 0 )
            {
                var ani = p[0] ;
                p.RemoveAt(0);
                return ani ;
            }
        }

        if ( this.mIsLoadedData == false )
        {
            UnityFactory.factory.LoadData(this.mEmojiAniData,true,1);
            this.mIsLoadedData = true ;
        }
        
        var drag = UnityFactory.factory.BuildArmatureComponent(name,"","","",null,true ) ;
        if ( drag == null )
        {
            return null ;
        }

        drag.transform.parent = this.transform ;
        drag.transform.localPosition = Vector3.zero;
        drag.gameObject.SetActive(false);

        drag.AddDBEventListener(DragonBones.EventObject.COMPLETE,( string type, DragonBones.EventObject eventObject )=>{ this.onPlayComplete(drag) ;}) ;
        return drag ;
    }

    public void onPlayComplete( UnityArmatureComponent drag )
    {
        Debug.Log("finish ani");
         this.mPlayingAni.Remove(drag) ;
         var name = drag.name ;
         if ( this.mEmojis.ContainsKey( name) )
         {
             this.mEmojis[name].Add(drag);
             drag.gameObject.SetActive(false);
             return ;
         }
         
         var plist = new List<UnityArmatureComponent>();
         this.mEmojis.Add( name,plist );
         plist.Add(drag);
         drag.gameObject.SetActive(false);
         return ;
    }
}
