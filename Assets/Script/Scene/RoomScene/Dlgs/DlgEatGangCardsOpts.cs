using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System ;
using UnityEngine.Events ;
public class DlgEatGangCardsOpts : MonoBehaviour
{
    [Serializable]
    public class ChosedGangEvent : UnityEvent<int>{};
    [Serializable]
    public class ChosedEatEvent : UnityEvent<eEatType>{};
    // Start is called before the first frame update
    public Transform mGang;
    public List<Transform> mGangs ;
    public List<RawImage> mGangImg ;

    public Transform mEat ;
    public List<Transform> mEats ;

    public List<RawImage> mEatImg0 ;
    public List<RawImage> mEatImg1 ;
    public List<RawImage> mEatImg2 ;

    List<int> mGangCards = new List<int>();
    List<eEatType> mEatTypes = new List<eEatType>();
    public ChosedEatEvent onChosedEat = null ;
    public ChosedGangEvent onChosedGang = null ;
    void Start()
    {
        clear();

        // List<int> vc = new List<int>();
        // vc.Add(MJCard.makeCardNum(eMJCardType.eCT_Tong, 1 ));
        // vc.Add(MJCard.makeCardNum(eMJCardType.eCT_Wan, 2 ));
        // vc.Add(MJCard.makeCardNum(eMJCardType.eCT_Wan, 4 ));

        // showGangOpts(vc);
        // List<eEatType> vc = new List<eEatType>();
        // vc.Add(eEatType.eEat_Left);
        // vc.Add(eEatType.eEat_Middle);
        // vc.Add(eEatType.eEat_Righ);
        // showEatOpts(vc,MJCard.makeCardNum(eMJCardType.eCT_Wan, 4 ));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onSelGang0()
    {
        //Debug.Log("onSelGang0");
        if ( this.onChosedGang != null )
        {
            this.onChosedGang.Invoke(this.mGangCards[0]);
        }
        this.close();
    }

    public void onSelGang1()
    {
        //Debug.Log("onSelGang1");
        if ( this.onChosedGang != null )
        {
            this.onChosedGang.Invoke(this.mGangCards[1]);
        }
        this.close();
    }

    public void onSelGang2()
    {
        //Debug.Log("onSelGang2");
        if ( this.onChosedGang != null )
        {
            this.onChosedGang.Invoke(this.mGangCards[2]);
        }
        this.close();
    }
    public void onSelGang3()
    {
        //Debug.Log("onSelGang3");
        if ( this.onChosedGang != null )
        {
            this.onChosedGang.Invoke(this.mGangCards[3]);
        }
        this.close();
    }

    public void onSelEat0()
    {
        //Debug.Log("onSelEat0");
        if ( this.onChosedEat != null )
        {
            this.onChosedEat.Invoke(this.mEatTypes[0]);
        }
        this.close();
    }

    public void onSelEat1()
    {
        //Debug.Log("onSelEat1");
        if ( this.onChosedEat != null )
        {
            this.onChosedEat.Invoke(this.mEatTypes[1]);
        }    
        this.close();   
    }

    public void onSelEat2()
    {
        //Debug.Log("onSelEat2");
        if ( this.onChosedEat != null )
        {
            this.onChosedEat.Invoke(this.mEatTypes[2]);
        }    
        this.close();    
    }

    public void showEatOpts( List<eEatType> vEatOpts , int nTargetCard )
    {
        this.clear();
        this.gameObject.SetActive(true);
        this.mEatTypes.AddRange(vEatOpts);
        this.mEat.gameObject.SetActive(true);
        for ( int i = 0 ; i < this.mEatTypes.Count ; ++i )
        {
            this.mEats[i].gameObject.SetActive(true);
            List<RawImage> imgs = null ;
            switch ( i )
            {
                case 0 :
                {
                    imgs = this.mEatImg0;
                }
                break ;
                case 1 :
                {
                    imgs = this.mEatImg1;
                }
                break ;
                case 2 :
                {
                    imgs = this.mEatImg2;
                }
                break ;
                default:
                Debug.LogError("unknown eat type ");
                continue ;
            }
            
            switch ( this.mEatTypes[i] )
            {
                case eEatType.eEat_Left:
                {
                    this.updateCard(imgs[0],nTargetCard ) ;
                    this.updateCard(imgs[1],nTargetCard + 1 ) ;
                    this.updateCard(imgs[2],nTargetCard + 2 ) ;
                }
                break ;
                case eEatType.eEat_Middle:
                {
                    this.updateCard(imgs[0],nTargetCard -1 ) ;
                    this.updateCard(imgs[1],nTargetCard  ) ;
                    this.updateCard(imgs[2],nTargetCard + 1 ) ;
                }
                break ;
                case eEatType.eEat_Righ:
                {
                    this.updateCard(imgs[0],nTargetCard -2 ) ;
                    this.updateCard(imgs[1],nTargetCard - 1 ) ;
                    this.updateCard(imgs[2],nTargetCard ) ;
                }
                break ;
                default:
                {
                    Debug.LogError("invalid eat type = " + this.mEatTypes[i] );
                    break ;
                }
            }
        }
    }
    public void showGangOpts( List<int> vGangOpts )
    {
        this.clear();
        this.gameObject.SetActive(true);
        this.mGangCards.AddRange(vGangOpts) ;
        this.mGang.gameObject.SetActive(true);
        for ( int i = 0 ; i < this.mGangCards.Count ; ++i )
        {
            this.mGangs[i].gameObject.SetActive(true);
            this.updateCard(this.mGangImg[i],this.mGangCards[i] ) ;
        }
    }

    void updateCard( RawImage img , int cardNum )
    {
        eMJCardType type = MJCard.parseCardType(cardNum);
        var value = MJCard.parseCardValue(cardNum); 
        int startValue = 0 ;
        switch( type )
        {
            case eMJCardType.eCT_Wan:
            {
                startValue = 17 ;
            }
            break;
            case eMJCardType.eCT_Tong:
            {
                startValue = 33 ;
            }
            break;
            case eMJCardType.eCT_Tiao:
            {
                startValue = 49 ;
            }
            break;
            case eMJCardType.eCT_Feng:
            {
                startValue = 65 ;
            }
            break;
            case eMJCardType.eCT_Jian:
            {
                startValue = 81 ;
            }
            break;
            default:
            {
                Debug.LogError("unknown card type = " + type);
            }
            return;
        }
        var url = "majiangpai/"+(startValue + value - 1 ) ;
        var i = Resources.Load<Texture>(url);
        if ( i == null )
        {
            Debug.LogError("failed load texture = " + url );
        }
        img.texture = Resources.Load<Texture>(url);
    }
    void clear()
    {
        this.mEat.gameObject.SetActive(false);
        foreach (var item  in this.mEats )
        {
            item.gameObject.SetActive(false);            
        }
        this.mGang.gameObject.SetActive(false);

        foreach (var item  in this.mGangs )
        {
            item.gameObject.SetActive(false);            
        }

        this.mGangCards.Clear();
        this.mEatTypes.Clear();
    }
    public void close()
    {
        this.gameObject.SetActive(false);
    }
}
