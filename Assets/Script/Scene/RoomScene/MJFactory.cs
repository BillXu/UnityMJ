using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MJFactory : MonoBehaviour
{
    Dictionary<int,List<MJCard> > vChacerCards = new Dictionary<int,List<MJCard>>();
    List<MJCard> vUnknownCards = new List<MJCard>();
    // Start is called before the first frame update
    private void Awake() {
        if ( this.vChacerCards.Count == 0 && this.vUnknownCards.Count == 0 )
        {
            this.prepareCard();
        }
    }

    void prepareCard()
    {
        // create unknown cards , we sigle 1 tong as kunown ;
        int cnt = 9 * 4 * 3 + 3 * 4 + 4 * 4 + 8;
        while ( cnt-- > 0  ) // clone left ;
        {
            var unknwon = createMJCardObject(eMJCardType.eCT_Tong,1) ;
            this.recycleUnknownCard(unknwon);
        }

        // create normal cards ; , wan tong tiao ;
        for ( int value = 1 ; value <= 9 ; ++value )
        {
            for ( int nCnt = 0 ; nCnt < 4 ; ++nCnt )
            {
                var p = this.createMJCardObject(eMJCardType.eCT_Wan,value);
                if ( p )
                {
                    this.recycleCard(p);
                }

                p = this.createMJCardObject(eMJCardType.eCT_Tong,value);
                if ( p )
                {
                    this.recycleCard(p);
                }

                p = this.createMJCardObject(eMJCardType.eCT_Tiao,value);
                if ( p )
                {
                    this.recycleCard(p);
                }
            }
        }

        // create feng jian
        for ( int value = 1 ; value <= 3 ; ++value )
        {
            for ( int nCnt = 0 ; nCnt < 4 ; ++nCnt )
            {
                var p = this.createMJCardObject(eMJCardType.eCT_Feng,value);
                if ( p )
                {
                    this.recycleCard(p);
                }

                p = this.createMJCardObject(eMJCardType.eCT_Jian,value);
                if ( p )
                {
                    this.recycleCard(p);
                }
            }
        } 

        // feng ---dong
        for ( int nCnt = 0 ; nCnt < 4 ; ++nCnt )
        {
            var p = this.createMJCardObject(eMJCardType.eCT_Feng,4);
            if ( p )
            {
                this.recycleCard(p);
            }
        }

        // create hua 
        for ( int value = 1 ; value <= 8 ; ++value )
        {
            var p = this.createMJCardObject(eMJCardType.eCT_Hua,value);
            if ( p )
            {
                this.recycleCard(p);
            }
        }

    }

    public void recycleCard( MJCard card )
    {
        if ( card == null )
        {
            Debug.LogWarning( "do not have mj card compoent" );
            return ;
        }

        var num = card.cardNum ;
        if ( this.vChacerCards.ContainsKey(num) == false )
        {
            var L = new List<MJCard>();
            this.vChacerCards.Add(num,L);
        }
        this.vChacerCards[num].Add(card);
        card.gameObject.SetActive(false);
        card.transform.SetParent(this.transform);
    }
    
    public MJCard getCard( int num, Transform parent )
    {

        if ( this.vChacerCards.ContainsKey(num) == false || this.vChacerCards[num] == null )
        {
            Debug.LogError("do not have card num = " + num + " type =" + MJCard.parseCardType(num) + " value = " + MJCard.parseCardValue(num) );
            return getUnknownCard(parent);
        }

        var v = this.vChacerCards[num] ;
        if ( v.Count == 0 )
        {
            Debug.LogError("why do not have this card ? , more than 4 = " + num);
            return getUnknownCard(parent);
        }

        var tmp = v[0] ;
        v.RemoveAt(0);
        tmp.transform.SetParent(parent);
        tmp.gameObject.SetActive(true);
        return tmp ;
    }

    public void recycleUnknownCard( MJCard card )
    {
        if ( card == null )
        {
            Debug.LogWarning( "do not have mj card compoent" );
            return ;
        }
        card.gameObject.SetActive(false);
        card.transform.SetParent(this.transform);
        this.vUnknownCards.Add(card);
    }
    public MJCard getUnknownCard( Transform parent )
    {
        if ( this.vUnknownCards.Count == 0 )
        {
            Debug.LogError("it is impossile , why unknown card is null ?");
            return null ;
        }

        var tmp = this.vUnknownCards[0] ;
        this.vUnknownCards.RemoveAt(0);
        tmp.transform.SetParent(parent);
        tmp.gameObject.SetActive(true);
        return tmp ;
    }
    MJCard createMJCardObject(eMJCardType type , int value )
    {
        string[] v = new string[]{ "","wan","tong","tiao","feng","jian","hua"} ;
        if ( (int)type >= v.Length )
        {
            Debug.LogError("invalid card type when create card block =" + type + " v = " + value );
            return null ;
        }

        var m = Resources.Load<GameObject>("mjmesh/mj_"+v[(int)type] + value);
        if ( null == m )
        {
            Debug.LogError("load mesh is null ?");
            return null;
        }

        var ms = Instantiate(m) ;
        ms.transform.SetParent(this.transform);
        ms.name = v[(int)type] + value;
        var c = ms.AddComponent<MJCard>();
        c.cardNum = MJCard.makeCardNum(type,value) ;
        return c;
    }
}
