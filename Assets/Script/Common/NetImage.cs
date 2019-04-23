using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System ;

public class NetImage : MonoBehaviour
{
    // Start is called before the first frame update
    public RawImage image ;
    private Texture defaultTet = null;
    private string  curUrl = "" ;
    public string url
    {
        get{ return curUrl ;}
        set 
        {
            if ( string.IsNullOrEmpty(value) )  // do reset photo
            {
                if ( this.defaultTet != null )
                {
                    this.image.texture = this.defaultTet;
                }
                this.curUrl = value ;
                return ;
            }

            if ( Uri.IsWellFormedUriString(value,UriKind.Absolute) == false )
            {
                Debug.LogError("invalid url = " + value);
                return ;
            }

            if ( string.Equals(this.curUrl,value) )
            {
                Debug.LogWarning("the same url do not load url = " + value );
                return ;
            }

            this.curUrl = value;
            if ( this.defaultTet != null )
            {
                this.image.texture = this.defaultTet;
            }

            this.downloadImage();
        }
    }

    static Dictionary<string,Texture> s_ImageCacher = new Dictionary<string,Texture>();
    void Start()
    {
        if ( null == this.image )
        {
            this.image = this.GetComponent<RawImage>();
            if ( null == this.image )
            {
                Debug.LogError("image atrribute is not set , script must bind to rawImage or image property can not be null ");
                return ;
            }
        }

        if ( this.defaultTet == null )
        {
            this.defaultTet = this.image.texture ; 
        }
    }

    IEnumerator loadImage()
    {
        if ( string.IsNullOrEmpty(this.curUrl) )
        {
            Debug.LogWarning("url is null or empty");
            yield break;
        }

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(this.curUrl);
        yield return www.SendWebRequest();

        if( www.isNetworkError || www.isHttpError ) {
            Debug.Log(www.error);
            Invoke("downloadImage",2);
        }
        else {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            this.image.texture = myTexture ;
            if ( s_ImageCacher.Count > 300 )
            {
                s_ImageCacher.Clear();
            }
            s_ImageCacher.Add(this.curUrl,myTexture);
        }
    }

    void downloadImage()
    {
        Texture t = null ;
        if ( s_ImageCacher.TryGetValue(this.curUrl,out t) )
        {
            this.image.texture = t;
            Debug.Log("already in cacher textrue");
            return ;
        }
        this.StartCoroutine("loadImage");
    } 
}
