using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON ;
using System.Text ;
public class ResultSingleData : MonoBehaviour
{
    public class ResultItem
    {
        public int mHuScore ;
        public int mGangScore ;
        public int mOffset ; 
        public int mIdx ;
        public bool mIsZiMo = false ;
        public List<eFanxingType> mHuTypes = null;

        public int mFinalChip ;
        public List<int> mAnHoldCards = new List<int>();

        public void clear()
        {
            this.mHuScore = 0 ;
            this.mGangScore = 0 ;
            this.mIdx = -1 ;
            this.mHuTypes = null ;
            this.mFinalChip = 0 ;
            this.mOffset = 0 ;
            this.mIsZiMo = false ;
            this.mAnHoldCards.Clear();
        }

        public bool isEmpty()
        {
            return this.mIdx == -1 ;
        }

        public bool haveHu()
        {
            return this.mHuTypes != null && this.mHuTypes.Count > 0 ;
        }

        public string getHuTypeStr()
        {
            var str = new StringBuilder() ;
            for ( int i = 0 ; i < this.mHuTypes.Count ; ++i )
            {
               if ( i != 0 )
               {
                   str.Append( " " ) ;
               }

               str.Append("【");
               str.Append(this.huTypeToString(this.mHuTypes[i]));
               str.Append("】");
            }
            return str.ToString() ;
        }
        public string huTypeToString( eFanxingType type )
        {
            string strHu = "" ;
            switch ( type )
            {
                case eFanxingType.eFanxing_PingHu:
                {
                    strHu = "平胡";
                }
                break;
                case eFanxingType.eFanxing_QingYiSe:
                {
                    strHu = "清一色";
                }
                break ;
                case eFanxingType.eFanxing_MengQing:
                {
                    strHu = "门清";
                }
                break ;
                case eFanxingType.eFanxing_QiangGang:
                {
                    strHu = "抢杠胡";
                }
                break;
                case eFanxingType.eFanxing_DaMenQing:
                {
                    strHu = "大门清";
                }
                break;
                case eFanxingType.eFanxing_XiaoMenQing:
                {
                    strHu = "小门清";
                }
                break ;
                case eFanxingType.eFanxing_GangKai:
                {
                    strHu = "杠开";
                }
                break ;
                case eFanxingType.eFanxing_JiaHu:
                {
                    strHu = "夹胡";
                }
                break;
                case eFanxingType.eFanxing_QiDui:
                {
                    strHu = "七对";
                }
                break;
                case eFanxingType.eFanxing_QuanQiuDuDiao:
                {
                    strHu = "全球独钓";
                }
                break ;
                case eFanxingType.eFanxing_HunYiSe:
                {
                    strHu = "混一色";
                }
                break;
                case eFanxingType.eFanxing_DiHu:
                {
                    strHu = "地胡";
                }
                break;
                case eFanxingType.eFanxing_ShuangQiDui:
                {
                    strHu = "双七对";
                }
                break;
                case eFanxingType.eFanxing_WuHuaGuo:
                {
                    strHu = "无花果";
                }
                break ;
                case eFanxingType.eFanxing_BianHu:
                {
                    strHu = "边胡";
                }
                break;
                case eFanxingType.eFanxing_DuiDuiHu:
                {
                    strHu = "对对胡";
                }
                break;
                case eFanxingType.eFanxing_GangHouPao:
                {
                    strHu = "杠后炮" ;
                }
                break;
                case eFanxingType.eFanxing_YaJue:
                {
                    strHu = "压绝" ;
                }
                break;
                default:
                {
                    strHu = type.ToString();
                }
                break ;
            }
            return strHu ;
        }

    }
    public List<ResultItem> mResults = new List<ResultItem>(); 

    public bool mIsLiuJu = true ;
    public void parseResult( JSONObject js )
    {
        if ( this.mResults.Count == 0 )
        {
            this.mResults.Add( new ResultItem() );
            this.mResults.Add( new ResultItem() );
            this.mResults.Add( new ResultItem() );
            this.mResults.Add( new ResultItem() );
        }

        foreach ( var item in this.mResults )
        {
            item.clear();
        }
        this.mIsLiuJu = true ;

        var realTimeCal = js["realTimeCal"].Array ;
        foreach (var item in realTimeCal )
        {
            var obj = item.Obj ;
            var actType = (eMJActType)obj["actType"].Number;
            var detail = obj["detial"].Array;
            for ( int i = 0; i < detail.Length; i++ )
            {
                var ret = detail[i].Obj;
                var idx = (int)ret["idx"].Number;
                var offset = (int)ret["offset"].Number;
                if ( actType == eMJActType.eMJAct_Hu )
                {
                    this.mResults[idx].mHuScore += offset ;
                }
                else
                {
                    this.mResults[idx].mGangScore += offset ;
                }
                this.mResults[idx].mIdx = idx ;
            }

            if ( actType == eMJActType.eMJAct_Hu )
            {
                this.mIsLiuJu = false ;
                this.parseHuInfo( obj["msg"].Obj );
            }
        }
        var players = js["players"].Array ;
        foreach (var itemPlayer in players )
        {
            var obj = itemPlayer.Obj ;
            int idx = (int)obj["idx"].Number;
            int offset = (int)obj["offset"].Number;
            int chip = (int)obj["chips"].Number;
            var holdCards = obj["holdCard"].Array;
            var itp = this.mResults[idx];
            itp.mIdx = idx ;
            itp.mFinalChip = chip ;
            itp.mOffset = offset ;
            for (int i = 0; i < holdCards.Length; i++)
            {
                itp.mAnHoldCards.Add((int)holdCards[i].Number);
            }
        }
    }

    void parseHuInfo( JSONObject jsHuInfo )
    {
        bool isZiMo = ((int)jsHuInfo["isZiMo"].Number) == 1 ;
        var detail = jsHuInfo["detail"].Obj ;
        if ( isZiMo )
        {
            int idx = (int)detail["huIdx"].Number ;
            var vht = detail["vhuTypes"].Array ;
            this.mResults[idx].mIsZiMo = true ;
            for ( int i = 0; i < vht.Length; i++ )
            {
                if ( null == this.mResults[idx].mHuTypes )
                {
                    this.mResults[idx].mHuTypes = new List<eFanxingType>();
                } 
                this.mResults[idx].mHuTypes.Add( (eFanxingType)vht[i].Number );
            }
        }
        else
        {
            var huPlayers = detail["huPlayers"].Array ;
            for (int i = 0; i < huPlayers.Length; i++)
            {
                var obj = huPlayers[i].Obj;
                int idx = (int)obj["idx"].Number ;
                var vht = obj["vhuTypes"].Array ;
                for ( int ht= 0; ht < vht.Length; ht++ )
                {
                    if ( null == this.mResults[idx].mHuTypes )
                    {
                        this.mResults[idx].mHuTypes = new List<eFanxingType>();
                    } 
                    this.mResults[idx].mHuTypes.Add( (eFanxingType)vht[ht].Number );
                }
            }
        }
    }
}
